using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BGMPlayer))]
public class CombatBGMSelector : MonoBehaviour
{
    [SerializeField] AudioClip normalIntro;
    [SerializeField] AudioClip bossIntro;

    [SerializeField] AudioClip[] normalSequence;
    [SerializeField] AudioClip[] bossSequence;

    Dictionary<string, AudioClip> stageEndSounds = new Dictionary<string, AudioClip>();

    AudioSource introSource;
    BGMPlayer bgmPlayer;

    private void Awake()
    {
        introSource = GetComponent<AudioSource>();
        bgmPlayer = GetComponent<BGMPlayer>();

        bool isBossStage = GameManager.Instance.MapTier == 4;
        introSource.clip = isBossStage ? bossIntro : normalIntro;
        bgmPlayer.loopClips = isBossStage ? bossSequence : normalSequence;

        stageEndSounds["bossWin"] = Resources.Load<AudioClip>("Music/Stage End/BossWin");
        stageEndSounds["win"] = Resources.Load<AudioClip>("Music/Stage End/Win");
        stageEndSounds["lose"] = Resources.Load<AudioClip>("Music/Stage End/Lose");
    }

    public void PlayEndMusic(string trackID)
    {
        AudioSource[] bgmSources = GetComponentsInChildren<AudioSource>();
        foreach (AudioSource source in bgmSources)
        {
            source.Stop();
        }

        bgmPlayer.looping = false;

        if (!stageEndSounds.ContainsKey(trackID))
        {
            Debug.LogWarning("provided Track ID not recognized");
            return;
        }

        introSource.clip = stageEndSounds[trackID];
        introSource.Play();
    }
}
