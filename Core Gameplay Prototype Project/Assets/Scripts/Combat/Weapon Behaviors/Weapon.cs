using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected bool isPlayer;
    [SerializeField] protected float fireRate;

    public abstract void TryShoot();

    public void SetFireRate(float val)
    {
        fireRate = val;
    }
}

public enum ShotType
{
    Single,
    Cone,
    Laser,
}
