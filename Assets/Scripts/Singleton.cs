using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;

    public static T GetInstance { get { return _instance ? _instance : GetInstanceObject(); } }

    protected virtual void Awake()
    {
        _instance = this as T;
    }

    private static T GetInstanceObject()
    {
        _instance = FindObjectOfType<T>().GetComponent<T>();
        return _instance;
    }
        
}