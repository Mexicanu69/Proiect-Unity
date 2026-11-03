using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    [SerializeField]
    private Transform targetPosition;
    [SerializeField]
    private float distance = 50f;
    [SerializeField]
    private float smoothTime = 0.15f;

    Vector3 offset;
    Vector3 velocity;

    void Start()
    { 
        //directia camerei pe axa blue (z): transform.forward (rezulta un versor)
        //directa inspre punctul dorit pentru camera: -transform.forward
        //distanta scaleaza versorul initial de directie al camerei
        offset = -transform.forward * distance;
    }

    private void LateUpdate()
    {   
        //pozitia camerei folosind offsetul calculat
        Vector3 pozitieCamera = targetPosition.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, pozitieCamera, ref velocity, smoothTime);
    }
}
