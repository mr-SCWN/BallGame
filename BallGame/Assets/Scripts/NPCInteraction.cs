using UnityEngine;
using TMPro;  

public class NPCInteraction : MonoBehaviour
{
    [Header("Tag of the player object")]
    public string playerTag = "Player";

    [Header("Interaction key")]
    public KeyCode interactionKey = KeyCode.E;

    [Header("Reference to DialogueManager (assign in Inspector)")]
    public DialogueManager dialogueManager;

    [Header("UI hint to show when player is in range")]
    public GameObject interactHint;

        private void Awake()
    {
        
        if (interactHint != null)
            interactHint.SetActive(false);
    }
    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            if (interactHint != null)
                interactHint.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            if (interactHint != null)
                interactHint.SetActive(false);
        }
    }

    private void Update()
    {
        // If player is in range and pressed the interaction key, start dialogue
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            if (dialogueManager != null)
            {
                
                if (interactHint != null)
                    interactHint.SetActive(false);

                dialogueManager.StartDialogue();
            }
            else
            {
                Debug.LogWarning("DialogueManager is not assigned in NPCInteraction!");
            }
        }
    }
}
