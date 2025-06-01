using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Which object to monitor")]
    public Transform player;    

    [Header("The offset of the camera relative to the play")]
    public Vector3 offset = new Vector3(0f, 10f, -10f);

    [Header("The speed of smoothing camera movement")]
    [Tooltip("The more, the faster the camera catches up with the player.")]
    public float smoothSpeed = 5f;

    private void LateUpdate()
    {
        if (player == null) return;

        // calculate the desired camera position
        Vector3 desiredPosition = player.position + offset;

        // smoothly interpolate the current camera position
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position, 
            desiredPosition, 
            smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;

        // make the camera always look at the play
        transform.LookAt(player);
    }
}
