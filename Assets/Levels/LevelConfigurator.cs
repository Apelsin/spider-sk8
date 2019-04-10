using System.Collections;
using UnityEngine;

public class LevelConfigurator : MonoBehaviour
{
    private void Start()
    {
        var camera = Camera.main;
        var camera_contstraints = GameObject.Find("Level Camera Constraints").GetComponent<RectTransform>();
        var spooder_object = GameObject.Find("Spooder");
        var spooder_controller = spooder_object.GetComponent<SpooderMotion>();
        var spooder_rb = spooder_object.GetComponent<Rigidbody2D>();
        var level_cam = camera.GetComponentInParent<LevelCamera2D>();
        var player_input = FindObjectOfType<PlayerInput>();
        spooder_controller.Input = player_input;
        level_cam.Constraints = camera_contstraints;
        level_cam.Target = spooder_rb;

        var input_events = GetComponent<InputEvents>();
        input_events.Input = player_input;
    }

    public void HandleInputButtonDownEvent(string button_name)
    {
        if (button_name == "Cancel")
            StartCoroutine(OnResetGame());
    }

    private IEnumerator OnResetGame()
    {
        var controller = FindObjectOfType<MainController>();
        controller.RequestChangeState(MainController.State.Default);
        yield return null;
    }
}