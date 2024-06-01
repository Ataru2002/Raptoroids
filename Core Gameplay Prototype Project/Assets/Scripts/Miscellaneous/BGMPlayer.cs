using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMPlayer : MonoBehaviour
{
    public AudioClip[] loopClips;

    double nextEventTime;
    int index = 0;

    private AudioSource introSource;
    private AudioSource[] loopSources;

    // Start is called before the first frame update
    void Start()
    {
        loopSources = new AudioSource[loopClips.Length];
        for (int i = 0; i < loopClips.Length; i++)
        {
            GameObject loopSourceObject = new GameObject("BGM Clip Source");
            loopSourceObject.transform.parent = transform;
            loopSources[i] = loopSourceObject.AddComponent<AudioSource>();
        }

        introSource = GetComponent<AudioSource>();
        introSource.Play();
        nextEventTime = AudioSettings.dspTime + introSource.clip.length;
    }

    // Update is called once per frame
    void Update()
    {
        double time = AudioSettings.dspTime;
        
        // Allow some time for the audio to buffer
        if (time >= nextEventTime - 3f)
        {
            loopSources[index].clip = loopClips[index];
            loopSources[index].PlayScheduled(nextEventTime);

            index = (index + 1) % loopClips.Length;
            nextEventTime += loopClips[index].length;
        }
    }
}
