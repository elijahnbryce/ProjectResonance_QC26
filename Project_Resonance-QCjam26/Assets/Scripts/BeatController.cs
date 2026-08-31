using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;

public class BeatController : MonoBehaviour
{
    //[SerializeField] private float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Intervals[] _intervals;
    [SerializeField] private EventReference eventReference;

    public FMODBeatTicker ticker;

    [Tooltip("How far ahead of 'now' to schedule playback, giving the audio " +
             "system time to prep the buffer without a hitch.")]
    public double scheduleAheadTime = 0.5;

    private void Start()
    {
        ticker = FMODBeatTicker._Instance;

        foreach (var interval in _intervals)
        {
            //ticker.OnSixteenthNote += interval.PlayOnInverval;
            ticker.AutoAssignNoteOccurence(interval.Trigger, (int)interval._note);
        }
        StartSong();
    }


    private void OnDisable()
    {
        foreach (var interval in _intervals)
        {
            FMODBeatTicker._Instance.OnSixteenthNote -= interval.PlayOnInverval;
        }
    }

    //public void StartSong()
    //{
    //    double songStartDspTime = AudioSettings.dspTime + scheduleAheadTime;

    //    _audioSource.PlayScheduled(songStartDspTime);
    //    ticker.StartTicker(songStartDspTime);
    //}

    public void StartSong()
    {
        ticker.StartTicker(eventReference);
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
    [SerializeField] private UnityEvent _trigger, _trigger2;
    private int _lastInterval;

    /*    public float GetIntervalLength(float bpm)
        {
            return 60f / (bpm * _steps);
        }*/

    public float GetInterval() => 16 / _note;
    public void Trigger()
    {
        if (!FMODBeatTicker._Instance.CheckNote())
            return;
        _trigger?.Invoke();
        _trigger2?.Invoke();
        //FlashOnBeat();
    }

    public void PlayOnInverval()
    {
        //if (FMODBeatTicker._Instance.sixteenthNoteCount % GetInterval() == 0) Trigger();
        Trigger();
    }

    //public void FlashOnBeat()
    //{
    //    if (FMODBeatTicker._Instance.CheckNote()) _trigger2?.Invoke();
    //}

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