using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineCamera cam;
    private CinemachineBasicMultiChannelPerlin camNoise;

    public float shakeIntensity = 3f; // How strong the shake is
    public float shakeDuration = 0.04f; // How long the shake lasts

    void Start()
    {
        cam = GetComponent<CinemachineCamera>();

        // Use GetComponent instead of GetCinemachineComponent
        camNoise = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();

    }

    public void ShakeCamera()
    {
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        camNoise.AmplitudeGain = shakeIntensity; // Corrected property
        yield return new WaitForSeconds(shakeDuration);
        camNoise.AmplitudeGain = 0f; // Reset shake after duration
    }
}
