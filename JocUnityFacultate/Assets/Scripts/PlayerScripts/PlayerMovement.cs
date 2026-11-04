using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private InputAction moveAction;
<<<<<<< HEAD
    [SerializeField]
=======
    public float speed;
>>>>>>> origin/main


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 direction = new Vector3(input.x, 0 ,input.y);
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
