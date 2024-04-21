using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected bool isPlayer;
    public abstract void TryShoot();
}

public enum ShotType
{
    Single,
    Cone,
    Laser,
}
