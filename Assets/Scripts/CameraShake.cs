using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera playerCamera;
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;

    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin 
            = playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }

    private void Update()
    {
        if (shakeTimer > 0) {
            shakeTimer -= Time.deltaTime;
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin
                = playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain
                = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
        }
    }

}