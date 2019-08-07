using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI's controller's singleton.
/// </summary>
public class UIController : MonoBehaviour
{

    /// <summary>
    /// Singleton's instance.
    /// </summary>
    public static UIController instance;
    UIController() { instance = this; }

    // UI elements
    public Animation PlayButtonAnimation;
    public GameObject GameOverScreen;
    public Text GameOverScreenText;

    /// <summary>
    /// Flag determining if player has started gameplay
    /// </summary>
    public bool GameStarted = false;

    /// <summary>
    /// The time since player has clicked start button.
    /// </summary>
    public float TimeSinceGameStarted = 0f;

    /// <summary>
    /// Flag determining if game has began interactive and responsive
    /// </summary>
    public bool GameInteractive = false;

    /// <summary>
    /// Flag determining if player has crashed
    /// </summary>
    public bool GameOver = false;

    /// <summary>
    /// Main menu play button's click callback.
    /// </summary>
    public void PlayButtonClicked()
    {
        if (!GameStarted)
        {
            PlayButtonAnimation.Play();
            ShapeController.instance.GetComponent<Animation>().Play();
            PlayerController.instance.GetComponent<Animation>().Play();
            UICollectedFoldersController.instance.GetComponent<Animation>().Play();
            GameStarted = true;
        }
    }

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStarted)
        {
            TimeSinceGameStarted += Time.deltaTime;
            if (TimeSinceGameStarted > 3)
                GameInteractive = true;
        }
    }

    /// <summary>
    /// Method used to fire Game Over state
    /// </summary>
    public void FireGameOver()
    {
        if(!GameOver)
        {
            GameOver = true;
            GameInteractive = false;
            PlayerController.instance.Speed = 0;
            PlayerController.instance.JumpSpeed = 0;
            StartCoroutine(GameoverCoroutine());
        }
    }
    IEnumerator GameoverCoroutine()
    {
        GameOverScreen.SetActive(true);

        string GameOverString = GameOverScreenText.text;
        int stringDividedPartLength = GameOverString.Length / 10;
        GameOverScreenText.text = "";
        int iterator = 0;
        while (!GameOverScreenText.text.Equals(GameOverString))
        {
            int startIndex = iterator * stringDividedPartLength;
            int length = ((startIndex + stringDividedPartLength) > GameOverString.Length) ? (GameOverString.Length - startIndex) : stringDividedPartLength;
            GameOverScreenText.text += GameOverString.Substring(startIndex, length);
            iterator++;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(2);

        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}