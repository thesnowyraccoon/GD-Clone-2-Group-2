using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    public float moveSpeed = 5f;    // player default move speed 

    float gravity = -9.8f;      // gravitational constant
    float verticalVelocity; 

    Vector2 moveInput;
    CharacterController cc;

    void Awake()
    {
        cc = GetComponent<CharacterController>();  
    }

    void Update()
    {
        // player movement relative to direction
        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y) * moveSpeed;

        // player vertical movement
        if (cc.isGrounded)
        {
            verticalVelocity = -1f;     // small force that ensures player is grounded
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;   // player falls according to gravity
        }

        move.y = verticalVelocity;  // sets vertical movement according to gravity

        cc.Move(move * Time.deltaTime);     // activates movement
    }

    public void OnMovement(InputAction.CallbackContext context)     // unity input system
    {
        moveInput = context.ReadValue<Vector2>();   // reads player movement input to a variable
    }
}
