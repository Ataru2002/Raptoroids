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
        RefreshRaptoroid();
    }

    private void OnEnable()
    {
        RefreshRaptoroid();
    }

    void RefreshRaptoroid()
    {
        int raptoroidID = GameManager.Instance.EquippedRaptoroid;
        raptoroidRenderer.sprite = raptoroidSprites[raptoroidID];
    }
}
