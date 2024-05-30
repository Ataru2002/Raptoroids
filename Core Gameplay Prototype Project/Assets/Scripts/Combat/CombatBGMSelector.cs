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

    AudioSource introSource;
    BGMPlayer bgmPlayer;

    private void Awake()
    {
        introSource = GetComponent<AudioSource>();
        bgmPlayer = GetComponent<BGMPlayer>();

        bool isBossStage = GameManager.Instance.MapTier == 4;
        introSource.clip = isBossStage ? bossIntro : normalIntro;
        bgmPlayer.loopClips = isBossStage ? bossSequence : normalSequence;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
