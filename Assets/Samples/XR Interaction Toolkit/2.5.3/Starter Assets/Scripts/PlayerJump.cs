using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private InputActionProperty JumpButton;
    [SerializeField] private float JumpHeight = 3f;
    [SerializeField] private CharacterController cc;
    [SerializeField] private LayerMask groundLayers;

    private float gravity = Physics.gravity.y;
    private Vector3 movement;

    private void Update()
    {
        bool _IsGrounded = IsGrounded();

        if (JumpButton.action.WasPerformedThisFrame() && _IsGrounded)
        {
            Jump();
        }

        movement.y += gravity * Time.deltaTime;
        cc.Move(movement * Time.deltaTime);
    }

   private void Jump()
    {
        movement.y = Mathf.Sqrt(JumpHeight * -3.0f * gravity);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, 0.2f, groundLayers);
    }
}

