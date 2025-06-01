using System.Collections;
using UnityEngine;

public class DoorMoveUpController : MonoBehaviour
{
    [Header("Door lift settings")]
    [Tooltip("How high (in local units) should the door be raised?")]
    public float raiseHeight = 3f;

    [Tooltip("Lifting speed (units per second)")]
    public float raiseSpeed = 2f;

    [Header("Audio (optional)")]
    [Tooltip("Door sound source")]
    public AudioSource audioSource;

    [Tooltip("Lifting sound clip")]
    public AudioClip raiseClip;

    private bool isOpening = false;
    private Vector3 startPos;
    private float raisedAmount = 0f;

    private void Awake()
    {
        // Memorize the initial position of the door
        startPos = transform.localPosition;
    }

    // will be called from outside 
    public void StartOpening()
    {
        if (isOpening) return;

        // We play the lifting sound
        if (audioSource != null && raiseClip != null)
            audioSource.PlayOneShot(raiseClip);

        StartCoroutine(RaiseDoorCoroutine());
    }

    private IEnumerator RaiseDoorCoroutine()
    {
        isOpening = true;

        // We haven't reached the required height
        while (raisedAmount < raiseHeight)
        {
            // How much will we "walk" in this frame
            float delta = raiseSpeed * Time.deltaTime;
            float moveStep = Mathf.Min(delta, raiseHeight - raisedAmount);

            // Moving the door up 
            transform.localPosition += Vector3.up * moveStep;
            raisedAmount += moveStep;

            yield return null;
        }

        isOpening = false;
    }
}
