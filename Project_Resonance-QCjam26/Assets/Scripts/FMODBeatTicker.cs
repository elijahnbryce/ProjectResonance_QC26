using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

// Fires OnSixteenthNote / OnEighthNote / OnQuarterNote / OnHalfNote / OnWholeNote.
// Tempo authority belongs entirely to FMOD: no bpm field here at all. Instead,
// FMOD's TIMELINE_BEAT callback tells us, once per quarter note, exactly where
// the beat landed and what tempo was active. Between beats, sixteenth notes are
// subdivided in C# from that anchor. Every beat callback re-anchors the counter,
// so any subdivision error can only ever accumulate for a single beat before
// being corrected -- it never compounds across the whole song.
public class FMODBeatTicker : MonoBehaviour
{
    public static FMODBeatTicker _Instance;

    public event Action OnSixteenthNote, OnEighthNote, OnQuarterNote, OnHalfNote, OnWholeNote;

    private EventInstance musicInstance;
    private EVENT_CALLBACK beatCallback; // kept as a field so it isn't garbage collected

    // Written on FMOD's callback thread, read on the main thread in Update().
    // A plain class behind a GCHandle -- no locks needed since Update() only
    // ever reads it, and a torn read here just means "use last frame's beat
    // info," which self-corrects on the very next beat regardless.
    private class TimelineInfo
    {
        public int beatCount;             // increments once per callback; used to detect "is this a new beat"
        public double beatPositionSeconds; // FMOD timeline position, in seconds, at that beat
        public float tempo = 120f;
    }

    private TimelineInfo timelineInfo;
    private GCHandle timelineHandle;
    private int lastAppliedBeat = -1;

    // The single shared counter every coarser subdivision derives from.
    public int sixteenthNoteCount { get; private set; }

    private double anchorSeconds;
    private int anchorSixteenthCount;
    private double secondsPerSixteenthNote;
    private bool hasAnchor;
    private bool isRunning;

    [Header("Song")]
    public List<Note> songNotes = new();
    public List<Note> currentNote = new();
    private Note tempNote = new();
    private float bufferWindow = 0.067f;

    private void Awake()
    {
        if (_Instance == null)
            _Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnDisable()
    {
        OnSixteenthNote = null;
        OnEighthNote = null;
        OnQuarterNote = null;
        OnHalfNote = null;
        OnWholeNote = null;
    }

    private void OnDestroy()
    {
        if (timelineHandle.IsAllocated)
            timelineHandle.Free();
    }

    public void StartTicker(EventReference musicEvent)
    {
        // Register TimelineInfo Memory to not be garbage collected by GC
        timelineInfo = new TimelineInfo();
        timelineHandle = GCHandle.Alloc(timelineInfo);

        // Hand FMOD a native C++ pointer to our TimelineInfo memory space
        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));

        // Assign and pass static reference to our local callback function
        beatCallback = BeatEventCallback;
        musicInstance.setCallback(beatCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

        sixteenthNoteCount = 0;
        hasAnchor = false;
        lastAppliedBeat = -1;

        currentNote.Clear();
        OnSixteenthNote += PeekNextNote;

        musicInstance.start();
        isRunning = true;
    }

    public void Stop()
    {
        isRunning = false;
        OnSixteenthNote -= PeekNextNote;
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }

    // Runs on FMOD's Studio Update thread, NOT Unity's main thread.
    // Must stay cheap and must never touch Unity APIs directly.
    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    private static FMOD.RESULT BeatEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        if (type != EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
            return FMOD.RESULT.OK;

        // Retreive musicInstance refernce from FMOD
        // Retreive native C++ pointer from musicInstance
        var instance = new EventInstance(instancePtr);
        instance.getUserData(out IntPtr userDataPtr);
        if (userDataPtr == IntPtr.Zero)
            return FMOD.RESULT.OK;

        // Retrieve native C++ ptr to the GC handle 
        // Convert native ptr to C# TimelineInfo obj
        var handle = GCHandle.FromIntPtr(userDataPtr);
        if (handle.Target is not TimelineInfo info)
            return FMOD.RESULT.OK;

        // Convert native C++ FMOD.Studio.TIMELINE_BEAT_PROPERTIES ptr to C# object
        var beatProps = (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_BEAT_PROPERTIES));

