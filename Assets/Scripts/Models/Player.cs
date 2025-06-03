using UnityEngine;
using UnityEngine.Animations;
using Mirror;
using UnityEngine.UIElements;
using RaveSurvival;

public class Player : NetworkBehaviour
{
  // Reference to the player's camera
  public Camera cam;

  // Reference to the player's gun
  public Gun gun;

  // Transform representing the position of the camera
  public Transform cameraPos;

  // Player's health value
  [SyncVar] public float health = 50.0f;

  public override void OnStartClient()
  {
    name = $"player[{netId}|{(isLocalPlayer ? "local" : "remote")}]";
  }

  public override void OnStartServer()
  {
    name = $"player[{netId}|server]";
  }

  /// <summary>
  /// Unity's Start method, called before the first frame update.
  /// Sets up the camera for the local player and links it to the gun.
  /// </summary>
  public void Start()
  {
    if (!isLocalPlayer) return;
    // Find the first camera in the scene
    Camera camera = FindFirstObjectByType<Camera>();

    // Check if this is the local player
    
    // Attach the camera to the player's camera position
    camera.transform.parent = cameraPos.transform;
    camera.transform.position = cameraPos.position;
    camera.transform.rotation = cameraPos.rotation;

    // Link the camera to the gun
    gun.SetBulletStart(camera.gameObject.transform);
  }

    /// <summary>
    /// Unity's Update method, called once per frame.
    /// Currently empty but can be used for player-specific updates.
    /// </summary>
    void Update()
    {
        // Placeholder for future update logic
    }

    /// <summary>
    /// Reduces the player's health when taking damage.
    /// If health reaches zero, logs a message indicating the player was killed.
    /// </summary>
    /// <param name="dmg">Amount of damage to apply</param>
    /// <param name="killedBy">GameObject that caused the player's death</param>
    public void TakeDamage(float dmg, GameObject killedBy)
    {
        // Subtract damage from health
        health -= dmg;

        // Check if health has dropped to zero or below
        if (health <= 0)
        {
            health = 0; // Ensure health doesn't go negative
            Debug.Log("You were just killed by: " + killedBy); // Log the killer
        }
    }
}