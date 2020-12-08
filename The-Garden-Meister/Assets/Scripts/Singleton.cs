
using UnityEngine;

public class Singleton<T> where T : class, new()
{
    static readonly T instance = new T();
    public static T Instance { get { return instance; } }

    protected Singleton()
    {
        Debug.Assert(instance == null);
    }

}