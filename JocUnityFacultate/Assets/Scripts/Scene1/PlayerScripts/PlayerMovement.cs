using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 15f;
    private InputAction moveAction;

    private Transform cameraTransform;
    private Rigidbody rb;

    void Start()
    {
        //Movementul playerului se face in functie de directi camerei
        cameraTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();

        moveAction = InputSystem.actions.FindAction("Move") ;
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        //Axele de miscare ale playerului
        Vector2 input = moveAction.ReadValue<Vector2>();
        float moveZ = input.y;
        float moveX = input.x;


        //Directia in functie de camera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        //Forward si right sunt vectori

        forward.y = 0f;
        right.y = 0f;

        //Normalizam vectorii pentru o miscare uniforma
        forward.Normalize();
        right.Normalize();
        Vector3 moveDirection = (forward * moveZ + right * moveX).normalized;

        //nu stiu ce face ascest if
        if (moveDirection.magnitude > 0.1f)
        {
            //Aplicam o forta asupra rigidBody pentru a misca playerul
            rb.AddForce(moveDirection * moveSpeed, ForceMode.VelocityChange);
            // Accelereaza la infinit?? Raspuns: NU. De ce? Playerul are Drag.

            //Quaternion.Slerp face o rotatie continua frumoasa. Nu stiu matematica din spate
            //Quaternion.Slerp(Quaternion from, Quaternion to, float t) , unde t este viteza de rotatie
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            
            
            
            // Vector3 targetVelocity = moveDirection * moveSpeed;
            // targetVelocity.y = rb.linearVelocity.y; // Păstrăm gravitația actuală pe verticală
            // rb.linearVelocity = targetVelocity;
        }
    }
}