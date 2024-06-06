using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class LoadoutItemControl : MonoBehaviour
{
    ItemData itemData;

    [SerializeField] Image itemImage;

    [SerializeField] Button displayInfoButton;
    [SerializeField] Button equipButton;
    [SerializeField] LocalizeStringEvent equipButtonTextLocalizeEvent;

    void OnEnable()
    {
        
    }

    public void DisplayItemInfo()
    {
        LoadoutManager.Instance.DisplayItemInfo(itemData);
    }

    public void AssociateItemData(ItemData val)
    {
        itemData = val;
        itemImage.sprite = itemData.itemSprite;
        UpdateDisplayButton();
    }

    public void EquipItem()
    {
        LoadoutManager.Instance.EquipItem(itemData);
    }

    public void UpdateEquipButton()
    {
        bool equipped = false;
        if (itemData.itemType == ItemType.Raptoroid)
        {
            equipped = itemData.itemNumber == GameManager.Instance.EquippedRaptoroid;
        }
        else if (itemData.itemType == ItemType.Weapon)
        {
            equipped = itemData.itemNumber == GameManager.Instance.EquippedWeapon;
        }

        bool owned = GameManager.Instance.ItemUnlocked(itemData.itemType, itemData.itemNumber);
        equipButton.interactable = owned && !equipped;

        equipButtonTextLocalizeEvent.SetEntry(!owned ? "NotOwned" : equipped ? "Equipped" : "Equip");
    }

    public void UpdateDisplayButton()
    {
        if (itemData == null)
        {
            return;
        }

        displayInfoButton.interactable = GameManager.Instance.ItemUnlocked(itemData.itemType, itemData.itemNumber);
    }
}
