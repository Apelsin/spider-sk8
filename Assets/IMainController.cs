using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IMainController
{
    IReadOnlyList<string> PermanentScenes { get; }
    
    void LoadLevel(string level_name);
    
    bool RequestChangeState(string next_state_str);
}