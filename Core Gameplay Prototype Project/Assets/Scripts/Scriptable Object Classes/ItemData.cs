using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Item Data")]
public class ItemData : ScriptableObject
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
