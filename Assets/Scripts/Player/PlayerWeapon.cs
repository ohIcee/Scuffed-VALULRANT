using UnityEngine;
using System.Collections.Generic;

public enum WeaponTypes
{ 
	Auto,
	Semi,
	Single,
	Melee,
	Grenade
}

[System.Serializable]
[CreateAssetMenu(menuName = "Weapons/Create Weapon", order = 0)]
public class PlayerWeapon : ScriptableObject
{

	public string weaponName;
	public WeaponTypes weaponType;

	public int damage = 10;
	public float range = 100f;
	public float fireRate = 0f;

	public int maxBullets = 20;
	[HideInInspector] public int bullets;

	public float reloadTime = 1f;
	public float explosionRange = 1f;

	public GameObject graphics;

	public List<AudioClip> UseSounds;

	// -- camera shake settings
	public float fireCameraShakeIntensity;
	public float fireCameraShakeTime;

	// -- weapon recoil ssettings
	public float verticalRecoilPerShot = .1f;
	public float maxVerticalRecoil = 10f;
	public Vector2 horizontalRecoilPerShot = new Vector2(-.1f, .1f);
	public float horizontalRecoilAfterMaxVerticalRecoil = 2f;
	public float recoilRecoveryTime = 1f;

	public PlayerWeapon()
	{
		bullets = maxBullets;
	}

	public AudioClip GetRandomShotSound() => Util.GetRandomAudioClip(UseSounds);

}

