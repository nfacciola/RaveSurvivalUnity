using UnityEngine;
using UnityEngine.EventSystems;

public class Projectile : MonoBehaviour
{
  private float damage = 5.0f;
  private Rigidbody rb;
  void Awake()
  {
    rb = GetComponent<Rigidbody>();
    if (rb != null) {
      rb.AddForce(transform.forward * 15);
    }
  }
  void OnCollisionEnter(Collision collision)
  {
    if(collision.gameObject.tag == "Player") {
      Player player = collision.gameObject.GetComponent<Player>();
      if(player != null) {
        player.TakeDamage(damage, gameObject);
      }
    }
  }
  public void setDamage(float dmg) {
    damage = dmg;
  }
}
