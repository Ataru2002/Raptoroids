using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    [SerializeField] Sprite slowIcon;
    [SerializeField] Sprite mediumIcon;
    [SerializeField] Sprite fastIcon;

    SpriteRenderer render;

    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        ChangeIcon(SpeedTier.Medium);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeIcon(string speedTier)
    {
        ChangeIcon((SpeedTier)Enum.Parse(typeof(SpeedTier), speedTier));
    }

    public void ChangeIcon(SpeedTier tier)
    {
        switch (tier)
        {
            case SpeedTier.Slow:
                render.sprite = slowIcon;
                break;
            case SpeedTier.Medium:
                render.sprite = mediumIcon;
                break;
            case SpeedTier.Fast:
                render.sprite = fastIcon;
                break;
            default:
                print("Unrecognized speed tier argument");
                break;
        }
    }
}
