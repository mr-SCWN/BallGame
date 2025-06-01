using UnityEngine;

public class DoorUpKeyController : MonoBehaviour
{
    [Header("Door Up Trigger settings")]
    [Tooltip("Player Object Tag")]
    public string playerTag = "Player";

    [Tooltip("Link to the DoorMoveUpController (the root of the door)")]
    public DoorMoveUpController doorController;

    [Tooltip("Reset the key after opening (true/false)")]
    public bool consumeKey = true;

    private void OnTriggerEnter(Collider other)
    {
        //checking that the player has entered the trigger
        if (!other.CompareTag(playerTag)) return;

        // trying to find the player's PlayerInventory component.
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory != null && inventory.hasKey)
        {
            // Запускаем подъём двери
            if (doorController != null)
                doorController.StartOpening();
            else
                Debug.LogWarning("DoorUpKeyController: not assigned DoorMoveUpController!");

            // If it is necessary to "use" the key
            if (consumeKey)
                inventory.hasKey = false;

            // turn off the trigger so that the door does not open again.
            GetComponent<Collider>().enabled = false;
        }
        else
        {
            //  player have no key
            Debug.Log("The door is locked. A key is required.");
        }
    }
}
