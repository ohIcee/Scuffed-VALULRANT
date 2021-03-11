using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{

    //private float lastFireTime;

    //[SerializeField] private PlayerFiring playerFiring;
    //[SerializeField] private Transform playerCamera;

    //private float currentVerticalRecoil = 0f;

    //private float currentRecoilRecoveryTime = 0f;

    //private void OnEnable()
    //{
    //    playerFiring.ClientOnFired += OnShotFired;
    //}

    //private void OnDisable()
    //{
    //    playerFiring.ClientOnFired -= OnShotFired;
    //}

    //private void Update()
    //{
    //    currentRecoilRecoveryTime += Time.deltaTime;
    //    PlayerWeapon currentWeapon = playerFiring.GetCurrentWeapon();

    //    if (!currentWeapon) return;

    //    if (currentRecoilRecoveryTime < currentWeapon.recoilRecoveryTime) return;

    //    if (currentVerticalRecoil < -currentWeapon.verticalRecoilPerShot) return;

    //    playerCameraRecoilWrapper.transform.Rotate(
    //        new Vector3(
    //             playerCamera.rotation.x + currentWeapon.verticalRecoilPerShot,
    //             0f,
    //             0f),
    //        Space.Self
    //        );

    //    currentVerticalRecoil -= currentWeapon.verticalRecoilPerShot;
    //}

    //private void OnShotFired()
    //{
    //    PlayerWeapon currentWeapon = playerFiring.GetCurrentWeapon();
    //    if (currentWeapon && currentWeapon.weaponType != WeaponTypes.Auto) return;

    //    lastFireTime = Time.time;

    //    playerCamera.Rotate(
    //        playerCamera.rotation.x - currentWeapon.verticalRecoilPerShot,
    //        0f,
    //        0f,
    //        Space.Self
    //        );

    //    playerCamera.Rotate(
    //        0f,
    //        Random.Range(currentWeapon.horizontalRecoilPerShot.x, currentWeapon.horizontalRecoilPerShot.y),
    //        0f,
    //        Space.Self
    //        );

    //    currentVerticalRecoil += currentWeapon.verticalRecoilPerShot;
    //    currentRecoilRecoveryTime = 0f;
    //}

}
