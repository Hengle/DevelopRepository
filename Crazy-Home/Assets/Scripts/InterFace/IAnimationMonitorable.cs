using System;

public interface IAnimationMonitorable
{
    event Action<ObjectType> OnAnimeStart;
    event Action<ObjectType> OnAnimeEnd;
    void FindRegisterComponent();
}
