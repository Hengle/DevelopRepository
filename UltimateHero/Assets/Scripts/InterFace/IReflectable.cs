using UnityEngine;

//反射するマン
public interface IReflectable
{
    void Reflect(GameObject gameObject, Collision collision);
}