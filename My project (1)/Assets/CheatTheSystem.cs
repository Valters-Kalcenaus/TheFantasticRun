using UnityEngine;

public class CheatTheSystem : MonoBehaviour
{
    public TimeController timeController;
    public bool hasPlayedSound = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPlayedSound)
        {
            timeController.PlayCheatTheSystemSound();
            hasPlayedSound = true;
        }
    }

    public void ResetSoundFlag()
    {
        hasPlayedSound = false;
    }
}
