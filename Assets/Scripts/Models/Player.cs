using UnityEngine;
using UnityEngine.Animations;
using Mirror;
using UnityEngine.UIElements;
using RaveSurvival;

public class Player : NetworkBehaviour
{
  public Camera cam;
  public Gun gun;
  public Transform cameraPos;
  public float health = 50.0f;
  public void Start()
  {
    Camera camera = FindFirstObjectByType<Camera>();
    if(isLocalPlayer) {
      camera.transform.parent = cameraPos.transform;
      camera.transform.position = cameraPos.position;
      camera.transform.rotation = cameraPos.rotation;
      gun.SetCam(camera);
    }
  }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float dmg, GameObject killedBy) {
      health -= dmg;
      if (health <= 0) {
        health = 0;
        Debug.Log("You were just killed by: " + killedBy);
      }
    }
}