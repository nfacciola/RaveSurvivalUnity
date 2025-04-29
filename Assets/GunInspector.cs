using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;
using UnityEngine.UIElements;

[ExecuteInEditMode]
[CustomEditor(typeof(Gun))]
public class GunInspector: Editor
{
  VisualElement myInspector;

  FloatField damage;
    FloatField range;
    FloatField fireRate;
    EnumField weaponType;
    ObjectField fireSound;
    ObjectField rayStart;
    ObjectField muzzleFlash;
    ObjectField impactEffect;
    ObjectField projectile;
    ObjectField projectileStart;
  public override VisualElement CreateInspectorGUI()
  {
    // Create a new VisualElement to be the root of our Inspector UI.
    myInspector = new VisualElement();

    damage = new("Damage") { bindingPath = "damage" };
    range = new("Range") { bindingPath = "range"};
    fireRate = new("FireRate") { bindingPath = "fireRate" };
    weaponType = new("Weapon Type") { bindingPath = "weaponType" };
    fireSound = new("Fire Sound") { bindingPath = "fireSound" };
    rayStart = new("Ray Start") { bindingPath = "rayStart" };
    muzzleFlash = new("Muzzle Flash") { bindingPath = "muzzleFlash" };
    impactEffect = new("Impact Effect") { bindingPath = "impactEffect"};
    projectile = new("Projectile") {bindingPath = "projectile"};
    projectileStart = new("Projectile Start") {bindingPath = "projectileStart"};

    // Add a simple label.
    myInspector.Add(damage);
    myInspector.Add(range);
    myInspector.Add(fireRate);
    myInspector.Add(weaponType);
    myInspector.Add(fireSound);
    myInspector.Add(rayStart);
    myInspector.Add(muzzleFlash);
    myInspector.Add(impactEffect);
    myInspector.Add(projectile);
    myInspector.Add(projectileStart);

    handler(weaponType);

    return myInspector;
  }
  public void handler(EnumField field) {
    field.RegisterCallback<ChangeEvent<Enum>>(showFields);
  }
  public void showFields(ChangeEvent<Enum> evt) {
    EnumField field = evt.currentTarget as EnumField;
    
    if (field.value.Equals(Gun.WeaponType.PROJECTILE)) {
      myInspector.Remove(rayStart);
      myInspector.Remove(muzzleFlash);
      myInspector.Remove(impactEffect);
      myInspector.Add(projectile);
      myInspector.Add(projectileStart);
    } else {
      myInspector.Add(rayStart);
      myInspector.Add(muzzleFlash);
      myInspector.Add(impactEffect);
      myInspector.Remove(projectile);
      myInspector.Remove(projectileStart);
    }
  }
}