        info.beatPositionSeconds = beatProps.position / 1000.0;
        info.tempo = beatProps.tempo;
        info.beatCount++;

        return FMOD.RESULT.OK;
    }

    private void Update()
    {
        if (!isRunning) return;

        ApplyAnchorIfNewBeat();
        if (!hasAnchor) return;

        double elapsedSinceAnchor = GetTimelineSeconds() - anchorSeconds;
        if (elapsedSinceAnchor < 0) elapsedSinceAnchor = 0; // guard against tiny cross-thread timing skew

        int expectedSixteenthCount = anchorSixteenthCount + (int)(elapsedSinceAnchor / secondsPerSixteenthNote);

        while (sixteenthNoteCount < expectedSixteenthCount)
        {
            sixteenthNoteCount++;
            OnSixteenthNote?.Invoke();

            // Every coarser subdivision is just modulo on the same counter --
            // guarantees a quarter note and its coincident sixteenth note always
            // fire from the same value, with no independent timer to disagree.
            if (sixteenthNoteCount % 2 == 0) OnEighthNote?.Invoke();
            if (sixteenthNoteCount % 4 == 0) OnQuarterNote?.Invoke();
            if (sixteenthNoteCount % 8 == 0) OnHalfNote?.Invoke();
            if (sixteenthNoteCount % 16 == 0) OnWholeNote?.Invoke();
        }
    }

    private void ApplyAnchorIfNewBeat()
    {
        if (timelineInfo.beatCount == lastAppliedBeat) return; // no new beat callback since last check
        lastAppliedBeat = timelineInfo.beatCount;

        secondsPerSixteenthNote = (60.0 / timelineInfo.tempo) / 4.0;

        // A real quarter-note beat just happened, so this is where the "true"
        // sixteenth-note grid sits. Snapping to the nearest multiple of 4 corrects
        // any subdivision error from the previous beat without needing to know
        // the song's absolute bar/beat position.
        anchorSixteenthCount = hasAnchor ? RoundToNearestMultiple(sixteenthNoteCount, 4) : 0;
        anchorSeconds = timelineInfo.beatPositionSeconds;
        hasAnchor = true;
    }

    private static int RoundToNearestMultiple(int value, int multiple)
    {
        return Mathf.RoundToInt(value / (float)multiple) * multiple;
    }

    public double GetTimelineSeconds()
    {
        musicInstance.getTimelinePosition(out int positionMs);
        return positionMs / 1000.0;
    }

    public bool CheckNote() => currentNote.Count > 0;
    public bool OnNote() => SongManager._Instance.CheckNote(sixteenthNoteCount);

    private IEnumerator HighlightNote()
    {
        double now = GetTimelineSeconds();
        double frontWindow = Math.Max(0.0, secondsPerSixteenthNote - bufferWindow);
        double next = now + frontWindow;
        while (GetTimelineSeconds() < next)
            yield return null;

        currentNote.Add(tempNote);

        now = GetTimelineSeconds();
        double backWindow = 2.0 * bufferWindow;
        next = now + backWindow;
        while (GetTimelineSeconds() < next)
            yield return null;

        currentNote.Clear();
    }

    private void PeekNextNote()
    {
        if (SongManager._Instance.CheckNote(sixteenthNoteCount + 1))
        {
            StartCoroutine(HighlightNote());
        }
    }

    public void AutoAssignNoteOccurence(Action listener, int noteType)
    {
        switch (noteType)
        {
            case 1: OnWholeNote += listener; break;
            case 2: OnHalfNote += listener; break;
            case 4: OnQuarterNote += listener; break;
            case 8: OnEighthNote += listener; break;
            case 16: OnSixteenthNote += listener; break;
        }
    }
}