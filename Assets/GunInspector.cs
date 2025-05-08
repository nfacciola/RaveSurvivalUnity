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
    ObjectField bulletStart;
    ObjectField muzzleFlash;
    ObjectField impactEffect;
    ObjectField projectile;
  public override VisualElement CreateInspectorGUI()
  {
    // Create a new VisualElement to be the root of our Inspector UI.
    myInspector = new VisualElement();

    damage = new("Damage") { bindingPath = "damage" };
    range = new("Range") { bindingPath = "range"};
    fireRate = new("FireRate") { bindingPath = "fireRate" };
    weaponType = new("Weapon Type") { bindingPath = "weaponType" };
    fireSound = new("Fire Sound") { bindingPath = "fireSound" };
    bulletStart = new("Bullet Start") { bindingPath = "bulletStart" };
    muzzleFlash = new("Muzzle Flash") { bindingPath = "muzzleFlash" };
    impactEffect = new("Impact Effect") { bindingPath = "impactEffect"};
    projectile = new("Projectile") {bindingPath = "projectile"};

    // Add a simple label.
    myInspector.Add(damage);
    myInspector.Add(range);
    myInspector.Add(fireRate);
    myInspector.Add(weaponType);
    myInspector.Add(fireSound);
    myInspector.Add(bulletStart);
    myInspector.Add(muzzleFlash);
    myInspector.Add(impactEffect);
    myInspector.Add(projectile);

    handler(weaponType);

    return myInspector;
  }
  public void handler(EnumField field) {
    field.RegisterCallback<ChangeEvent<Enum>>(showFields);
  }
  public void showFields(ChangeEvent<Enum> evt) {
    EnumField field = evt.currentTarget as EnumField;
    
    if (field.value.Equals(Gun.WeaponType.PROJECTILE)) {
      myInspector.Remove(impactEffect);
      myInspector.Add(projectile);
    } else {
      myInspector.Add(impactEffect);
      myInspector.Remove(projectile);
    }
  }
}
