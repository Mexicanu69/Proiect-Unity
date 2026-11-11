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
        cameraTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move") ;
    }

    void Update()
    {

    }

    void FixedUpdate()
    {

        Vector2 input = moveAction.ReadValue<Vector2>();
        float moveZ = input.y;
        float moveX = input.x;


        // 2. Calculăm direcția în funcție de cameră
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();
        Vector3 moveDirection = (forward * moveZ + right * moveX).normalized;

        // 3. Mutăm și rotim jucătorul
        if (moveDirection.magnitude > 0.1f)
        {
            rb.MovePosition(rb.position + moveDirection *moveSpeed * Time.fixedDeltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}