public interface IShootable
{
    public void TryShoot();
}

public enum ShotType
{
    Single,
    Cone,
    Laser,
}
