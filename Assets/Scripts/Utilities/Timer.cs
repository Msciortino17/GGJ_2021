using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    /// <summary>
    /// The overall time
    /// </summary>
    public float Seconds => time_sec;

    /// <summary>
    /// Call this once per frame for the timer
    /// The timer increments by Time.deltaTime
    /// </summary>
    public void Interval() {
        time_sec += Time.deltaTime;
    }

    /// <summary>
    /// Call this once per frame for the timer
    /// The timer decrements by Time.deltaTime
    /// </summary>
    public void IntervalDown() {
        time_sec -= Time.deltaTime;
    }

    /// <summary>
    /// Overrides the timer to a certain value
    /// </summary>
    public void SetTime(float time_sec) {
        this.time_sec = time_sec;
    }


    /// <summary>
    /// Resets the timer to the reset point, default is 0
    /// </summary>
    public void Reset() {
        time_sec = reset_sec;
    }

    private float reset_sec;
    private float time_sec;
}
