
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToOFscreen : MonoBehaviour
{
    public void ToOF()
    {
        Debug.Log("If you see this then we need to get you some help");
        SceneManager.LoadScene("OfPage");
    }
}

