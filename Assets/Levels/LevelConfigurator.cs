using System.Collections;
using UnityEngine;

public class LevelConfigurator : MonoBehaviour
{
    [SerializeField]
    private int _PointsToFinish = 10000; // TODO
    private GameObject _WarpCloudObject;
    private GameplayScoreController _ScoreController;
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

        _WarpCloudObject = GameObject.Find("Warp Cloud");
        _WarpCloudObject.SetActive(false);

        _ScoreController = FindObjectOfType<GameplayScoreController>();
        _ScoreController.TotalPointsChanged += HandleTotalPointsChanged;

        // Demo mode
        StartCoroutine(CheckIdleInputForDemo());
    }

    private void HandleTotalPointsChanged(int total_points)
    {
        if(total_points > _PointsToFinish) // TODO
        {
            _WarpCloudObject.SetActive(true);

            var cloud_events = _WarpCloudObject.GetComponentInChildren<CollisionEvents2D>();
            cloud_events.TriggerEnter += HandleWarpCloudTriggerEnter;
            _ScoreController.TotalPointsChanged -= HandleTotalPointsChanged;
        }
    }

    private void HandleWarpCloudTriggerEnter(Collider2D collider)
    {
        if(collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StartCoroutine(OnSpooderWarp(_WarpCloudObject, collider));
            var cloud_events = _WarpCloudObject.GetComponentInChildren<CollisionEvents2D>();
            cloud_events.TriggerEnter -= HandleWarpCloudTriggerEnter;
            // Stop the timer
            var timer = FindObjectOfType<GameplayTimerController>();
            timer.enabled = false;
            _ScoreController.enabled = false;
        }
    }

    IEnumerator OnSpooderWarp(GameObject cloud, Collider2D spooder_collider)
    {
        var spooder_rb = spooder_collider.GetComponentInParent<Rigidbody2D>();
        var cloud_rb = cloud.GetComponentInChildren<Rigidbody2D>();
        var cloud_joint = cloud_rb.gameObject.AddComponent<FixedJoint2D>();
        var skate_jc = spooder_rb.GetComponent<SkateboardJointController>();
        skate_jc.BreakForce = new Vector2(5000f, 5000f);

        while (Vector2.Distance(spooder_rb.position, cloud_rb.position) > 0.1f)
        {
            spooder_rb.MovePosition(Vector2.Lerp(spooder_rb.position, cloud_rb.position, 5f * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
        }
        cloud_joint.connectedBody = spooder_rb;
        cloud_joint.autoConfigureConnectedAnchor = false;
        cloud_joint.anchor = Vector2.zero;
        var cloud_animator = cloud.GetComponent<Animator>();
        cloud_animator.SetTrigger("Ascend");
        yield return new WaitForSeconds(1);
        var hud = FindObjectOfType<GameplayHUDController>();
        hud.GetComponent<Animator>().SetBool("Focused", true);

        // Demo mode: automatically reset the game after showing the score for 10 seconds
        StartCoroutine(OnResetGame());
        yield return new WaitForSeconds(10);
        
    }

    public void HandleInputButtonDownEvent(string button_name)
    {
        if (button_name == "Cancel" || button_name == "Player 1" || button_name == "Player 2")
            StartCoroutine(OnResetGame());
        else if (button_name == "Toggle1")
        {
            var spooder_object = GameObject.Find("Spooder");
            var spooder_animator = spooder_object.GetComponent<Animator>();
            var hat = !spooder_animator.GetBool("Hat");
            spooder_animator.SetBool("Hat", hat);
        }
    }

    private IEnumerator CheckIdleInputForDemo()
    {
        // Demo mode: automatically reset the game if there is no input for some time
        var last_input_time = Time.unscaledTime;
        var player_input = FindObjectOfType<PlayerInput>();
        for (; ; )
        {
            var any_input =
                player_input.GetAxis("Horizontal") != 0 ||
                player_input.GetAxis("Vertical") != 0 ||
                player_input.GetButton("Jump");
            if (any_input)
                last_input_time = Time.unscaledTime;

            // Reset after 20 seconds
            if( (Time.unscaledTime - last_input_time) > 20f)
            {
                StartCoroutine(OnResetGame());
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator OnResetGame()
    {
        var controller = GameObject
            .FindWithTag("GameController")
            .GetComponent<IMainController>();
        controller.RequestChangeState("Default");
        yield return null;
    }
}