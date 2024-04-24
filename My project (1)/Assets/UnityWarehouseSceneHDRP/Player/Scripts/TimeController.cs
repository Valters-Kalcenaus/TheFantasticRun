
using UnityEngine;
using TMPro;

public class TimeController : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float timeRemaining = 10;
    private bool timerHasEnded = false; 

    public Animator leftCurtainAnimator;
    public Animator rightCurtainAnimator;

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
        }
        else if (!timerHasEnded)
        {
            Debug.Log("Time's Up!");
            timeRemaining = 0;
            timerHasEnded = true;

            leftCurtainAnimator.SetTrigger("OpenCurtain");
            rightCurtainAnimator.SetTrigger("OpenCurtain");
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1; 

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}


