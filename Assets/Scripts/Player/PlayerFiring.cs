using System.Collections;
using UnityEngine;
using Mirror;
using System;

public class PlayerFiring : NetworkBehaviour
{
    [SerializeField]
    private Camera playerCamera;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField] private AudioSource firingAudioSource;
    [SerializeField] private WeaponProceduralFireFakeRecoil weaponRecoil;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private CameraShake cameraShake;

    public event Action<int, int> ClientOnAmmoUpdated;
    public event Action ClientOnFired;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;
    private Player player;

    public PlayerWeapon GetCurrentWeapon() => currentWeapon;

    private float nextFireTime;
    private Coroutine automaticFireCoroutine;

    void Start()
    {
        if (playerCamera == null)
        {
            Debug.LogError("PlayerFire: No camera referenced!");
            this.enabled = false;
        }

        player = GetComponent<Player>();
        weaponManager = GetComponent<WeaponManager>();
        currentWeapon = weaponManager.GetCurrentWeapon();

        ClientOnAmmoUpdated?.Invoke(currentWeapon.bullets, currentWeapon.maxBullets);
    }

    #region Server

    //Is called on the server when a player fires
    [Command]
    void CmdOnFire(PlayerFiring firingPlayer)
    {
        RpcDoFireEffect(firingPlayer);
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
    // a fire effect
    [ClientRpc]
    void RpcDoFireEffect(PlayerFiring firingPlayer)
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();

        if (currentWeapon.UseSounds.Count > 0)
            firingPlayer.firingAudioSource.PlayOneShot(currentWeapon.GetRandomShotSound());
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
    void Fire()
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

        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + 1f / currentWeapon.fireRate;

        ClientOnFired?.Invoke();

        currentWeapon.bullets--;
        ClientOnAmmoUpdated?.Invoke(currentWeapon.bullets, currentWeapon.maxBullets);

        //Debug.Log("Remaining bullets: " + currentWeapon.bullets);

        weaponRecoil.Fire();
        //firingAudioSource.PlayOneShot(currentWeapon.GetRandomShotSound());

        if (cameraShake && cameraShake.isActiveAndEnabled)
            cameraShake.ShakeCamera(currentWeapon.fireCameraShakeIntensity, currentWeapon.fireCameraShakeTime);

        //We are firing, call the OnFire method on the server
        CmdOnFire(this);

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
        //firingAudioSource.PlayOneShot(currentWeapon.GetRandomShotSound());
    }

    #endregion

    #region Inputs

    public void OnStartAiming()
    {
        //gun.IsAiming = true;
    }

    public void OnStopAiming()
    {
        //gun.IsAiming = false;
    }

    public void OnPressReload()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();
        if (currentWeapon.bullets < currentWeapon.maxBullets)
            weaponManager.Reload();
    }

    public void OnStartFiring()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (currentWeapon.weaponType == WeaponTypes.Single || currentWeapon.weaponType == WeaponTypes.Semi)
        {
            Fire();
            return;
        }

        if (currentWeapon.fireRate <= 0f)
        {
            Fire();
            return;
        }

        //Fire();

        automaticFireCoroutine = StartCoroutine(FireAutomaticWeapon());

        //if (currentWeapon.fireRate > 0f)
        //{
        //    InvokeRepeating(nameof(Fire), 0f, 1f / currentWeapon.fireRate);
        //}
    }

    IEnumerator FireAutomaticWeapon()
    {
        while (true)
        {
            Fire();

            yield return new WaitUntil(() => Time.time >= nextFireTime);
        }
    }

    public void OnStopFiring()
    {
        //CancelInvoke(nameof(Fire));

        if (automaticFireCoroutine != null)
            StopCoroutine(automaticFireCoroutine);
    }

    #endregion

}
