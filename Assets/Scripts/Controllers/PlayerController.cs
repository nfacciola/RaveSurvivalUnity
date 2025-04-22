using UnityEngine;
using Mirror;

namespace RaveSurvival
{
    public class PlayerController : NetworkBehaviour
    {
        void Update()
        {
            if(isLocalPlayer)
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                Vector3 playerMovement = new Vector3(h * 0.25f, v * 0.25f, 0);
                transform.position = transform.position + playerMovement;
            }
        }
    }
}
