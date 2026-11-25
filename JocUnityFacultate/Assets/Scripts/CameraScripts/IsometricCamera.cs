using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    [SerializeField]
    private Transform targetPosition;
    [SerializeField]
    private float smoothTime = 0.15f;
    private float distance = 100.0f;

    Vector3 offset;
    Vector3 velocity;

    void Start()
    { 
        //directia camerei pe axa blue (z): transform.forward (rezulta un versor)
        //directa inspre punctul dorit pentru camera: -transform.forward
        //distanta scaleaza versorul initial de directie al camerei
        //totusi in ortographic view distanta nu schimba cu nimic perspectiva obiectelor
        offset = -transform.forward * distance;
    }

    private void LateUpdate()
    {   
        //calculam pozitia camerei folosind offsetul calculat
        Vector3 pozitieCamera = targetPosition.position + offset;

        //transformam pozitia camerei
        transform.position = Vector3.SmoothDamp(transform.position, pozitieCamera, ref velocity, smoothTime);
    }
}
