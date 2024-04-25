using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon Data")]
public class WeaponData : ScriptableObject
{
    public ShotType shotType;
    public float fireRate;
    public int projectileCount;
    public float coneAngle;
}
