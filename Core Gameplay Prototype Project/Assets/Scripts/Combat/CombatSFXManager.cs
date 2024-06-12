using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSFXManager : MonoBehaviour
{
    static Dictionary<string, AudioClip> combatFXDict = new Dictionary<string, AudioClip>();

    public static void LoadCombatSFX()
    {
        AudioClip[] fxClips = Resources.LoadAll<AudioClip>("SFX/Combat");
        foreach (AudioClip fxClip in fxClips)
        {
            combatFXDict[fxClip.name] = fxClip;
        }
    }

    public static void PlaySoundAtLocation(string name, Vector3 location)
    {
        float volume = PlayerPrefs.GetInt("sfxOn") * PlayerPrefs.GetFloat("sfxVol");

        GameObject tempObj = new GameObject($"sfxSource {name}");
        tempObj.transform.position = location;
        AudioSource tempSrc = tempObj.AddComponent<AudioSource>();
        tempSrc.volume = volume;
        tempSrc.clip = combatFXDict[name];
        tempSrc.Play();
        Destroy(tempObj, tempSrc.clip.length);
    }
}
