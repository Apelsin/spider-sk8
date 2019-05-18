using UnityEngine;

public class ArcadeMainController : MainControllerBase<ArcadeMainController.State>, IMainController
{
    public enum State
    {
        Hold = -2,
        Clear = -1,
        Default = 0,
        PlayGame,
    }

    private string _PendingLevelName = null;

    public void LoadLevel(string level_name)
    {
        Debug.Log("Load level " + level_name);
        _PendingLevelName = level_name;
        RequestChangeState(State.PlayGame);
        _PendingLevelName = null;
    }

    protected override void Initialize()
    {
        // TODO
        base.Initialize();
    }

    protected override bool OnChangeStateRequest(State next_state)
    {
        switch (next_state)
        {
            case State.Hold:
                return true;
            case State.Clear:
                ReplaceLoadedScenes();
                return true;
            default:
                ReplaceLoadedScenes("Arcade Main Menu");
                return true;

            case State.PlayGame:
                if (!string.IsNullOrWhiteSpace(_PendingLevelName))
                {
                    ReplaceLoadedScenes(_PendingLevelName);
                    return true;
                }
                return false;
        }
    }
}