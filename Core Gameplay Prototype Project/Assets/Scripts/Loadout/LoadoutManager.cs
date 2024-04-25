using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class LoadoutManager : MonoBehaviour
{
    public static LoadoutManager Instance { get; private set; }

    static ItemData[] weaponsData;
    static ItemData[] raptoroidsData;

    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform raptoroidDisplayRow;
    [SerializeField] Transform weaponDisplayRow;

    private void Awake()
    {
        if (weaponsData == null)
        {
            List<ItemData> weaponsList = new List<ItemData>(Resources.LoadAll<ItemData>("Scriptable Objects/Items/Basic/Weapon"));
            weaponsList.AddRange(Resources.LoadAll<ItemData>("Scriptable Objects/Items/Purchasable/Weapon"));
            weaponsData = weaponsList.ToArray();
        }

        if (raptoroidsData == null)
        {
            List<ItemData> raptoroidsList = new List<ItemData>(Resources.LoadAll<ItemData>("Scriptable Objects/Items/Basic/Raptoroid"));
            raptoroidsList.AddRange(Resources.LoadAll<ItemData>("Scriptable Objects/Items/Purchasable/Raptoroid"));
            raptoroidsData = raptoroidsList.ToArray();
        }

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
    }

    void Start()
    {
        foreach (ItemData raptoroidData in raptoroidsData)
        {
            GameObject itemEntry = Instantiate(itemPrefab);
            itemEntry.transform.SetParent(raptoroidDisplayRow);
            itemEntry.transform.localScale = Vector3.one;
            itemEntry.GetComponent<LoadoutItemControl>().AssociateItemData(raptoroidData);
        }

        foreach (ItemData weaponData in weaponsData)
        {
            GameObject itemEntry = Instantiate(itemPrefab);
            itemEntry.transform.SetParent(weaponDisplayRow);
            itemEntry.transform.localScale = Vector3.one;
            itemEntry.GetComponent<LoadoutItemControl>().AssociateItemData(weaponData);
        }

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

    public void DisplayItemInfo(ItemData item)
    {
        // TODO: show equipment info
    }

    public void EquipItem(ItemData item)
    {
        GameManager.Instance.EquipItem(item.itemType, item.itemNumber);
        BroadcastMessage("UpdateEquipButton", SendMessageOptions.DontRequireReceiver);
    }
}
