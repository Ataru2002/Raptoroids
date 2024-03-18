using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuRaptoroidControl : MonoBehaviour
{
    [SerializeField] Sprite[] raptoroidSprites;
    [SerializeField] SpriteRenderer raptoroidRenderer;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("EquippedRaptoroid"))
        {
            PlayerPrefs.SetInt("EquippedRaptoroid", 0);
        }

        RefreshRaptoroid();
    }

    private void OnEnable()
    {
        RefreshRaptoroid();
    }

    void RefreshRaptoroid()
    {
        int raptoroidID = PlayerPrefs.GetInt("EquippedRaptoroid");
        raptoroidRenderer.sprite = raptoroidSprites[raptoroidID];
    }
}
