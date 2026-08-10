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
    public static SampleTicker _Instance;

    [Header("Song Timing (static BPM)")]
    public float bpm = 120f;

    public event Action OnSixteenthNote;
    public event Action OnQuarterNote;

    private double songStartDspTime;
    private double secondsPerSixteenthNote;
    private double secondsPerQuarterNote;

    // How many of each subdivision have fired so far. The threshold time
    // for the *next* one is always recomputed as count * unit, from the
    // fixed songStartDspTime origin -- never by adding onto a running total.
    public int sixteenthNoteCount;
    public int quarterNoteCount;

    private bool isRunning;

    private void Awake()
    {
        if (_Instance == null)
            _Instance = this;
        else
            Destroy(gameObject);
    }

    // Call with the exact same dspTime value passed to AudioSource.PlayScheduled,
    // so the ticker's origin matches what the player actually hears.
    public void StartTicker(double songStartDspTime)
    {
        this.songStartDspTime = songStartDspTime;
        secondsPerQuarterNote = 60.0 / bpm;
        secondsPerSixteenthNote = secondsPerQuarterNote / 4.0;

        sixteenthNoteCount = 0;
        quarterNoteCount = 0;
        isRunning = true;
    }

    public void Stop() => isRunning = false;

    private void Update()
    {
        if (!isRunning) return;

        double now = AudioSettings.dspTime;

        while (songStartDspTime + sixteenthNoteCount * secondsPerSixteenthNote <= now)
        {
            sixteenthNoteCount++;
            OnSixteenthNote?.Invoke();
        }

        while (songStartDspTime + quarterNoteCount * secondsPerQuarterNote <= now)
        {
            quarterNoteCount++;
            OnQuarterNote?.Invoke();
        }
    }
}

