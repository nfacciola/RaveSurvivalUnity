using UnityEngine;

namespace RaveSurvival
{
  public class Enemy : MonoBehaviour
  {
      // Enemy's health value
      public float health = 50f;

      /// <summary>
      /// Reduces the enemy's health when taking damage.
      /// If health reaches zero or below, the enemy is destroyed.
      /// </summary>
      /// <param name="dmg">Amount of damage to apply</param>
      public void TakeDamage(float dmg) 
      {
        // Subtract damage from health
        health -= dmg;

        // Check if health has dropped to zero or below
        if (health <= 0f) 
        {
          Die(); // Trigger the death logic
        }
      }

      /// <summary>
      /// Handles the enemy's death logic.
      /// Destroys the enemy game object.
      /// </summary>
      void Die() 
      {
        Destroy(gameObject); // Remove the enemy from the scene
      }
  }
}