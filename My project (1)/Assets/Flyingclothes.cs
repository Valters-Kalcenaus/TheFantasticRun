using UnityEngine;

public class Flyingclothes : MonoBehaviour
{
    public float amplitude = 5f; 
    public float cycleTime = 2f; 
    private Vector3 originalPosition;
    private float timer;

    void Start()
    {
        originalPosition = transform.position;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime; 

        float progress = (timer % cycleTime) / cycleTime;

        float newY;
        if (progress < 0.5f)
        {
            newY = Mathf.Lerp(originalPosition.y, originalPosition.y + amplitude, progress * 2);
        }
        else
        {
            newY = Mathf.Lerp(originalPosition.y + amplitude, originalPosition.y, (progress - 0.5f) * 2);
        }

        transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);
    }




}
