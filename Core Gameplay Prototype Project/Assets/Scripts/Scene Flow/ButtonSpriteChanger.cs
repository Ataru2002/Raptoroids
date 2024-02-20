using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSpriteChanger : MonoBehaviour
{
    static Sprite[,] buttonSprites;
    [SerializeField] Image[] buttonImages;

    void Awake()
    {
        if (buttonSprites == null)
        {
            buttonSprites = new Sprite[3, 3];

            Sprite[] grayButtons = Resources.LoadAll<Sprite>("Button Sprites/Gray Buttons");
            Sprite[] cyanButtons = Resources.LoadAll<Sprite>("Button Sprites/Cyan Buttons");
            Sprite[] orangeButtons = Resources.LoadAll<Sprite>("Button Sprites/Orange Buttons");

            for (int i = 0; i < 3; i++)
            {
                buttonSprites[0, i] = grayButtons[i];
                buttonSprites[1, i] = cyanButtons[i];
                buttonSprites[2, i] = orangeButtons[i];
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("btnColorIndex"))
        {
            PlayerPrefs.SetInt("btnColorIndex", 0);
        }

        if (!PlayerPrefs.HasKey("btnSizeIndex"))
        {
            PlayerPrefs.SetInt("btnSizeIndex", 0);
        }

        SetButtonSprite();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetButtonSprite()
    {
        int buttonColor = PlayerPrefs.GetInt("btnColorIndex");
        int buttonSizeIndex = PlayerPrefs.GetInt("btnSizeIndex");

        for (int i = 0; i < buttonImages.Length; i++)
        {
            buttonImages[i].sprite = buttonSprites[buttonColor, buttonSizeIndex];
            buttonImages[i].type = Image.Type.Tiled;
        }
    }

    public void SetSpriteColor(int colorIndex)
    {
        PlayerPrefs.SetInt("btnColorIndex", colorIndex);
        SetButtonSprite();
    }

    public void SetSpriteSize(int sizeIndex)
    {
        PlayerPrefs.SetInt("btnSizeIndex", sizeIndex);
        SetButtonSprite();
    }
}
