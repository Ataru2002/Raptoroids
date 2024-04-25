using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutItemControl : MonoBehaviour
{
    ItemData itemData;

    [SerializeField] Image itemImage;

    [SerializeField] Button equipButton;
    [SerializeField] TextMeshProUGUI equipButtonText;

    public void DisplayItemInfo()
    {
        LoadoutManager.Instance.DisplayItemInfo(itemData);
    }

    public void AssociateItemData(ItemData val)
    {
        itemData = val;
        itemImage.sprite = itemData.itemSprite;
    }

    public void EquipItem()
    {
        LoadoutManager.Instance.EquipItem(itemData);
    }

    public void UpdateEquipButton()
    {
        bool equipped = PlayerPrefs.GetInt($"Equipped{itemData.itemType}") == itemData.itemNumber;
        bool owned = GameManager.Instance.ItemUnlocked(itemData.itemType, itemData.itemNumber);
        equipButton.interactable = owned && !equipped;

        equipButtonText.text = !owned ? "Not owned" : equipped ? "Equipped" : "Equip";
    }
}
