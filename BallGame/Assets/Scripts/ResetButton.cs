using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetButton : MonoBehaviour
{
    [Header("Tag of the player object")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        // only player trigger
        if (!other.CompareTag(playerTag))
            return;

        // reset scene:
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
       
    }
}
