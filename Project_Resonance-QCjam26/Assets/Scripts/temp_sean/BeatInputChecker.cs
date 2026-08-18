using System;
using UnityEngine;

public class BeatInputChecker
{
    private static SampleTicker ticker = GameObject.Find("BeatMapper").GetComponent<SampleTicker>();
    private static int comboCounter = 0;

    public static void checkInput() {
        if (ticker.CheckNote()) {
            comboCounter++;
            //ticker.currentNote[0].hit(comboCounter);
            Debug.Log("NOTE HIT");
        } else {
            comboCounter = 0;
            Debug.Log("NOTE NOT HIT");
        }
    }
}