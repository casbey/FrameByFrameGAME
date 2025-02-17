using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineCamera cam;
    private CinemachineBasicMultiChannelPerlin camNoise;

    public float shakeIntensity = 3f; // How strong the shake is
    public float shakeDuration = 0.2f; // How long the shake lasts

    void Start()
    {
        cam = GetComponent<CinemachineCamera>();

        // Use GetComponent instead of GetCinemachineComponent
        camNoise = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();

        // Ensure noise exists and prevent unwanted shaking
        if (camNoise != null)
        {
            camNoise.AmplitudeGain = 0f; // Set to 0 at start to prevent shaking
        }
        else
        {
            Debug.LogError("CinemachineBasicMultiChannelPerlin is missing! Assign a Noise Profile.");
        }
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
