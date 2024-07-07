using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponData weaponData;
    protected float effectiveFireRate;

    protected void Start()
    {
        if (weaponData == null)
        {
            return;
        }

        effectiveFireRate = weaponData.fireRate;
    }

    public abstract void TryShoot();

    public void SetWeaponData(WeaponData val)
    {
        weaponData = val;
        effectiveFireRate = weaponData.fireRate;
    }

    public void FireRateModify(float factor)
    {
        effectiveFireRate *= factor;
    }

    public void ResetFireRate()
    {
        effectiveFireRate = weaponData.fireRate;
    }
}
