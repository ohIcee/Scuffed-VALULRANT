using UnityEngine;
using System.Collections;

public class WeaponProceduralFireFakeRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    public float rotationSpeed = 6f;
    public float returnSpeed = 25f;
    [Space()]

    public Vector3 RecoilRotation = new Vector3(2f, 2f, 2f);

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
        currentRotation += new Vector3(-RecoilRotation.x, Random.Range(-RecoilRotation.y, RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
    }
}