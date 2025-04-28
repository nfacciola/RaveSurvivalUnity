using System;
using UnityEngine;
using Mirror;

namespace RaveSurvival
{
    public class PlayerLookHandler : NetworkBehaviour
    {

        public float mouseSensitivity = 100f;

        public Transform playerBody;

        float xRotation = 0f;

        void Start()
        {
            if(isLocalPlayer)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        void Update()
        {
            if(isLocalPlayer)
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

                xRotation -= mouseY;
                xRotation = Math.Clamp(xRotation, -90f, 90f);

                transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

                playerBody.Rotate(Vector3.up * mouseX);
            }
        }
    }
}
