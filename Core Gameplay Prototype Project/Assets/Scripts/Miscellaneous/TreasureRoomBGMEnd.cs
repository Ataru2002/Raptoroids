using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

[RequireComponent(typeof(AudioSource))]
public class TreasureRoomBGMEnd : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip endClip;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayEnding()
    {
        AudioSource[] bgmSources = GetComponentsInChildren<AudioSource>();
        foreach (AudioSource source in bgmSources)
        {
            source.Stop();
        }

        GetComponent<BGMPlayer>().looping = false;

        audioSource.clip = endClip;
        audioSource.Play();
    }
}
