using System.Collections;
using UnityEngine;

public class DoorHingeController : MonoBehaviour
{
    [Header("Door hinge opening settings")]
    [Tooltip("Rotation angle of the door (in degrees). Positive value → opens outward (to the right)")]
    public float openAngleY = -90f;

    [Tooltip("Rotation speed (degrees per second)")]
    public float openSpeed = 120f;

    [Header("Audio (optional)")]
    [Tooltip("Audio source attached to the door")]
    public AudioSource audioSource;

    [Tooltip("Sound clip for door opening")]
    public AudioClip openClip;

    private bool isOpening = false;
    private float currentAngle = 0f;

    // Called externally to start the opening animation
    public void StartOpening()
    {
        if (isOpening) return;

        // Play sound if specified
        if (audioSource != null && openClip != null)
        {
            audioSource.PlayOneShot(openClip);
        }

        StartCoroutine(OpenDoorCoroutine());
    }

    private IEnumerator OpenDoorCoroutine()
    {
        isOpening = true;

        // Rotate around the local Y axis until full openAngleY is reached
        float targetAngle = Mathf.Abs(openAngleY);
        while (currentAngle < targetAngle)
        {
            // Degrees to rotate this frame
            float delta = openSpeed * Time.deltaTime;
            float angleStep = Mathf.Min(delta, targetAngle - currentAngle);

            // Rotate Y 
            transform.Rotate(0f, angleStep * Mathf.Sign(openAngleY), 0f, Space.Self);

            currentAngle += angleStep;
            yield return null;
        }

        isOpening = false;
    }
}
