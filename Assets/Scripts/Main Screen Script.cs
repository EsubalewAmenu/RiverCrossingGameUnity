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

    [Header("Level Access")]
    [SerializeField] private GameObject[] levelLockIcons;
    [SerializeField] private TextMeshProUGUI lockedLevelMessage;
    [SerializeField] private float lockedMessageSeconds = 2f;
    [SerializeField] private Color unlockedLevelColor = Color.white;
    [SerializeField] private Color lockedLevelColor = new Color(1f, 1f, 1f, 0.45f);

    [Header("Sound Buttons")]
    [SerializeField] private Button soundOnButton;
    [SerializeField] private Button soundOffButton;

    [Header("Audio")]
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource buttonClick;

    // Constants
    private const string SOUND_ENABLED_KEY = "SoundEnabled";
    private const float BUTTON_DELAY = 0.2f;
    private const string LOCKED_LEVEL_MESSAGE = "Please complete the previous level first.";

    private GameObject[] runtimeLockIcons;
    private Coroutine lockedMessageCoroutine;

    private void Start()
    {
        LevelProgression.EnsureInitialized();

        // Navigation buttons
        quitButton.onClick.AddListener(QuitGame);
        level1Button.onClick.AddListener(() => TryLoadLevel(1));
        level2Button.onClick.AddListener(() => TryLoadLevel(2));
        level3Button.onClick.AddListener(() => TryLoadLevel(3));


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

        EnsureLockedMessage();
        RefreshLevelAccessUI();
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
        TryLoadLevel(1);
    }

    public void Level2()
    {
        TryLoadLevel(2);
    }
    public void Level3()
    {
        TryLoadLevel(3);
    }

    private void TryLoadLevel(int levelNumber)
    {
        if (!LevelProgression.IsLevelUnlocked(levelNumber))
        {
            ShowLockedLevelMessage();
            return;
        }

        StartCoroutine(LoadSceneWithDelay(GetSceneIndexForLevel(levelNumber)));
    }

    private void RefreshLevelAccessUI()
    {
        LevelProgression.EnsureInitialized();

        SetLevelButtonState(1, level1Button);
        SetLevelButtonState(2, level2Button);
        SetLevelButtonState(3, level3Button);
    }

    private void SetLevelButtonState(int levelNumber, Button button)
    {
        if (button == null) return;

        bool unlocked = LevelProgression.IsLevelUnlocked(levelNumber);
        button.interactable = unlocked;

        if (button.targetGraphic != null)
            button.targetGraphic.color = unlocked ? unlockedLevelColor : lockedLevelColor;

        GameObject lockIcon = GetLockIcon(levelNumber, button);
        if (lockIcon != null)
        {
            lockIcon.SetActive(!unlocked);
            if (!unlocked)
                EnsureLockIconFeedback(lockIcon);
        }
    }

    private GameObject GetLockIcon(int levelNumber, Button button)
    {
        int index = levelNumber - 1;

        if (levelLockIcons != null && index >= 0 && index < levelLockIcons.Length && levelLockIcons[index] != null)
            return levelLockIcons[index];

        if (runtimeLockIcons == null || runtimeLockIcons.Length < LevelProgression.LastConfiguredLevel)
            runtimeLockIcons = new GameObject[LevelProgression.LastConfiguredLevel];

        if (index < 0 || index >= runtimeLockIcons.Length || button == null)
            return null;

        if (runtimeLockIcons[index] == null)
            runtimeLockIcons[index] = CreateRuntimeLockIcon(button.transform);

        return runtimeLockIcons[index];
    }

    private GameObject CreateRuntimeLockIcon(Transform parent)
    {
        GameObject lockIcon = new GameObject("Runtime Lock Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        lockIcon.transform.SetParent(parent, false);

        RectTransform rectTransform = lockIcon.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(1f, 1f);
        rectTransform.anchoredPosition = new Vector2(-8f, -8f);
        rectTransform.sizeDelta = new Vector2(30f, 30f);

        TextMeshProUGUI label = lockIcon.GetComponent<TextMeshProUGUI>();
        label.text = "\U0001F512";
        label.alignment = TextAlignmentOptions.Center;
        label.fontSize = 22f;
        label.raycastTarget = true;

        return lockIcon;
    }

    private void EnsureLockIconFeedback(GameObject lockIcon)
    {
        Button feedbackButton = lockIcon.GetComponent<Button>();
        if (feedbackButton == null)
            feedbackButton = lockIcon.AddComponent<Button>();

        if (feedbackButton.targetGraphic == null)
            feedbackButton.targetGraphic = lockIcon.GetComponent<Graphic>();

        feedbackButton.onClick.RemoveListener(ShowLockedLevelMessage);
        feedbackButton.onClick.AddListener(ShowLockedLevelMessage);
    }

    private void ShowLockedLevelMessage()
    {
        Debug.Log(LOCKED_LEVEL_MESSAGE);

        EnsureLockedMessage();
        if (lockedLevelMessage == null) return;

        lockedLevelMessage.text = LOCKED_LEVEL_MESSAGE;
        lockedLevelMessage.gameObject.SetActive(true);

        if (lockedMessageCoroutine != null)
            StopCoroutine(lockedMessageCoroutine);

        lockedMessageCoroutine = StartCoroutine(ClearLockedMessageAfterDelay());
    }

    private IEnumerator ClearLockedMessageAfterDelay()
    {
        yield return new WaitForSeconds(lockedMessageSeconds);

        if (lockedLevelMessage != null)
        {
            lockedLevelMessage.text = string.Empty;
            lockedLevelMessage.gameObject.SetActive(false);
        }

        lockedMessageCoroutine = null;
    }

    private void EnsureLockedMessage()
    {
        if (lockedLevelMessage != null) return;

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        GameObject messageObject = new GameObject("Runtime Locked Level Message", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        messageObject.transform.SetParent(canvas.transform, false);

        RectTransform rectTransform = messageObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0f);
        rectTransform.anchorMax = new Vector2(0.5f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0f);
        rectTransform.anchoredPosition = new Vector2(0f, 80f);
        rectTransform.sizeDelta = new Vector2(640f, 80f);

        lockedLevelMessage = messageObject.GetComponent<TextMeshProUGUI>();
        lockedLevelMessage.text = string.Empty;
        lockedLevelMessage.alignment = TextAlignmentOptions.Center;
        lockedLevelMessage.fontSize = 30f;
        lockedLevelMessage.color = Color.white;
        lockedLevelMessage.raycastTarget = false;
        lockedLevelMessage.gameObject.SetActive(false);
    }

    private int GetSceneIndexForLevel(int levelNumber)
    {
        return levelNumber;
    }


    private IEnumerator LoadSceneWithDelay(int sceneIndex)
    {
        if (PlayerPrefs.GetInt(SOUND_ENABLED_KEY, 1) == 1)
            buttonClick.Play();

        yield return new WaitForSeconds(BUTTON_DELAY);
        SceneManager.LoadSceneAsync(sceneIndex);
    }
}
