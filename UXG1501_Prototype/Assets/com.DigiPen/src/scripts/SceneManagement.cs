using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneManagement : Singleton<SceneManagement>
{
    public UnityEvent onSceneLoaded;
    public UnityEvent onSceneUnloaded;
    public UnityEvent<float> onSceneLoadProgress;
    public UnityEvent<float> onSceneUnloadProgress;
    private string m_SceneToLoad = "";
    private string m_SceneToUnload = "";

    protected override void Start()
    {
        base.Start();
        Logger.Log("Initialised " + MethodBase.GetCurrentMethod().ReflectedType.Name,
            Logger.SEVERITY_LEVEL.INFO,
            Logger.LOGGER_OPTIONS.SIMPLE);
    }

    private void Update()
    {
        if (m_SceneToLoad != "")
        {
            StartCoroutine(LoadSceneAsync(m_SceneToLoad));
            m_SceneToLoad = "";
        }

        if (m_SceneToUnload != "")
        {
            StartCoroutine(UnloadSceneAsync(m_SceneToUnload));
            m_SceneToUnload = "";
        }
    }

    public void PushLoadScene(string path) => m_SceneToLoad = path;
    public void PushUnloadScene(string path) => m_SceneToUnload = path;

    public bool SetActiveScene(string path)
    {
        Logger.Log("Active scene set from path: " + path,
            Logger.SEVERITY_LEVEL.INFO,
            Logger.LOGGER_OPTIONS.VERBOSE,
            MethodBase.GetCurrentMethod());

        string curPath = SceneManager.GetActiveScene().path;
        if (SceneManager.SetActiveScene(SceneManager.GetSceneByPath(path)))
        {
            m_SceneToUnload = curPath;
            return true;
        }
        else
            return false;
    }

    public void UnloadAllScenes(string exceptPath = "")
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != exceptPath)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    }

    public void ReloadScene()
    {
        Logger.Log("Reloading Current Scene",
            Logger.SEVERITY_LEVEL.INFO,
            Logger.LOGGER_OPTIONS.VERBOSE,
            MethodBase.GetCurrentMethod());
        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }

    private IEnumerator LoadSceneAsync(string path)
    {
        Logger.Log("Loading Scene from path: " + path,
            Logger.SEVERITY_LEVEL.INFO,
            Logger.LOGGER_OPTIONS.VERBOSE,
            MethodBase.GetCurrentMethod());

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive);
        while (!asyncOperation.isDone) 
        {
            onSceneLoadProgress?.Invoke(asyncOperation.progress);
            yield return null;
        }
        onSceneLoaded?.Invoke();

        Logger.Log("Scene loaded from path: " + path,
            Logger.SEVERITY_LEVEL.INFO,
            Logger.LOGGER_OPTIONS.VERBOSE,
            MethodBase.GetCurrentMethod());
    }

    private IEnumerator UnloadSceneAsync(string path)
    {
        Logger.Log("Unloading Scene from path: " + path,
            Logger.SEVERITY_LEVEL.INFO,
            Logger.LOGGER_OPTIONS.VERBOSE,
            MethodBase.GetCurrentMethod());

        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(path);
        while (!asyncOperation.isDone) 
        {
            onSceneUnloadProgress?.Invoke(asyncOperation.progress);
            yield return null;
        }
        onSceneUnloaded?.Invoke();

        Logger.Log("Scene unloaded from path: " + path,
            Logger.SEVERITY_LEVEL.INFO,
            Logger.LOGGER_OPTIONS.VERBOSE,
            MethodBase.GetCurrentMethod());
    }
}
