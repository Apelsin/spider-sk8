using System.Collections;
using UnityEngine;

public class MainMenuConfigurator : MonoBehaviour
{
    private void Start()
    {
        var camera = Camera.main;
        var camera_contstraints = GameObject.Find("Camera Constraints").GetComponent<RectTransform>();
        var level_cam = camera.GetComponentInParent<LevelCamera2D>();
        level_cam.Constraints = camera_contstraints;
        var canvases = FindObjectsOfType<Canvas>();
        foreach (var canvas in canvases)
            canvas.worldCamera = camera;
        //level_cam.Target = GameObject.FindGameObjectWithTag("Camera Target").GetComponent<Rigidbody2D>();

        var player_input = FindObjectOfType<PlayerInput>();
        var input_events = GetComponent<InputEvents>();
        input_events.Input = player_input;
    }

    public void HandleInputButtonDownEvent(string button_name)
    {
        if (button_name == "Jump")
            StartCoroutine(OnStartGame());
    }

    private IEnumerator OnStartGame()
    {
        var press_start_ani = GameObject.Find("PRESS START").GetComponent<Animator>();
        press_start_ani.SetFloat("Speed", 5f);
        yield return new WaitForSeconds(1f);
        var controller = GameObject
            .FindWithTag("GameController")
            .GetComponent<IMainController>();
        controller.LoadLevel("Level 1");
    }
}