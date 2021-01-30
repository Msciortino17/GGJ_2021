using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public float Seconds => time_sec;

    public void Interval() {
        time_sec += Time.deltaTime;
    }

    public void Reset() {
        time_sec = 0;
    }

    private float time_sec;
}
