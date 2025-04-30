using System;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UIElements;

public class Gun : MonoBehaviour
{
  public float damage = 10f;
  public float range = 100f;
  public float fireRate = 15f;
  private float nextTimeToFire = 0f;

  public WeaponType weaponType = WeaponType.RAYCAST;

  public Transform rayStart;
  public ParticleSystem muzzleFlash;
  public GameObject impactEffect;

  public Transform projectileStart;
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

    public void Shoot() { 
      if(weaponType == WeaponType.RAYCAST) {
        if(audioSource.clip == null || audioSource != fireSound) {
          audioSource.clip = fireSound;
        }
        audioSource.Play();
        muzzleFlash.Play();
        RaycastHit hit;
        if (Physics.Raycast(rayStart.position, rayStart.forward, out hit, range)) {
          Enemy enemy = hit.transform.GetComponent<Enemy>();
          if(enemy != null) {
            enemy.TakeDamage(damage, rayStart);
          }

          GameObject impactFx = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
          Destroy(impactFx, 2f);
        }
      }
      else if(weaponType == WeaponType.PROJECTILE) {
        if(Time.time >= nextTimeToFire) {
          nextTimeToFire = Time.time + (1f/fireRate);
          if(audioSource.clip == null || audioSource != fireSound) {
            audioSource.clip = fireSound;
          }
          GameObject projectile = Instantiate(this.projectile);
          projectile.transform.position = projectileStart.position;
          projectile.transform.rotation = projectileStart.rotation;
          projectile.GetComponent<Projectile>().FireBullet(15f);
        }
        
      } else {

      }
    }
}
