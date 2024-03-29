﻿using UnityEngine;
using Cinemachine;
using Mirror;

public class FirstPersonLook : NetworkBehaviour
{
    [Header("Values")]
    #region mouse look
    public float sensitivity = 0.15f;
    public float smoothing = 2f;
    public bool cursorLock = false;

    Vector2 mouseInput;
    Vector2 currentMouseLook;
    Vector2 appliedMouseDelta;
    #endregion

    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private Transform character;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private Transform cameraTransform;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        cameraTransform.GetComponent<CinemachineVirtualCamera>().enabled = true;
        audioListener.enabled = true;
    }

    [ClientCallback]
    void Update()
    {
        if (cursorLock && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (!cursorLock && Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        // Get smooth mouse look.
        Vector2 smoothMouseDelta = Vector2.Scale(new Vector2(mouseInput.x, mouseInput.y), Vector2.one * sensitivity * smoothing);
        appliedMouseDelta = Vector2.Lerp(appliedMouseDelta, smoothMouseDelta, 1 / smoothing);
        currentMouseLook += appliedMouseDelta;
        currentMouseLook.y = Mathf.Clamp(currentMouseLook.y, -90, 90);

        // Rotate camera and controller.
        cameraTransform.localRotation = Quaternion.AngleAxis(-currentMouseLook.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(currentMouseLook.x, Vector3.up);
    }

    public void ReceiveInput(Vector2 mouseInput) => this.mouseInput = mouseInput;
}