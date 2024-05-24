using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected bool isPlayer;
    [SerializeField] protected float fireRate;
    protected float effectiveFireRate;

    protected void Start()
    {
        effectiveFireRate = fireRate;
    }

    public abstract void TryShoot();

    public void SetFireRate(float val)
    {
        fireRate = val;
        effectiveFireRate = fireRate;
    }

    public void FireRateBoost(float factor)
    {
        effectiveFireRate *= factor;
    }

    public void ResetFireRate()
    {
        effectiveFireRate = fireRate;
    }
}

public enum ShotType
{
    Single,
    Cone,
    Laser,
}
