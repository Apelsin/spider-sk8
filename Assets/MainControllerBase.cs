using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A very basic scene controller base class
/// </summary>
public abstract class MainControllerBase<TState> :
    MonoBehaviour, ISerializationCallbackReceiver where TState : Enum
{
    public class InvalidSceneException : Exception
    {
        public readonly string SceneName;

        public InvalidSceneException(string message, string scene_name) :
            base(message)
        {
            SceneName = scene_name;
        }
    }

    private TState _RuntimeCachedCurrentState;

    [SerializeField]
    private TState _CurrentState;

    [SerializeField]
    private string[] _PermanentScenes = new[] { "Globals" };

    private Queue<IEnumerator> _ActionQueue = new Queue<IEnumerator>();

    public IReadOnlyList<string> PermanentScenes
    {
        get { return _PermanentScenes; }
    }

    /// <summary>
    /// The current state of the controller
    /// </summary>
    public TState CurrentState
    {
        get { return _CurrentState; }
        private set
        {
            if (!value.Equals(_RuntimeCachedCurrentState))
            {
                RequestChangeState(value);
            }
        }
    }

    public bool RequestChangeState(TState next_state)
    {
        if (_OnChangeStateRequest(next_state))
        {
            _CurrentState = next_state;
            _RuntimeCachedCurrentState = next_state;
            return true;
        }
        else
        {
            // Rollback
            _CurrentState = _RuntimeCachedCurrentState;
            Debug.LogWarning($"Could not change states.\n{_CurrentState} => {next_state}");
            return false;
        }
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

    protected IEnumerable<string> AddScenes(params string[] request)
    {
        // Skip any scenes that are already loaded
        var scenes_to_load = request
            .Except(GetLoadedScenes(true).Select(s => s.name).ToArray());
        {
            var already_loaded = request
                .Except(scenes_to_load)
                .ToArray();
            if (already_loaded.Any())
                Debug.LogWarning(
                    "Attepted to load one or more already-loaded scenes.\n" +
                    string.Join(", ", already_loaded));
        }

        foreach (var name in scenes_to_load)
        {
            Debug.Log($"Load {name}");
            SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        }
        return scenes_to_load;
    }

    protected IEnumerable<Scene> RemoveOtherScenes(params string[] request)
    {
        var scenes_to_unload = GetLoadedScenes(false)
            .Where(s => !PermanentScenes.Contains(s.name) && !request.Contains(s.name))
            .ToArray();
        foreach (var scene in scenes_to_unload)
        {
            SceneManager.UnloadSceneAsync(scene);
            Debug.Log($"Unload {scene.name}");
        }
        return scenes_to_unload;
    }

    protected IEnumerator ReplaceLoadedScenesCoroutine(params string[] request)
    {
        // Add the requested scenes
        var scenes_to_load = AddScenes(request);

        // While the scenes have not yet been loaded, yield every frame
        while (scenes_to_load.Except(GetLoadedScenes(false).Select(s => s.name)).Any())
            yield return new WaitForEndOfFrame();

        // Unload other scenes
        var scenes_to_unload = RemoveOtherScenes(request);

        // While the scenes have not yet been unloaded, yield every frame
        while (scenes_to_unload.Intersect(GetLoadedScenes(false)).Any())
            yield return new WaitForEndOfFrame();
    }

    public void ReplaceLoadedScenes(params string[] scenes_to_load)
    {
        foreach (var name in scenes_to_load)
        {
            Debug.Log($"Verify {name}");
            if (!Application.CanStreamedLevelBeLoaded(name))
                throw new InvalidSceneException("Unable to find scene by name.", name);
        }

        _ActionQueue.Enqueue(ReplaceLoadedScenesCoroutine(scenes_to_load));
    }

    protected abstract bool OnChangeStateRequest(TState next_state);

    private bool _OnChangeStateRequest(TState next_state)
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            return true;
#endif
        return OnChangeStateRequest(next_state);
    }

    private IEnumerator Start()
    {
        // Unload all scenes (except for this scene)
        var scenes_to_unload = GetLoadedScenes(false).ToArray();
        foreach (var scene in scenes_to_unload)
            SceneManager.UnloadSceneAsync(scene);
        // Wait for all scenes to unload
        while (GetLoadedScenes(false).Any())
            yield return new WaitForEndOfFrame();
        // Then add permanent scenes
        AddScenes(PermanentScenes.ToArray());
        // Then load scenes as necessary
        _OnChangeStateRequest(CurrentState);
        // Initialize the cached current state
        _RuntimeCachedCurrentState = _CurrentState;
        // Begin action queue loop
        for (; ; )
        {
            while (_ActionQueue.Count > 0)
            {
                var r = _ActionQueue.Dequeue();
                for (; ; )
                {
                    try
                    {
                        if (!r.MoveNext())
                            break;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                        break;
                    }
                    yield return r.Current;
                }
            }
            yield return null;
        }
    }

    public void OnBeforeSerialize()
    {
        // Round-trip property validation
        CurrentState = CurrentState;
    }

    public void OnAfterDeserialize()
    {
    }
}