using System;
using UnityEngine;
using Mirror;

namespace RaveSurvival
{
    public class PlayerMoveHandler : NetworkBehaviour
    {
        // Reference to the CharacterController component for handling movement
        public CharacterController controller;

        // Movement speed of the player
        public float speed = 12f;

        // Gravity applied to the player
        public float gravity = -9.81f;

        // Height the player can jump
        public float jumpHeight = 3f;

        // Transform used to check if the player is grounded
        public Transform groundCheck;

        // Radius of the sphere used for ground detection
        public float groundDistance = 0.4f;

        // Layer mask to identify what is considered ground
        public LayerMask groundMask;

        // Velocity vector for vertical movement (gravity and jumping)
        Vector3 velocity;

        // Boolean to check if the player is grounded
        bool isGrounded;

        /// <summary>
        /// Unity's Update method, called once per frame.
        /// Handles player movement and jumping for the local player.
        /// </summary>
        void Update()
        {
            // Ensure the movement logic only applies to the local player
            if (isLocalPlayer)
            {
                // Check if the player is grounded using a sphere at the groundCheck position
                isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

                // Reset vertical velocity if the player is grounded
                if (isGrounded && velocity.y < 0)
                {
                    velocity.y = -2f; // Small negative value to keep the player grounded
                }

                // Get input for horizontal (x) and vertical (z) movement
                float x = Input.GetAxis("Horizontal");
                float z = Input.GetAxis("Vertical");

                // Calculate the movement direction based on input and player orientation
                Vector3 move = transform.right * x + transform.forward * z;

                // Move the player using the CharacterController
                controller.Move(move * speed * Time.deltaTime);

                // Check if the jump button is pressed and the player is grounded
                if (Input.GetButtonDown("Jump") && isGrounded)
                {
                    // Calculate the vertical velocity needed to achieve the desired jump height
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }

                // Apply gravity to the vertical velocity
                velocity.y += gravity * Time.deltaTime;

                // Apply the vertical velocity to the player
                controller.Move(velocity * Time.deltaTime);
            }
        }
    }
}