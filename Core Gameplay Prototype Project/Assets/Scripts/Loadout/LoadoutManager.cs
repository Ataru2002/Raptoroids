using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class LoadoutManager : MonoBehaviour
{
    public static LoadoutManager Instance { get; private set; }

    static ItemData[] weaponsData;
    static ItemData[] raptoroidsData;

    [SerializeField] GameObject itemPrefab;
    [SerializeField] RectTransform raptoroidDisplayRow;
    [SerializeField] RectTransform weaponDisplayRow;

    [SerializeField] GameObject infoDisplayBox;
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemNickname;
    [SerializeField] TextMeshProUGUI itemCodename;
    [SerializeField] LocalizeStringEvent flavorText;
    ItemData selectedItem = null;

    const float rowHeight = 200f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        BroadcastMessage("UpdateEquipButton", SendMessageOptions.DontRequireReceiver);
        BroadcastMessage("UpdateDisplayButton", SendMessageOptions.DontRequireReceiver);
    }

    void Start()
    {
        int raptoroidCount = 0;
        foreach (ItemData raptoroidData in raptoroidsData)
        {
            GameObject itemEntry = Instantiate(itemPrefab);
            itemEntry.transform.SetParent(raptoroidDisplayRow);
            itemEntry.transform.localScale = Vector3.one;
            itemEntry.GetComponent<LoadoutItemControl>().AssociateItemData(raptoroidData);
            raptoroidCount++;
        }
        raptoroidDisplayRow.sizeDelta = new Vector2(raptoroidCount * 125 - 5, rowHeight);

        int weaponCount = 0;
        foreach (ItemData weaponData in weaponsData)
        {
            GameObject itemEntry = Instantiate(itemPrefab);
            itemEntry.transform.SetParent(weaponDisplayRow);
            itemEntry.transform.localScale = Vector3.one;
            itemEntry.GetComponent<LoadoutItemControl>().AssociateItemData(weaponData);
            weaponCount++;
        }
        weaponDisplayRow.sizeDelta = new Vector2(weaponCount * 125 - 5, rowHeight);

        if (!PlayerPrefs.HasKey("EquippedRaptoroid"))
        {
            PlayerPrefs.SetInt("EquippedRaptoroid", 0);
        }

        if (PlayerPrefs.HasKey("EquippedWeapon"))
        {
            PlayerPrefs.SetInt("EquippedWeapon", 0);
        }

        BroadcastMessage("UpdateEquipButton", SendMessageOptions.DontRequireReceiver);
    }

    public static void LoadItems()
    {
        List<ItemData> weaponsList = new List<ItemData>(Resources.LoadAll<ItemData>("Scriptable Objects/Items/Basic/Weapon"));
        weaponsList.AddRange(ShopManager.WeaponsData);
        weaponsData = weaponsList.ToArray();

        List<ItemData> raptoroidsList = new List<ItemData>(Resources.LoadAll<ItemData>("Scriptable Objects/Items/Basic/Raptoroid"));
        raptoroidsList.AddRange(ShopManager.RaptoroidsData);
        raptoroidsData = raptoroidsList.ToArray();
    }

    public static int GetItemCount(ItemType type)
    {
        switch (type)
        {
            case ItemType.Weapon:
                return weaponsData.Length;
            case ItemType.Raptoroid:
                return raptoroidsData.Length;
            default:
                Debug.LogError("Got unrecognized ItemType for GetItemCount");
                return -1;
        }
    }

    public void DisplayItemInfo(ItemData item)
    {
        infoDisplayBox.SetActive(true);

        itemCodename.text = item.itemCodename;
        itemNickname.text = item.itemNickname;
        itemImage.sprite = item.itemSprite;

        flavorText.SetTable($"{item.itemType}ShopFlavorText");
        flavorText.SetEntry(item.flavorText);

        selectedItem = item;
    }

    public void HideItemInfo()
    {
        infoDisplayBox.SetActive(false);
    }

    public void EquipDisplayedItem()
    {
        GameManager.Instance.EquipItem(selectedItem.itemType, selectedItem.itemNumber);
        BroadcastMessage("UpdateEquipButton", SendMessageOptions.DontRequireReceiver);
        HideItemInfo();
    }

    public void EquipItem(ItemData item)
    {
        GameManager.Instance.EquipItem(item.itemType, item.itemNumber);
        BroadcastMessage("UpdateEquipButton", SendMessageOptions.DontRequireReceiver);
    }
}
