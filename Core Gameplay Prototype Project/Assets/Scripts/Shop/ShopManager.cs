using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using NUnit.Framework.Interfaces;

public class ShopManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static ShopManager instance;
    public static ShopManager Instance { get { return instance; } }

    static ItemData[] weaponsData;
    static ItemData[] raptoroidsData;

    [SerializeField] LocalizeStringEvent gemCounterLocalizeEvent;

    [SerializeField] GameObject itemInfoDisplay;
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemCodenameText;
    [SerializeField] TextMeshProUGUI itemNicknameText;
    [SerializeField] TextMeshProUGUI flavorText;
    [SerializeField] TextMeshProUGUI purchasabilityLabel;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] Button purchaseButton;

    [SerializeField] GameObject itemPrefab;
    Dictionary<ItemType, List<GameObject>> shopPages;

    [SerializeField] Transform itemGridTransform;

    public ItemData selectedItem;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            shopPages = new Dictionary<ItemType, List<GameObject>>();

            weaponsData = Resources.LoadAll<ItemData>("Scriptable Objects/Items/Purchasable/Weapon");
            raptoroidsData = Resources.LoadAll<ItemData>("Scriptable Objects/Items/Purchasable/Raptoroid");
            
            shopPages[ItemType.Weapon] = new List<GameObject>();
            shopPages[ItemType.Raptoroid] = new List<GameObject>();
        }
    }

    void Start()
    {
        UpdateGemCount();
        SwitchPage("Raptoroid");
    }

    private void OnEnable()
    {
        UpdateGemCount();
    }

    public void UpdateGemCount()
    {
        gemCounterLocalizeEvent.StringReference.Add("currency", new IntVariable { Value = GameManager.Instance.GetTotalGems() });
        gemCounterLocalizeEvent.StringReference.RefreshString();
    }

    public void SwitchPage(string type)
    {
        ItemType pageType = Enum.Parse<ItemType>(type);
        if (shopPages[pageType].Count == 0)
        {
            ItemData[] itemArray = pageType == ItemType.Weapon ? weaponsData : raptoroidsData;
            foreach (ItemData item in itemArray)
            {
                GameObject itemEntry = Instantiate(itemPrefab);
                itemEntry.transform.SetParent(itemGridTransform);
                itemEntry.transform.localScale = Vector3.one;
                itemEntry.GetComponent<ShopItemControl>().AssociateItemData(item);
                shopPages[pageType].Add(itemEntry);
            }
        }

        foreach(GameObject item in shopPages[pageType])
        {
            item.SetActive(true);
        }

        List<GameObject> deactivatedPage = pageType == ItemType.Weapon ? shopPages[ItemType.Raptoroid] : shopPages[ItemType.Weapon];
        foreach(GameObject item in deactivatedPage)
        {
            item.SetActive(false);
        }

        BroadcastMessage("UpdatePurchaseButton", SendMessageOptions.DontRequireReceiver);
    }

    public void PurchaseSelectedItem()
    {
        GameManager.Instance.PurchaseItem(selectedItem);
        UpdateGemCount();

        if (itemInfoDisplay.activeInHierarchy)
        {
            itemInfoDisplay.SetActive(false);
        }

        BroadcastMessage("UpdatePurchaseButton", SendMessageOptions.DontRequireReceiver);
    }

    public void DisplayItemInfo(ItemData item)
    {
        itemInfoDisplay.SetActive(true);
        itemImage.sprite = item.itemSprite;
        itemCodenameText.text = item.itemCodename;
        itemNicknameText.text = item.itemNickname;
        flavorText.text = item.flavorText;
        priceText.text = item.gemCost.ToString();

        selectedItem = item;
        UpdatePurchaseButton();
    }

    public void HideItemInfo()
    {
        itemInfoDisplay.SetActive(false);
    }

    public void UpdatePurchaseButton()
    {
        if (selectedItem == null)
        {
            return;
        }

        bool alreadyOwned = GameManager.Instance.ItemUnlocked(selectedItem.itemType, selectedItem.itemNumber);
        bool hasEnoughGems = GameManager.Instance.GetTotalGems() >= selectedItem.gemCost;
        purchaseButton.interactable = !alreadyOwned && hasEnoughGems;
        purchasabilityLabel.text = alreadyOwned ? "Already owned" : hasEnoughGems ? "Get Now" : "Not enough Gems";
    }
}
