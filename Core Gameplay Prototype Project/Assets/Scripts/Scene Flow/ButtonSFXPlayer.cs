using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSFXPlayer : MonoBehaviour
{
    static ButtonSFXPlayer instance;
    public static ButtonSFXPlayer Instance { get { return instance; } }

    AudioSource audioSource;

    Dictionary<string, AudioClip> sfxLibrary;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        sfxLibrary = new Dictionary<string, AudioClip>();
    }

    public void PlaySFX(string key)
    {
        if (!sfxLibrary.ContainsKey(key))
        {
            sfxLibrary[key] = Resources.Load<AudioClip>("SFX/Scene Flow/" + key);
        }

        audioSource.clip = sfxLibrary[key];
        audioSource.Play();
    }

    public void UpdateVolume()
    {
        audioSource.volume = PlayerPrefs.GetInt("sfxOn") * PlayerPrefs.GetFloat("sfxVol");
    }
}
