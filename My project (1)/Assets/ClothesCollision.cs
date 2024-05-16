using UnityEngine;
using UnityEngine.SceneManagement;

public class ClothesCollision : MonoBehaviour
{
    public TimeController timeController;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (timeController != null)
            {
                if (timeController.GetTimeRemaining() > 0)
                {
                    gameObject.SetActive(false);
                    Invoke("GameOverWin", 8.0f);
                    timeController.StopTimer();
                    timeController.PlayYouWonSound();
                }
                else
                {
                    Debug.Log("Time is up!");
                }
            }
        }
    }

    void GameOverWin()
    {
        SceneManager.LoadScene("YouWON");
    }
}
