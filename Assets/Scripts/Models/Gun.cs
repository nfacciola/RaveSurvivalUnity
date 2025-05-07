using Mirror;
using UnityEngine;

public class Gun : NetworkBehaviour
{
  public float damage = 10f;
  public float range = 100f;
  public float fireRate = 15f;
  private float nextTimeToFire = 0f;

  public WeaponType weaponType = WeaponType.RAYCAST;

  [SerializeField]
  public Transform bulletStart;
  public ParticleSystem muzzleFlash;
  public GameObject impactEffect;

  public GameObject projectile;

  private AudioSource audioSource;
  public AudioClip fireSound;
    public enum WeaponType {
    RAYCAST = 0,
    PROJECTILE
  }

  void Start()
  {
    audioSource = GetComponent<AudioSource>();
  }

  // Update is called once per frame
  void Update()
    {
        if(Input.GetButton("Fire1") && Time.time >= nextTimeToFire) {
          nextTimeToFire = Time.time + (1f/fireRate);
          Shoot();
        }
    }

    public void SetBulletStart(Transform start) {
      bulletStart = start;
    }

    public void Shoot()
    {
        // Play the muzzle flash effect
        muzzleFlash.Play();

        // Get the origin and direction of the shot from the camera
        Transform origin = bulletStart.transform;
        Vector3 direction = bulletStart.transform.forward;

        // Send the shot information to the server
        CmdShoot(origin, direction);
    }

    [Command]
    void CmdShoot(Transform origin, Vector3 direction) { 
      RpcPlayMuzzleFlash();
      if(weaponType == WeaponType.RAYCAST) {
        
        RaycastHit hit;
        if (Physics.Raycast(origin.position, direction, out hit, range)) {
          Enemy enemy = hit.transform.GetComponent<Enemy>();
          if(enemy != null) {
            enemy.TakeDamage(damage, bulletStart);
          }

          GameObject impactFx = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
          Destroy(impactFx, 2f);
        }
      }
      else if(weaponType == WeaponType.PROJECTILE) {
        if(Time.time >= nextTimeToFire) {
          nextTimeToFire = Time.time + (1f/fireRate);
          GameObject projectile = Instantiate(this.projectile);
          projectile.transform.position = origin.position;
          projectile.transform.rotation = origin.rotation;
          projectile.GetComponent<Projectile>().FireBullet(15f);
        }
        
      } else {

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
            if(audioSource.clip == null || audioSource != fireSound) {
          audioSource.clip = fireSound;
        }
        audioSource.Play();
        muzzleFlash.Play();
        }
    }
}
