using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 50f;

    public void TakeDamage(float dmg) {
      health -= dmg;
      if(health <= 0f) {
        Die();
      }
    }

    void Die() {
      Destroy(gameObject);
    }
}
