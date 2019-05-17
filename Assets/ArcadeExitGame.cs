using UnityEngine;

/// <summary>
/// Helper script to allow the game to be exited on the arcade cabinet
/// </summary>
public class ArcadeExitGame : MonoBehaviour
{
    [SerializeField]
    private string _Player1Button = "Player 1";

    /// <summary>
    /// Input name for the Player 1 button
    /// </summary>
    public string Player1Button
    {
        get { return _Player1Button; }
        set { _Player1Button = value; }
    }

    [SerializeField]
    private string _Player2Button = "Player 2";

    /// <summary>
    /// Input name for the Player 2 button
    /// </summary>
    public string Player2Button
    {
        get { return _Player2Button; }
        set { _Player2Button = value; }
    }

    void Update()
    {
        var exit1 = Input.GetButton(Player1Button) && Input.GetButtonDown(Player2Button);
        var exit2 = Input.GetButtonDown(Player1Button) && Input.GetButton(Player2Button);
        if (exit1 || exit2)
            Application.Quit();
    }
}
