using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("SampleScene"); // Or use SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
