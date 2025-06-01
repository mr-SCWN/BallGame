using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag of a player object that can pick up a key")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        // trigger for player 
        if (other.CompareTag(playerTag))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                // giving key
                inventory.hasKey = true;
                Debug.Log("Key picked up!");
            }
            else
            {
                Debug.LogWarning("PlayerInventory not found on Player object!");
            }

            // destroy key
            Destroy(gameObject);
        }
    }
}
