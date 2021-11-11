using System;
using UnityEngine;
using Mirror;
using System.Collections;

public enum WeaponSlot
{
	Primary,
	Secondary
}

public class WeaponManager : NetworkBehaviour
{

	[SerializeField]
	private string weaponLayerName = "Weapon";

	[SerializeField]
	private Transform weaponHolder;

	[SerializeField]
	private PlayerWeapon primaryWeapon;

	[SerializeField] 
	private PlayerWeapon secondaryWeapon;

	private PlayerWeapon currentWeapon;
	private WeaponGraphics currentGraphics;
	[SerializeField] private PlayerFiring playerShooting;

	public bool isReloading = false;
	
	public event Action<int, int> ClientOnAmmoUpdated;

	void Start()
	{
		EquipWeapon(primaryWeapon);
	}

	public PlayerWeapon GetCurrentWeapon()
	{
		return currentWeapon;
	}

	public WeaponGraphics GetCurrentGraphics()
	{
		return currentGraphics;
	}

	public void EquipWeapon(WeaponSlot slot)
	{
		switch (slot)
		{
			case WeaponSlot.Primary:
				EquipWeapon(primaryWeapon);
				break;
			case WeaponSlot.Secondary:
				EquipWeapon(secondaryWeapon);
				break;
			default:
				Debug.LogError($"Invalid enum type in EquipWeapon: {slot}");
				break;
		}
	}
	
	void EquipWeapon(PlayerWeapon _weapon)
	{
		if (currentWeapon != null)
		{
			Destroy(currentGraphics.gameObject);
		}

		currentWeapon = _weapon;

		GameObject _weaponIns = (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
		_weaponIns.transform.SetParent(weaponHolder);

		currentWeapon.bullets = currentWeapon.maxBullets;

		currentGraphics = _weaponIns.GetComponent<WeaponGraphics>();
		if (currentGraphics == null)
			Debug.LogError("No WeaponGraphics component on the weapon object: " + _weaponIns.name);

		if (hasAuthority)
			Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
		
		// Update Ammo UI
		ClientOnAmmoUpdated?.Invoke(currentWeapon.bullets, currentWeapon.maxBullets);
	}

	public void Reload()
	{
		if (isReloading)
			return;

		StartCoroutine(Reload_Coroutine());
	}

	private IEnumerator Reload_Coroutine()
	{
		isReloading = true;

		CmdOnReload();

		yield return new WaitForSeconds(currentWeapon.reloadTime);

		currentWeapon.bullets = currentWeapon.maxBullets;

		playerShooting.OnReloadComplete();

		isReloading = false;
	}

	[Command]
	void CmdOnReload()
	{
		RpcOnReload();
	}

	[ClientRpc]
	void RpcOnReload()
	{
		Animator anim = currentGraphics.GetComponent<Animator>();
		if (anim != null)
		{
			anim.SetTrigger("Reload");
		}
	}

}
