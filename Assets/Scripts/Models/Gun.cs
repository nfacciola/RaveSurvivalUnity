using UnityEngine;
using Mirror;

namespace RaveSurvival
{
  public class Gun : NetworkBehaviour
  {
    // Damage dealt by the gun
    public float damage = 10f;

    // Maximum range of the gun
    public float range = 100f;

    // Fire rate of the gun (shots per second)
    public float fireRate = 15f;

    // Reference to the first-person camera
    [SerializeField]
    private Camera fpsCam;

    // Particle system for the muzzle flash effect
    public ParticleSystem muzzleFlash;

    // Prefab for the impact effect when a shot hits something
    public GameObject impactEffect;

    // Time when the gun can fire the next shot
    private float nextTimeToFire = 0f;

    /// <summary>
    /// Sets the camera for the gun.
    /// This is used to link the player's camera to the gun.
    /// </summary>
    /// <param name="cam">Camera to set</param>
    public void SetCam(Camera cam)
    {
      fpsCam = cam;
    }

    /// <summary>
    /// Unity's Update method, called once per frame.
    /// Handles firing logic for the local player.
    /// </summary>
    void Update()
    {
        // Ensure only the local player can fire the gun
        if (isLocalPlayer)
        {
            // Check if the fire button is pressed and the gun is ready to fire
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                // Calculate the next time the gun can fire
                nextTimeToFire = Time.time + 1f / fireRate;

                // Trigger the shooting logic
                Shoot();
            }
        }
    }

    /// <summary>
    /// Handles the shooting logic for the gun.
    /// Plays the muzzle flash and sends a command to the server to process the shot.
    /// </summary>
    void Shoot()
    {
        // Play the muzzle flash effect
        muzzleFlash.Play();

        // Get the origin and direction of the shot from the camera
        Vector3 origin = fpsCam.transform.position;
        Vector3 direction = fpsCam.transform.forward;

        // Send the shot information to the server
        CmdShoot(origin, direction);
    }

    /// <summary>
    /// Command method executed on the server to process the shot.
    /// Handles raycasting, damage application, and spawning impact effects.
    /// </summary>
    /// <param name="origin">Origin of the shot</param>
    /// <param name="direction">Direction of the shot</param>
    [Command]
    void CmdShoot(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;

        // Play the muzzle flash effect on all clients
        RpcPlayMuzzleFlash();

        // Perform a raycast to detect what the shot hits
        if (Physics.Raycast(origin, direction, out hit, range))
        {
            Debug.Log(hit.transform.name); // Log the name of the object hit

            // Check if the object hit is an enemy and apply damage
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Spawn the impact effect at the hit point
            GameObject impactFx = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            NetworkServer.Spawn(impactFx); // Spawn the effect on the network
            Destroy(impactFx, 2f); // Destroy the effect after 2 seconds
        }
    }

    /// <summary>
    /// ClientRpc method executed on all clients to play the muzzle flash effect.
    /// Ensures the effect is played for non-local players.
    /// </summary>
    [ClientRpc]
    void RpcPlayMuzzleFlash()
    {
        if (!isLocalPlayer)
        {
            muzzleFlash.Play();
        }
    }
  }
}
