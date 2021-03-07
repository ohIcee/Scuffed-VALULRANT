using System.Collections;
using UnityEngine;
using Mirror;
using System;

public class PlayerShooting : NetworkBehaviour
{

    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private Camera playerCamera;

    [SerializeField]
    private LayerMask layerMask;

	[SerializeField] private AudioSource shootingAudioSource;
	[SerializeField] private WeaponRecoil weaponRecoil;
	[SerializeField] private PlayerHealth playerHealth;

	public event Action<int, int> ClientOnAmmoUpdated;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;
	private Player player;

    void Start()
    {
        if (playerCamera == null)
        {
            Debug.LogError("PlayerShoot: No camera referenced!");
            this.enabled = false;
        }

		player = GetComponent<Player>();
        weaponManager = GetComponent<WeaponManager>();
        currentWeapon = weaponManager.GetCurrentWeapon();

		ClientOnAmmoUpdated?.Invoke(currentWeapon.bullets, currentWeapon.maxBullets);
	}

	#region Server

	//Is called on the server when a player shoots
	[Command]
	void CmdOnShoot(PlayerShooting shootingPlayer)
	{
		RpcDoShootEffect(shootingPlayer);
	}

	//Is called on the server when we hit something
	//Takes in the hit point and the normal of the surface
	[Command]
	void CmdOnHit(Vector3 _pos, Vector3 _normal)
	{
		RpcDoHitEffect(_pos, _normal);
	}

	[Command]
	void CmdPlayerShot(NetworkIdentity _playerIdentity, int _damage)
	{
		RpcPlayerShot();

		// Return if we hit our own unit
		if (_playerIdentity.connectionToClient == connectionToClient) return;

		if (_playerIdentity.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
		{
			playerHealth.DealDamage(_damage, player.GetNetworkPlayer());
		}
	}

	#endregion

	#region Client

	//Is called on all clients when we need to to
	// a shoot effect
	[ClientRpc]
	void RpcDoShootEffect(PlayerShooting shootingPlayer)
	{
		weaponManager.GetCurrentGraphics().muzzleFlash.Play();
		//shootingAudioSource.PlayOneShot(currentWeapon.GetRandomShotSound());
		shootingPlayer.shootingAudioSource.PlayOneShot(currentWeapon.GetRandomShotSound());

		Debug.Log($"Playing sound effect on {transform.name}");
	}

	//Is called on all clients
	//Here we can spawn in cool effects
	[ClientRpc]
	void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
	{
		GameObject _hitEffect = (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
		Destroy(_hitEffect, 2f);
	}

	[Client]
	void Shoot()
	{
		if (!hasAuthority || weaponManager.isReloading)
		{
			return;
		}

		if (currentWeapon.bullets <= 0)
		{
			weaponManager.Reload();
			return;
		}

		currentWeapon.bullets--;
		ClientOnAmmoUpdated?.Invoke(currentWeapon.bullets, currentWeapon.maxBullets);

		//Debug.Log("Remaining bullets: " + currentWeapon.bullets);

		weaponRecoil.Fire();
		//shootingAudioSource.PlayOneShot(currentWeapon.GetRandomShotSound());

		//We are shooting, call the OnShoot method on the server
		CmdOnShoot(this);

		RaycastHit _hit;
		if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out _hit, currentWeapon.range, layerMask))
		{
			if (_hit.transform.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
			{
				CmdPlayerShot(networkIdentity, currentWeapon.damage);
			}

			//// We hit something, call the OnHit method on the server
			CmdOnHit(_hit.point, _hit.normal);
		}

		if (currentWeapon.bullets <= 0)
		{
			weaponManager.Reload();
		}

	}

	public void OnReloadComplete()
	{
		ClientOnAmmoUpdated?.Invoke(currentWeapon.bullets, currentWeapon.maxBullets);
	}

	[ClientRpc]
	void RpcPlayerShot()
	{
        //shootingAudioSource.PlayOneShot(currentWeapon.GetRandomShotSound());
    }

    #endregion

    #region Inputs

    public void OnStartAiming() {
        //gun.IsAiming = true;
    }

    public void OnStopAiming() {
        //gun.IsAiming = false;
    }

    public void OnPressReload() {
        currentWeapon = weaponManager.GetCurrentWeapon();
        if (currentWeapon.bullets < currentWeapon.maxBullets)
            weaponManager.Reload();
    }

    public void OnStartShooting() {
        currentWeapon = weaponManager.GetCurrentWeapon();

		if (currentWeapon.fireRate <= 0f)
		{
			Shoot();
			return;
		}

		if (currentWeapon.fireRate > 0f)
		{
			InvokeRepeating(nameof(Shoot), 0f, 1f / currentWeapon.fireRate);
		}
	}

    public void OnStopShooting() {
        CancelInvoke(nameof(Shoot));
    }

    #endregion

}
