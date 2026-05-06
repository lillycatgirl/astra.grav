using UnityEngine;

public class RightClickDragPan : MonoBehaviour
{
    private Camera _cam;
    private Vector3 _lastMousePos;
    public float zoom;
    private float _baseZoom;
    public Vector3 camLimits;

    private void Start()
    {
        _cam = Camera.main;
        _baseZoom = _cam.orthographicSize;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - _lastMousePos;

            // convert screen delta → world delta
            Vector3 move = _cam.ScreenToWorldPoint(Input.mousePosition)
                           - _cam.ScreenToWorldPoint(_lastMousePos);

            transform.position -= move;

            _lastMousePos = Input.mousePosition;
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            zoom -= Input.mouseScrollDelta.y;
            zoom = Mathf.Clamp(zoom, -4, 3);
            _cam.orthographicSize = zoom + _baseZoom;
        }

        var camHeight = _cam.orthographicSize;
        var camWidth = camHeight * _cam.aspect;

        var pos = _cam.transform.position;

        pos.x = Mathf.Clamp(pos.x, -camLimits.x + camWidth, camLimits.x - camWidth);
        pos.y = Mathf.Clamp(pos.y, -camLimits.y + camHeight, camLimits.y - camHeight);

        _cam.transform.position = pos;
    }
}