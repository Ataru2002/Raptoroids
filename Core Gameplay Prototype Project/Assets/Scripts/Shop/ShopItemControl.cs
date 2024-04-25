using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemControl : MonoBehaviour
{
    ItemData itemData;

    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemCodenameLabel;

    [SerializeField] Button purchaseButton;
    [SerializeField] TextMeshProUGUI purchasabilityLabel;
    [SerializeField] TextMeshProUGUI itemPriceText;

    public void DisplayItemInfo()
    {
        ShopManager.Instance.DisplayItemInfo(itemData);
    }

    public void AssociateItemData(ItemData val)
    {
        itemData = val;
        itemImage.sprite = itemData.itemSprite;
        itemCodenameLabel.text = val.itemCodename;
        itemPriceText.text = val.gemCost.ToString();
        
        UpdatePurchaseButton();
    }

    public void Purchase()
    {
        GameManager.Instance.PurchaseItem(itemData);
        UpdatePurchaseButton();
        ShopManager.Instance.UpdateGemCount();
    }

    public void UpdatePurchaseButton()
    {
        bool alreadyOwned = GameManager.Instance.ItemUnlocked(itemData.itemType, itemData.itemNumber);
        bool hasEnoughGems = GameManager.Instance.GetTotalGems() >= itemData.gemCost;
        purchaseButton.interactable = !alreadyOwned && hasEnoughGems;
        purchasabilityLabel.text = alreadyOwned ? "Already owned" : hasEnoughGems ? "Get Now" : "Not enough Gems";
    }
}
