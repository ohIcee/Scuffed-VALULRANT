using UnityEngine;
using System.Collections;

public class WeaponRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    public float rotationSpeed = 6f;
    public float returnSpeed = 25f;
    [Space()]

    [Header("Hipfire")]
    public Vector3 RecoilRotation = new Vector3(2f, 2f, 2f);
    [Space()]

    [Header("Aiming")]
    public Vector3 RecoilRotationAiming = new Vector3(.5f, .5f, .5f);
    [Space()]

    [Header("State")]
    public bool aiming;

    private Vector3 currentRotation;
    private Vector3 rot;

    private void FixedUpdate()
    {
        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        rot = Vector3.Slerp(rot, currentRotation, rotationSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(rot);
    }

    public void Fire()
    {
        if (aiming)
        {
            currentRotation += new Vector3(-RecoilRotationAiming.x, Random.Range(-RecoilRotationAiming.y, RecoilRotationAiming.y), Random.Range(-RecoilRotationAiming.z, RecoilRotationAiming.z));
        }
        else 
        { 
            currentRotation += new Vector3(-RecoilRotation.x, Random.Range(-RecoilRotation.y, RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
        }
    }
}