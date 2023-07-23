using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 15;
    private Vector3 move;

    public float gravity = -10;
    public float jumpHeight = 2;
    private Vector3 velocity;
    // Start is called before the first frame update

    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;

    InputAction movement;
    InputAction jump;

    public Animator animator;
    void Start()
    {
        movement = new InputAction("PlayerMovement");
        jump = new InputAction("Jump", binding: "<keyboard>/space");
        movement.AddCompositeBinding("Dpad")
            .With("Up", "<keyboard>/w")
            .With("Down", "<keyboard>/s")
            .With("Left", "<keyboard>/a")
            .With("Right", "<keyboard>/d");


        movement.Enable();
        jump.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        // float x = Input.GetAxis("Horizontal");
        // float z = Input.GetAxis("Vertical");

        float x = movement.ReadValue<Vector2>().x;
        float z = movement.ReadValue<Vector2>().y;

        animator.SetFloat("VelX", x);
        animator.SetFloat("VelZ", z);

        move =  transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.3f, groundLayer);
        if(isGrounded && velocity.y < 0){
            velocity.y = -1;
        }
        if (isGrounded) {
            if(Mathf.Approximately(jump.ReadValue<float>(), 1)) {
                Debug.Log("Jump!");

                PerformJump();
            }
        } else {
             velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);

    }

    private void PerformJump() {
     velocity.y = Mathf.Sqrt(jumpHeight * 2 * -gravity);
    }
}
