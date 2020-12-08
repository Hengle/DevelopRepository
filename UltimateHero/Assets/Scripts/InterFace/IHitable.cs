using UnityEngine;


public interface IHittable
{
    void Hit();
    bool IsKillable();
}
