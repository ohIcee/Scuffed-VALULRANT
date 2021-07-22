using System.Collections;
using UnityEngine;
using Mirror;

public class PlayerShooting : NetworkBehaviour
{

    [Header("Properties")]
    // these will be replaced with gun info
    [SerializeField] private float shootRange;
    [SerializeField] private float shotDamage;
    [SerializeField] private float fireRate;

    private bool isShooting = false;

    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);    // shot line show duration
    private LineRenderer laserLine;
    private float nextFire;

    private Camera playerCamera;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        laserLine = GetComponent<LineRenderer>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    // automatic weapon
    private void Update()
    {
        if (!hasAuthority || !isShooting || Time.time < nextFire) return;

        laserLine.SetPosition(0, playerCamera.transform.position);

        nextFire = Time.time + fireRate;

        StartCoroutine(ShotEffect());

        Vector3 rayOrigin = playerCamera.ViewportToWorldPoint(new Vector3(.5f, .5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, playerCamera.transform.forward, out hit, shootRange))
        {
            laserLine.SetPosition(1, hit.point);

            // do damage
            Debug.Log($"Hit {hit.transform.name}");
        }
        else {
            laserLine.SetPosition(1, rayOrigin + (playerCamera.transform.forward * shootRange));
        }
    }

    private IEnumerator ShotEffect() {
        // play gunshot sound

        laserLine.enabled = true;
        
        yield return shotDuration;
        
        laserLine.enabled = false;
    }

    public void OnStartShooting() {
        isShooting = true;
    }

    public void OnStopShooting() {
        isShooting = false;
    }

}
