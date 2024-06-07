using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class BGMPlayer : MonoBehaviour
{
    public AudioClip[] loopClips;
    public bool looping = false;

    double nextEventTime;
    int index = 0;

    private AudioSource introSource;
    private AudioSource[] loopSources;

    public UnityEvent onLoopEnd;

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

        UpdateVolume();

        introSource.Play();
        nextEventTime = AudioSettings.dspTime + introSource.clip.length;
        looping = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!looping)
        {
            return;
        }

        double time = AudioSettings.dspTime;
        
        // Allow some time for the audio to buffer
        if (time >= nextEventTime - 1f)
        {
            if (index == loopClips.Length)
            {
                onLoopEnd.Invoke();
            }

            if (!looping)
            {
                return;
            }

            index = index % loopClips.Length;

            loopSources[index].clip = loopClips[index];
            loopSources[index].PlayScheduled(nextEventTime);
            nextEventTime += loopClips[index].length;

            index++;
        }
    }

    public void UpdateVolume()
    {
        if (!PlayerPrefs.HasKey("bgmOn"))
        {
            PlayerPrefs.SetInt("bgmOn", 1);
        }

        if (!PlayerPrefs.HasKey("bgmVol"))
        {
            PlayerPrefs.SetFloat("bgmVol", 1);
        }

        float volume = PlayerPrefs.GetInt("bgmOn") * PlayerPrefs.GetFloat("bgmVol");

        introSource.volume = volume;

        foreach (AudioSource source in loopSources)
        {
            source.volume = volume;
        }
    }
}
