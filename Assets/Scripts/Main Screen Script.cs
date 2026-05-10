using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainScreenScript : MonoBehaviour
{
    [Header("Navigation Buttons")]
    [SerializeField] private Button quitButton;
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    [SerializeField] private Button level4Button;


    [Header("Sound Buttons")]
    [SerializeField] private Button soundOnButton;
    [SerializeField] private Button soundOffButton;

    [Header("Audio")]
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource buttonClick;

    // Constants
    private const string SOUND_ENABLED_KEY = "SoundEnabled";
    private const float BUTTON_DELAY = 0.2f;

    private void Start()
    {
        EnsureLevel4Button();

        // Navigation buttons
        quitButton.onClick.AddListener(QuitGame);
        level1Button.onClick.AddListener(Level1);
        level2Button.onClick.AddListener(Level2);
        level3Button.onClick.AddListener(Level3);
        if (level4Button != null)
            level4Button.onClick.AddListener(Level4);


        // Sound buttons - DIRECT APPROACH
        soundOnButton.onClick.AddListener(() => {
            // Turn sound ON
            music.Play();
            buttonClick.Play();
            PlayerPrefs.SetInt(SOUND_ENABLED_KEY, 1);

            // Hide ON button, Show OFF button
            soundOnButton.gameObject.SetActive(false);
            soundOffButton.gameObject.SetActive(true);
        });

        soundOffButton.onClick.AddListener(() => {
            // Turn sound OFF
            music.Stop();
            buttonClick.Play();
            PlayerPrefs.SetInt(SOUND_ENABLED_KEY, 0);

            // Show ON button, Hide OFF button
            soundOnButton.gameObject.SetActive(true);
            soundOffButton.gameObject.SetActive(false);
        });

        // Set initial state
        bool soundEnabled = PlayerPrefs.GetInt(SOUND_ENABLED_KEY, 1) == 1;

        if (soundEnabled)
        {
            music.Play();
            soundOnButton.gameObject.SetActive(false);
            soundOffButton.gameObject.SetActive(true);
        }
        else
        {
            music.Stop();
            soundOnButton.gameObject.SetActive(true);
            soundOffButton.gameObject.SetActive(false);
        }
    }

    public void QuitGame()
    {
        StartCoroutine(QuitWithDelay());
    }

    private IEnumerator QuitWithDelay()
    {
        if (PlayerPrefs.GetInt(SOUND_ENABLED_KEY, 1) == 1)
            buttonClick.Play();

        yield return new WaitForSeconds(BUTTON_DELAY);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Level1()
    {
        StartCoroutine(LoadSceneWithDelay(1));
    }

    public void Level2()
    {
        StartCoroutine(LoadSceneWithDelay(2));
    }
    public void Level3()
    {
        StartCoroutine(LoadSceneWithDelay(3));
    }

    public void Level4()
    {
        StartCoroutine(LoadSceneWithDelay(4));
    }

    private void EnsureLevel4Button()
    {
        if (level4Button != null || level3Button == null) return;

        level4Button = Instantiate(level3Button, level3Button.transform.parent);
        level4Button.name = "Level4Button";
        level4Button.onClick.RemoveAllListeners();

        RectTransform rectTransform = level4Button.GetComponent<RectTransform>();
        if (rectTransform != null)
            rectTransform.anchoredPosition += new Vector2(0f, -140f);

        TMP_Text tmpText = level4Button.GetComponentInChildren<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = "Level 4";
            return;
        }

        Text uiText = level4Button.GetComponentInChildren<Text>();
        if (uiText != null)
            uiText.text = "Level 4";
    }


    private IEnumerator LoadSceneWithDelay(int sceneIndex)
    {
        if (PlayerPrefs.GetInt(SOUND_ENABLED_KEY, 1) == 1)
            buttonClick.Play();

        yield return new WaitForSeconds(BUTTON_DELAY);
        SceneManager.LoadSceneAsync(sceneIndex);
    }
}
