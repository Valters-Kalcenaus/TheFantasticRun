using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TimeController : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public GameObject timerCanva;
    private float timeRemaining = 40;
    private bool timerStarted = true;

    public Animator leftCurtainAnimator;
    public Animator rightCurtainAnimator;

    public AudioSource introSentenceSource;
    public AudioSource cheatTheSystemSource;
    public AudioSource youLostSource;
    public AudioSource youWonSource;
    public AudioSource backgroundMusicSource;
    public CheatTheSystem CheatTheSystem;

    public GameObject viewBlocker;


    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnEnable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnDisable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    void PlaySound(AudioSource audioSource)
    {
        audioSource.Play();
    }

    void Update()
    {
        if (timerStarted && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
        }
        else if (timerStarted && timeRemaining <= 0.2)
        {
            timerStarted = false;
            timerCanva.SetActive(false);

            leftCurtainAnimator.SetTrigger("OpenCurtain");
            rightCurtainAnimator.SetTrigger("OpenCurtain");
            DiableViewBlock();

            PlaySound(youLostSource);
            Invoke("GameOver", 16.0f);

        }


        if (timeRemaining < 10)
        {
            CheatTheSystem.hasPlayedSound = true;
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartTimer()
    {
        timerStarted = true;
    }

    public void StopTimer()
    {
        timerStarted = false;
    }

    void GameOver()
    {
        SceneManager.LoadScene("YouLOST");
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    public void PlayCheatTheSystemSound()
    {
        PlaySound(cheatTheSystemSource);
    }

    public void PlayYouWonSound()
    {
        PlaySound(youWonSource);
    }

    void DiableViewBlock()
    {
        if (viewBlocker != null)
        {
            viewBlocker.SetActive(false);
        }
    }

}
