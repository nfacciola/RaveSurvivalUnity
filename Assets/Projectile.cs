using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Projectile : MonoBehaviour
{
  private float damage = 5.0f;
  private Rigidbody rb;
  void Awake() {
    rb = GetComponent<Rigidbody>();
  }
  public void FireBullet(float velocity) {
    if (rb != null) {
          rb.AddForce(transform.forward * velocity);
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
