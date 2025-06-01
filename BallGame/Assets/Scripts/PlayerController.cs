using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Motion Settings")]
    [Tooltip("The speed of the ball movement")]
    public float moveSpeed = 5f;

    private Rigidbody rb;

    private void Awake()
    {
        //  Getting the Rigidbody component when starting the scene
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        
        float moveX = Input.GetAxis("Horizontal"); // A/D or left/right arrows
        float moveZ = Input.GetAxis("Vertical");   // W/S or forward/backward arrows

        // Forming the direction vector
        Vector3 moveVector = new Vector3(moveX, 0f, moveZ).normalized;

       //   movement by changing the speed of the Rigidbody
        Vector3 velocity = moveVector * moveSpeed;
        velocity.y = rb.linearVelocity.y; 
        rb.linearVelocity = velocity;
    }
}
