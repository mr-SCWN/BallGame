using System.Collections;
using UnityEngine;

public class DoorUpController : MonoBehaviour
{
    [Header("Opening settings")]
    [Tooltip("Angle of rotation (in degrees) to open the door to the right")]
    public float openAngle = 90f;
    [Tooltip("Rotation speed (degrees per second)")]
    public float openSpeed = 120f;

    private bool isOpening = false;
    private float currentAngle = 0f;

    // Method for starting the opening animation
    public void StartOpening()
    {
        if (!isOpening)
            StartCoroutine(OpenDoorCoroutine());
    }

    private IEnumerator OpenDoorCoroutine()
    {
        isOpening = true;

        // Rotate the door around its local Y axis until currentAngle reaches openAngle
        while (currentAngle < openAngle)
        {
            float delta = openSpeed * Time.deltaTime;
            float angleToRotate = Mathf.Min(delta, openAngle - currentAngle);

            // Rotate around local Y (0, angleToRotate, 0)
            transform.Rotate(0f, angleToRotate, 0f, Space.Self);

            currentAngle += angleToRotate;
            yield return null;
        }

        isOpening = false;
    }
}
