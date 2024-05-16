
using UnityEngine;
using UnityEngine.SceneManagement;

public class PromiseButton : MonoBehaviour
{
    public void ToGameOver()
    {
        Debug.Log("I made my promise");
        SceneManager.LoadScene("YouLOST");
    }
}

