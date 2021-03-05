using System.Collections;
using UnityEngine;
using Mirror;

public class PlayerShooting : NetworkBehaviour
{

    [Header("Properties")]
    // these will be replaced with gun info
    [SerializeField] private float shootRange;
    [SerializeField] private int shotDamage = 20;
    [SerializeField] private float fireRate;

    private bool isShooting = false;

    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);    // shot line show duration
    [SyncVar] private float nextFire;

    private Camera playerCamera;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        playerCamera = GetComponentInChildren<Camera>();
    }

    // automatic weapon
    private void Update()
    {
        if (!hasAuthority || !isShooting || Time.time < nextFire) return;

        CmdTryShoot(playerCamera.transform.position, playerCamera.transform.forward);
    }

    [Command]
    private void CmdTryShoot(Vector3 clientCameraPosition, Vector3 clientCameraForward) {
        // Server side check

        if (Time.time < nextFire) return;

        nextFire = Time.time + fireRate;

        Ray ray = new Ray(clientCameraPosition, clientCameraForward * shootRange);
        Debug.DrawRay(clientCameraPosition, clientCameraForward * shootRange, Color.red, .3f);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            // if we hit a networked player
            if (hit.transform.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity)) {
                if (networkIdentity.connectionToClient == connectionToClient) return;

                if (hit.transform.TryGetComponent<Health>(out Health health)) {
                    health.DealDamage(shotDamage);
                }
            }
        }
    }

    public void OnStartShooting() {
        isShooting = true;
    }

    public void OnStopShooting() {
        isShooting = false;
    }

}
