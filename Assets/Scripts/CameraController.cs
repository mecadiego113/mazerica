
using Sirenix.OdinInspector;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField][Range(0, 10)] private float panLerp = 5;
    [SerializeField][Range(0, 10)] private float zoomLerp = 5;
    [SerializeField][Range(0, 10)] private float targetZoom = 5;

    [SerializeField][ReadOnly] private Transform target;

    [SerializeField][ReadOnly] private bool followTarget = false;

    private float INITIAL_CAMERA_SIZE = 10;
    private Vector3 INITIAL_POSITION = new Vector3(17, 17, -17);

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

    public void SetFollowMode(bool _value)
    {
        followTarget = _value;
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }
}