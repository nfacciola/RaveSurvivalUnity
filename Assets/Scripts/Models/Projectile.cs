using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
  private float damage = 5.0f;
  private Rigidbody rb;
  [SerializeField]
  private float lifetime = 3f;

  void Awake() {
    rb = GetComponent<Rigidbody>();
  }
    
  public void FireBullet(float velocity) {
    if (rb != null) {
      rb.AddForce(transform.forward * velocity);
    }
    //Handle destrcution server side
    if(isServer)
    {
      Invoke(nameof(DestroySelf), lifetime);
    }
  }

  void DestroySelf()
  {
    NetworkServer.Destroy(this.gameObject);
  }
  void OnCollisionEnter(Collision collision)
  {
    if(!isServer)
      return;
    
    if(collision.gameObject.tag == "Player") {
      Player player = collision.gameObject.GetComponent<Player>();
      if(player != null) {
        player.TakeDamage(damage, gameObject);
      }
    }
    NetworkServer.Destroy(gameObject);
  }
  public void setDamage(float dmg) {
    damage = dmg;
  }
}
