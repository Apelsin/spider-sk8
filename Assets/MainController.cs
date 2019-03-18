using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A very basic scene controller
/// </summary>
public class MainController : MonoBehaviour, ISerializationCallbackReceiver
{
    public enum EState
    {
        Default,
        PlayGame
    }

    private EState _RuntimeCachedCurrentState;

    [SerializeField]
    private EState _CurrentState;

    [SerializeField]
    private string[] _PermanentScenes = new[] { "Globals" };

    public EState CurrentState
    {
        get { return _CurrentState; }
        set
        {
            if (value != _RuntimeCachedCurrentState)
            {
                if (OnChangeState(value))
                {
                    _RuntimeCachedCurrentState = value;
                    _CurrentState = value;
                }
            }
        }
    }

    [SerializeField]
    private string _LevelName = "Level 1";

    public string LevelName
    {
        get { return _LevelName; }
        protected set { _LevelName = value; }
    }

    protected IEnumerable<Scene> GetLoadedScenes(bool include_own_scene)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (include_own_scene || scene != gameObject.scene)
                yield return scene;
        }
    }

    public void ReplaceLoadedScenes(params string[] scenes_to_load)
    {
        var unload = GetLoadedScenes(false).ToArray();
        foreach (var scene in unload.Where(s => !_PermanentScenes.Contains(s.name)))
            SceneManager.UnloadSceneAsync(scene);
        foreach (var name in scenes_to_load.Except(_PermanentScenes))
            SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
    }

    private bool OnChangeState(EState next_state)
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            return true;
#endif
        switch (next_state)
        {
            default:
                ReplaceLoadedScenes("Main Menu");
                return true;

            case EState.PlayGame:
                ReplaceLoadedScenes(LevelName);
                return true;
        }
    }

    public bool LoadLevel(string level_name)
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            return true;
#endif
        LevelName = level_name;
        if (OnChangeState(EState.PlayGame))
        {
            _CurrentState = EState.PlayGame;
            _RuntimeCachedCurrentState = _CurrentState;
        }
        return true;
    }

    private void Start()
    {
        var unload = GetLoadedScenes(false).ToArray();
        foreach (var scene in unload)
            SceneManager.UnloadSceneAsync(scene);
        foreach (var name in _PermanentScenes)
            SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        OnChangeState(CurrentState);
        _RuntimeCachedCurrentState = _CurrentState;
    }

    public void OnBeforeSerialize()
    {
        // Round-trip property valication
        CurrentState = CurrentState;
    }

    public void OnAfterDeserialize()
    {
    }
}