
using Sirenix.OdinInspector;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField][Range(0, 10)] private float panLerp = 5;
    [SerializeField][Range(0, 10)] private float zoomLerp = 5;
    [SerializeField][Range(0, 10)] private float targetZoom = 5;

    [SerializeField][ReadOnly] private Transform target;

    [SerializeField][ReadOnly] private bool followTarget = false;

    private float INITIAL_CAMERA_SIZE = 10;

    private Vector3 INITIAL_POSITION = new Vector3(17, 17, -17);

    private Vector3 EASY_INITIAL_POSITION = new Vector3(6.5f, 6.5f, -6.5f);
    private Vector3 NORMAL_INITIAL_POSITION = new Vector3(10, 10, -10);
    private Vector3 HARD_INITIAL_POSITION = new Vector3(17, 17, -17);
    private Vector3 IMPOSSIBLE_INITIAL_POSITION = new Vector3(27, 27, -27);

    private float EASY_CAMERA_SIZE = 6.5f;
    private float NORMAL_CAMERA_SIZE = 10;
    private float HARD_CAMERA_SIZE = 17;
    private float IMPOSSIBLE_CAMERA_SIZE = 27;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        transform.position = INITIAL_POSITION;
        Camera.main.orthographicSize = INITIAL_CAMERA_SIZE;
    }

    private void Update()
    {
        if (followTarget)
        {
            transform.position = Vector3.Lerp(transform.position, target.position - Camera.main.transform.forward * 50, panLerp * Time.deltaTime);
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, zoomLerp * Time.unscaledDeltaTime);
        }
    }

    public void OnMazeGenerated()
    {
        Vector3 position = Vector3.zero;
        float cameraSize = 0;

        switch (GameManager.Instance.GetDifficulty())
        {
            case GameManager.Difficulty.Easy:
                position = EASY_INITIAL_POSITION;
                cameraSize = EASY_CAMERA_SIZE;
                break;
            case GameManager.Difficulty.Normal:
                position = NORMAL_INITIAL_POSITION;
                cameraSize = NORMAL_CAMERA_SIZE;
                break;
            case GameManager.Difficulty.Hard:
                position = HARD_INITIAL_POSITION;
                cameraSize = HARD_CAMERA_SIZE;
                break;
            case GameManager.Difficulty.Impossible:
                position = IMPOSSIBLE_INITIAL_POSITION;
                cameraSize = IMPOSSIBLE_CAMERA_SIZE;
                break;
        }

        transform.position = position;
        Camera.main.orthographicSize = cameraSize;
    }

    public void SetFollowMode(bool _value)
    {
        followTarget = _value;
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }
}