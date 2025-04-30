using UnityEngine;
using UnityEngine.Animations;

public class Player : MonoBehaviour
{
    public float health = 50.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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