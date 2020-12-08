public enum ObjectType
{
    Player,
    Enemy,
    Gimmick
}

public interface IAnimationRegistable
{
    void Register(IAnimationMonitorable monitorable);
    void Unregister(IAnimationMonitorable monitorable);
}
