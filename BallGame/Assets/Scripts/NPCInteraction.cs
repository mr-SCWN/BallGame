using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("Tag of the player object")]
    public string playerTag = "Player";

    [Header("Interaction key")]
    public KeyCode interactionKey = KeyCode.E;

    [Header("Reference to DialogueManager (assign in Inspector)")]
    public DialogueManager dialogueManager;

    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        // If player is in range and pressed the interaction key, start dialogue
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            if (dialogueManager != null)
            {
                dialogueManager.StartDialogue();
            }
            else
            {
                Debug.LogWarning("DialogueManager is not assigned in NPCInteraction!");
            }
        }
    }
}
