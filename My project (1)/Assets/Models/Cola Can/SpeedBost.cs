using UnityEngine;
using StarterAssets;

public class SpeedBost : MonoBehaviour
{
    public AudioClip consumptionSound;
    public float speedMultiplier = 1.5f; 
    public float boostDuration = 10f; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);

            AudioSource.PlayClipAtPoint(consumptionSound, transform.position);

            other.GetComponent<FirstPersonController>().IncreaseSpeed(speedMultiplier, boostDuration);

            
        }
    }
}

