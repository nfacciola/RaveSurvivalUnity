using UnityEngine;
using UnityEngine.Animations;
using Mirror;
using UnityEngine.UIElements;

public class Player : NetworkBehaviour
{
  public Camera cam;
  public Transform cameraPos;
  public float health = 50.0f;
  public void Start()
  {
    Camera camera = FindFirstObjectByType<Camera>();
    if(isLocalPlayer) {
     camera.transform.position = cameraPos.position;
     camera.transform.rotation = cameraPos.rotation;
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