using UnityEngine;
using Mirror;

namespace RaveSurvival
{
  public class Gun : NetworkBehaviour
  {
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;
    [SerializeField]
    private Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    private float nextTimeToFire = 0f;

    public void SetCam(Camera cam)
    {
      fpsCam = cam;
    }
    // Update is called once per frame
    void Update()
      {
        if(isLocalPlayer)
        {
          if(Input.GetButton("Fire1") && Time.time >= nextTimeToFire) {
            nextTimeToFire = Time.time + 1f/fireRate;
            Shoot();
          }
        }
      }

    void Shoot()
    {
      muzzleFlash.Play();
      Vector3 origin = fpsCam.transform.position;
      Vector3 direction = fpsCam.transform.forward;
      CmdShoot(origin, direction);
    }

    [Command]
    void CmdShoot(Vector3 origin, Vector3 direction) {
      
      RaycastHit hit;
      RpcPlayMuzzleFlash();
      if (Physics.Raycast(origin, direction, out hit, range)) {
        Debug.Log(hit.transform.name);

        Enemy enemy = hit.transform.GetComponent<Enemy>();
        if(enemy != null) {
          enemy.TakeDamage(damage);
        }

        GameObject impactFx = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        // SetImpactFx(impactFx);
        NetworkServer.Spawn(impactFx);
        Destroy(impactFx, 2f);
      }
    }

    [ClientRpc]
    void RpcPlayMuzzleFlash() {
        if(!isLocalPlayer)
        {
          muzzleFlash.Play();
        }
    }
  }
}
