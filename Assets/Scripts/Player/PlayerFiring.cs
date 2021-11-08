using UnityEngine;
using Mirror;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class PlayerFiring : NetworkBehaviour
{
    [SerializeField]
    private Camera playerCamera;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField] private AudioSource firingAudioSource;

    [SerializeField] private AudioSource hitPlayerAudioSource;
    [SerializeField] private AudioSource killPlayerAudioSource;
    [SerializeField] private List<AudioClip> playerHitSFXs;
    [SerializeField] private List<AudioClip> playerKillSFXs;

    [SerializeField] private WeaponProceduralFireFakeRecoil weaponRecoil;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private CameraShake cameraShake;

    public event Action<int, int> ClientOnAmmoUpdated;

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
    private void CmdOnHit(Vector3 _pos, Vector3 _normal)
    {
        RpcDoHitEffect(_pos, _normal);
    }

    [Command]   //// _playerIdentity -> hit player identity
    private void CmdPlayerShot(NetworkIdentity _playerIdentity, int _damage)
    {
        RpcPlayerShot();

        // Return if we hit our own unit
        if (_playerIdentity.connectionToClient == connectionToClient) return;

        if (_playerIdentity.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            // Calculate damage based on kevlar
            PlayerEquipment playerEquipment = playerHealth.GetComponent<PlayerEquipment>();
            int currentArmor = playerEquipment.GetKevlarDurability();
            currentArmor -= (int)(_damage * playerEquipment.GetDamageDecreaseMultiplier());
            int extraDamage = currentArmor < 0 ? Mathf.Abs(currentArmor) : 0;

            TargetCreateDamageIndicator(_playerIdentity.connectionToClient, transform);

            playerHealth.DealDamage(_damage + extraDamage, player.GetNetworkPlayer());
        }
    }

    #endregion

    #region Client

    [TargetRpc]
    void TargetCreateDamageIndicator(NetworkConnection target, Transform shooterPlayerTransform)
    {
        ValulrantNetworkPlayer networkPlayer = target.identity.GetComponent<ValulrantNetworkPlayer>();
        DamageIndicatorSystem.CreateIndicator(shooterPlayerTransform);
    }

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

    public void OnKilledPlayer()
    {
        killPlayerAudioSource.PlayOneShot( Util.GetRandomAudioClip(playerKillSFXs) );        
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

        currentWeapon.bullets--;
        ClientOnAmmoUpdated?.Invoke(currentWeapon.bullets, currentWeapon.maxBullets);

        weaponRecoil.Fire();

        //if (cameraShake && cameraShake.isActiveAndEnabled)
        //    cameraShake.ShakeCamera(currentWeapon.fireCameraShakeIntensity, currentWeapon.fireCameraShakeTime);

        //We are firing, call the OnFire method on the server
        CmdOnFire(this);

        RaycastHit[] hits = Physics.RaycastAll(playerCamera.transform.position, playerCamera.transform.forward, currentWeapon.range, layerMask).OrderBy(h => h.distance).ToArray();
        if (hits != null && hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];

                // If we hit a player object go to the next one
                // (We're looking for a limb)
                if (hit.transform.TryGetComponent(out CharacterController controller)) continue;

                if (hit.transform.TryGetComponent(out PlayerLimb hitLimb))
                {
                    // If we hit our own limb, skip to the next hit
                    if (hitLimb.GetOwningPlayer() == player) continue;

                    CmdOnHit(hit.point, hit.normal);

                    hitPlayerAudioSource.PlayOneShot( Util.GetRandomAudioClip(playerHitSFXs) );

                    CmdPlayerShot(hitLimb.GetOwningPlayer().GetNetworkIdentity(), (int)(currentWeapon.damage * hitLimb.GetDamageMultiplier()));
                    break;
                }
                else
                {
                    // If it's not a player, play the hit FX and
                    // stop checking (no wallbangs)
                    CmdOnHit(hit.point, hit.normal);
                    break;
                }
            }
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
