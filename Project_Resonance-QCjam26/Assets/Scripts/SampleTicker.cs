using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Fires OnTick / OnQuarterNote at a coarser granularity than Update(),
// driven entirely by AudioSettings.dspTime. No incremental accumulation --
// the current tick count is always recomputed fresh from dspTime each frame,
// so there's nothing to drift.
public class SampleTicker : MonoBehaviour
{
    //    public static SampleTicker _Instance;

    //    [Header("Song Timing (static BPM)")]
    //    public float bpm = 120f;

    //    public event Action OnSixteenthNote, OnEightNote, OnQuarterNote, OnHalfNote, OnFullNote;

    //    private double songStartDspTime;
    //    private double secondsPerSixteenthNote, secondsPerEightNote, secondsPerQuarterNote, secondsPerHalfNote, secondsPerWholeNote;

    //    // How many of each subdivision have fired so far. The threshold time
    //    // for the *next* one is always recomputed as count * unit, from the
    //    // fixed songStartDspTime origin -- never by adding onto a running total.
    //    public int sixteenthNoteCount, quarterNoteCount;

    //    private bool isRunning;

    //    [Header("Song")]
    //    public List<Note> songNotes = new();
    //    private Queue<Note> notesInView = new();
    //    public List<Note> currentNote = new();
    //    private Note tempNote = new();
    //    private float bufferWindow = 0.067f;

    //    private void Awake()
    //    {
    //        if (_Instance == null)
    //            _Instance = this;
    //        else
    //            Destroy(gameObject);
    //    }

    //    private void OnDisable()
    //    {
    //        OnQuarterNote = null;
    //        OnEightNote = null;
    //        OnSixteenthNote = null;
    //        OnHalfNote = null;
    //        OnFullNote = null;
    //    }

    //    // Call with the exact same dspTime value passed to AudioSource.PlayScheduled,
    //    // so the ticker's origin matches what the player actually hears.
    //    public void StartTicker(double songStartDspTime)
    //    {
    //        this.songStartDspTime = songStartDspTime;
    //        secondsPerQuarterNote = 60.0 / bpm;
    //        secondsPerSixteenthNote = secondsPerQuarterNote / 4.0;
    //        secondsPerEightNote = secondsPerQuarterNote / 2.0;
    //        secondsPerHalfNote = secondsPerQuarterNote * 2.0;
    //        secondsPerWholeNote = secondsPerQuarterNote * 4.0;

    //        sixteenthNoteCount = 0;
    //        quarterNoteCount = 0;
    //        isRunning = true;

    //        //OnSixteenthNote += PeekNextNote;
    //        currentNote.Clear();
    //    }

    //    public void Stop() => isRunning = false; // unsub peeknextnote

    //    private void Update()
    //    {
    //        if (!isRunning) return;

    //        double now = AudioSettings.dspTime;

    //        while (songStartDspTime + (1 + sixteenthNoteCount) * secondsPerSixteenthNote <= now)
    //        {
    //            sixteenthNoteCount++;
    //            OnSixteenthNote?.Invoke();
    //        }

    //        while (songStartDspTime + (1 + quarterNoteCount) * secondsPerQuarterNote <= now)
    //        {
    //            quarterNoteCount++;
    //            OnQuarterNote?.Invoke();
    //        }
    //    }

    public bool CheckNote() => FMODBeatTicker._Instance.CheckNote();
    public bool OnNote() => FMODBeatTicker._Instance.OnNote();

    //    //public bool CheckNote() => currentNote.Count > 0;
    //    //public bool CheckNoteTime(float dspTime = 0f) => true; // temp

    //    //public bool OnNote() => SongManager._Instance.CheckNote(sixteenthNoteCount);

    //    //private IEnumerator HighlightNote()
    //    //{
    //    //    double now = AudioSettings.dspTime;
    //    //    float frontWindow = Mathf.Max(0f, (float)(secondsPerSixteenthNote - bufferWindow));
    //    //    double next = now + frontWindow;
    //    //    while(AudioSettings.dspTime  < next) 
    //    //        yield return null;

    //    //    //yield return new WaitForSeconds(frontWindow);
    //    //    currentNote.Add(tempNote);

    //    //    now = AudioSettings.dspTime;
    //    //    float backWindow = 2f * bufferWindow;
    //    //    next = now + backWindow;
    //    //    while (AudioSettings.dspTime < next) 
    //    //        yield return null;

    //    //    //yield return new WaitForSeconds(2f * bufferWindow);
    //    //    currentNote.Clear();
    //    //}

    //    //private void PeekNextNote()
    //    //{
    //    //    int notePos = SongManager._Instance.GetCurrentNote();
    //    //    //Debug.Log($"PeekNextNote: {notePos}");
    //    //    if (notePos > 0 && notePos == sixteenthNoteCount + 1)
    //    //    {
    //    //        StartCoroutine(HighlightNote());
    //    //    }
    //    //}
}

////[System.Serializable]
////public class Note
////{
////    public int pos;

////    public Note(int pos = 0)
////    {
////        this.pos = pos;
////    }
////}