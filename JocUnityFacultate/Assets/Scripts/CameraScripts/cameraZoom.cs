using UnityEngine;
using UnityEngine.InputSystem;

public class cameraZoom : MonoBehaviour
{
    private float maxZoom = 80f;
    private float minZoom = 10f;
    private float targetZoomValue;
    private float actualCameraSize;
    private float velocity;

    [SerializeField]
    private float zoomStep = 0.5f;

    [SerializeField]
    private float smoothTime = 0.3f;
    public InputActionReference zoomAction;
    private Camera cam;

        
    public void Awake()
    {
        //luam camera din ierarhie si egalam targetZoom cu Size-ul actual al camerei (cum la inceput nu facem nici ZoomIn nici ZoomOut)
        cam = GetComponent<Camera>();
        targetZoomValue = actualCameraSize = cam.orthographicSize;
    }

    public void Update()
    {   
        //La fiecare frame, zoomAction ne intoarece -1 (scroll down) , 1 (scroll down) sau 0 (no scroll action)
        float scroll = zoomAction.action.ReadValue<Vector2>().y;

        targetZoomValue -= scroll * zoomStep;

        //Clamp ne intoarece val minima/maxima din intervalul [min,max] daca targetZoomValue scade/creste peste aceste limite
        targetZoomValue = Mathf.Clamp(targetZoomValue, minZoom, maxZoom);

        //acutalizam size-ul camerei ortografice
        actualCameraSize = Mathf.SmoothDamp(actualCameraSize, targetZoomValue, ref velocity, smoothTime);
        cam.orthographicSize = actualCameraSize;
    }
}
