using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    private Vector2 mouseInput;

    [Header("Position")]
    public float amount = .02f;
    public float maxAmount = .06f;
    public float smoothAmount = 6f;

    [Header("Rotation")]
    public float rotationAmount = 4f;
    public float maxRotationAmount = 5f;
    public float smoothRotation = 12f;

    [Space]

    public bool rotationX = true;
    public bool rotationY = true;
    public bool rotationZ = true;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float inputX;
    private float inputY;

    private SettingsManager settingsManager;

    private void Start()
    {
        settingsManager = FindObjectOfType<SettingsManager>();

        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        calculateSway();

        moveSway();
        tiltSway();
    }

    private void moveSway() {
        float moveX = Mathf.Clamp(inputX * amount, -maxAmount, maxAmount);
        float moveY = Mathf.Clamp(inputY * amount, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(moveX, moveY, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
    }

    private void tiltSway() {
        float tiltY = Mathf.Clamp(inputX * rotationAmount, -maxRotationAmount, maxRotationAmount);
        float tiltX = Mathf.Clamp(inputY * rotationAmount, -maxRotationAmount, maxRotationAmount);

        Quaternion finalRotation = Quaternion.Euler(new Vector3(
            rotationX ? -tiltX : 0f, 
            rotationY ? tiltY : 0f, 
            rotationZ ? tiltY : 0f
            ));

        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * initialRotation, Time.deltaTime * smoothRotation);
    }

    private void calculateSway() {
        inputX = -mouseInput.x * settingsManager.GetMouseSensitivity();
        inputY = -mouseInput.y * settingsManager.GetMouseSensitivity();
    }

    public void ReceiveInput(Vector2 mouseInput) {
        this.mouseInput = mouseInput;
    }
}
