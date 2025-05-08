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

  private bool isReady = false;

    void Awake()
    {
        if (this.weaponType == WeaponType.PROJECTILE && bulletStart == null)
        {
            bulletStart = transform.Find("bulletSpawn");
            if (bulletStart == null)
            {
                Debug.LogError("Gun: bulletSpawn Transform not found on this weapon!");
            }
        }

        audioSource = GetComponent<AudioSource>();
    }


    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        isReady = true;
    }

  // Update is called once per frame
  void Update()
    {
        if(!isReady)
          return;

        if(!isLocalPlayer)
          return;

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
        muzzleFlash.Play();

        if(bulletStart == null)
        {
          Debug.LogWarning("bulletStart us null, cannot shoot");
          return;
        }

        // Get the origin and direction of the shot from the camera
        Vector3 originPosition = bulletStart.position;
        Vector3 direction = bulletStart.forward;

        //If this gun is server side (e.g on an enemy), do the shoot logic server side
        if (isServer)
        {
          ServerShoot(originPosition, direction);
        }
        else
        {
          // Send the shot information to the server
          CmdShoot(originPosition, direction);
        }
    }

    void ServerShoot(Vector3 originPosition, Vector3 direction)
    {
      RpcPlayMuzzleFlash();
      if(weaponType == WeaponType.RAYCAST) {
        
        RaycastHit hit;
        if (Physics.Raycast(originPosition, direction, out hit, range)) {
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
          GameObject projectile = Instantiate(this.projectile, originPosition, Quaternion.LookRotation(direction));
          projectile.GetComponent<Projectile>().FireBullet(15f);
        }
      } 
    }

    [Command]
    void CmdShoot(Vector3 originPosition, Vector3 direction) { 
      RpcPlayMuzzleFlash();
      if(weaponType == WeaponType.RAYCAST) {
        
        RaycastHit hit;
        if (Physics.Raycast(originPosition, direction, out hit, range)) {
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
          GameObject projectile = Instantiate(this.projectile, originPosition, Quaternion.LookRotation(direction));
          projectile.GetComponent<Projectile>().FireBullet(15f);
        }
      } 
    }

    /// <summary>
    /// ClientRpc method executed on all clients to play the muzzle flash effect.
    /// Ensures the effect is played for non-local players.
    /// </summary>
    [ClientRpc]
    void RpcPlayMuzzleFlash()
    {
        if (isLocalPlayer)
        {
            if(audioSource.clip == null || audioSource.clip != fireSound) {
          audioSource.clip = fireSound;
        }
        audioSource.Play();
        muzzleFlash.Play();
        }
    }
}
