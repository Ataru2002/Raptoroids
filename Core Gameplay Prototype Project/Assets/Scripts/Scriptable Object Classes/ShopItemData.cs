using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item Data", menuName = "Shop Item Data")]
public class ShopItemData : ScriptableObject
{
    public ItemType itemType;
    public int itemNumber;
    
    public string itemCodename;

    // TODO: drop this - use localized nickname
    public string itemNickname;

    public Sprite itemSprite;
    public int gemCost;

    // TODO: switch to localization table entry instead
    public string flavorText;
}

public enum ItemType
{
    Raptoroid,
    Weapon
}
