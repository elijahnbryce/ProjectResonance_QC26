using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatController : MonoBehaviour
{
    //[SerializeField] private float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Intervals[] _intervals;

    public SampleTicker ticker;

    [Tooltip("How far ahead of 'now' to schedule playback, giving the audio " +
             "system time to prep the buffer without a hitch.")]
    public double scheduleAheadTime = 0.5;

    private void Start()
    {
        ticker = SampleTicker._Instance;
        StartSong();
    }

    private void OnEnable()
    {
        foreach (var interval in _intervals)
        {
            SampleTicker._Instance.OnSixteenthNote += interval.PlayOnInverval;
        }
    }

    private void OnDisable()
    {
        foreach(var interval in _intervals)
            SampleTicker._Instance.OnSixteenthNote -= interval.PlayOnInverval;
    }

    public void StartSong()
    {
        double songStartDspTime = AudioSettings.dspTime + scheduleAheadTime;

        _audioSource.PlayScheduled(songStartDspTime);
        ticker.StartTicker(songStartDspTime);
    }

    //private void Update()
    //{
    //    foreach (var interval in _intervals)
    //    {
    //        float sampledTime = (_audioSource.timeSamples / (_audioSource.clip.frequency * interval.GetIntervalLength(_bpm)));
    //        interval.CheckForNewInterval(sampledTime);
    //    }
    //}
}


[System.Serializable]
public class Intervals
{
    [SerializeField] public float _steps, _note;
    [SerializeField] private UnityEvent _trigger;
    private int _lastInterval;

    /*    public float GetIntervalLength(float bpm)
        {
            return 60f / (bpm * _steps);
        }*/

    public float GetInterval() => 16f / _note;
    public void Trigger()
    {
        _trigger?.Invoke();
    }

    public void PlayOnInverval()
    {
        if (SampleTicker._Instance.sixteenthNoteCount % GetInterval() == 0) Trigger();
    }

    //public void CheckForNewInterval (float interval)
    //{
    //    int interval_int = Mathf.FloorToInt(interval);
    //    if (interval_int != _lastInterval)
    //    {
    //        _lastInterval = interval_int;
    //        _trigger.Invoke();
    //    }
    //}
}