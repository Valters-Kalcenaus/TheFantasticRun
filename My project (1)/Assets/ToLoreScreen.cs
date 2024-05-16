using UnityEngine;
using UnityEngine.SceneManagement;

public class ToLoreScreen : MonoBehaviour
{
    public void PlayLore()
    {
        Debug.Log("No turning back now");  
        SceneManager.LoadScene("Lore");  
    }
}
