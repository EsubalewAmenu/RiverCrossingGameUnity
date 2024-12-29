using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScreenScript : MonoBehaviour
{
    public Button PlayButton;
    public Button QuitButton;
    public Button SoundOnButton;
    public Button SoundOffButton;
    public Button Level1Button;
    public Button Level2Button;

    [SerializeField] AudioSource Music;
    [SerializeField] AudioSource ButtonClick;

    public GameObject LevelSelect;
    



    private void Start()
    {
        PlayButton.onClick.AddListener(PlayGame);
        QuitButton. onClick.AddListener(QuitGame);
        SoundOnButton.onClick.AddListener(SoundOn);
        SoundOffButton.onClick.AddListener(SoundOff);
        Level1Button.onClick.AddListener(Level1);
        Level2Button.onClick.AddListener(Level2);

    }

    public void PlayGame()
    {
        LevelSelect.SetActive(true);
        ButtonClick.Play();
    }

    public void QuitGame()
    {
        Application.Quit();
        ButtonClick.Play();
    }

    public void Level1()
    {
        SceneManager.LoadSceneAsync(1);
        ButtonClick.Play();
    }
    public void Level2()
    {
        SceneManager.LoadSceneAsync(2);
        ButtonClick.Play();
    }


    public void SoundOn()
    {
        Music.Play();
    }

    public void SoundOff()
    {
        Music.Stop();
    }

}
