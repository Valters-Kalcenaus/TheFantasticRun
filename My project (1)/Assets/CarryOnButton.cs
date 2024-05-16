
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarryOnButton : MonoBehaviour
{
    public void ToStartMenu()
    {
        Debug.Log("ToStartMenu method called.");
        SceneManager.LoadScene("Startmenu");
    }
}

