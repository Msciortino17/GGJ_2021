using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeMusic : MonoBehaviour
{
    public AudioSource[] musicSources;
    private double bpm = 67.7;
    private int timeSignature = 4;
    private int barsLength = 32;

    private double loopPointSeconds;
    private double time;
    private int nextSource;

    // Start is called before the first frame update
    void Start()
    {
        loopPointSeconds = (barsLength * timeSignature) / bpm * 60;
        time = AudioSettings.dspTime;
        musicSources[0].Play();
        nextSource = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!musicSources[nextSource].isPlaying)
        {
            time += loopPointSeconds;
            musicSources[nextSource].PlayScheduled(time);
            nextSource = 1 - nextSource;
        }
    }
}
