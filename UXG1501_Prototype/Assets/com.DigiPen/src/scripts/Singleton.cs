using System.Reflection;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public bool m_AutoUnparentOnAwake = true;

    protected static  T m_Instance;

    public static bool HasInstance => m_Instance != null;
    public static T TryGetInstance() => HasInstance ? m_Instance : null;

    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindAnyObjectByType<T>();
                if (m_Instance == null)
                {
                    var obj = new GameObject(typeof(T).Name + " Auto-Generated");
                    m_Instance = obj.AddComponent<T>();
                }
            }
            return m_Instance;
        }
    }

    protected virtual void Awake()
    {
        InitialiseSingleton();
    }

    protected virtual void Start()
    {
       
    }

    protected virtual void Init()
    {

    }

    protected virtual void InitialiseSingleton()
    {
        if (!Application.isPlaying) return;

        if (m_AutoUnparentOnAwake)
            transform.SetParent(null);

        if (m_Instance == null)
        {
            m_Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
            if (m_Instance != this) 
                Destroy(gameObject);
    }
}
