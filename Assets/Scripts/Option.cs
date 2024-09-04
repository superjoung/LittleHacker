using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Option : MonoBehaviour
{
    private GameObject optionSettingPanel;
    public Slider BGMSlider;
    public Slider SFXSlider;

    private void Awake()
    {
        if (BGMSlider)
        {
            BGMSlider.onValueChanged.AddListener(SetBGMVolume);
            SFXSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    private void Start()
    {
        Initialized();
    }

    private void Initialized()
    {
        if (SceneManager.GetActiveScene().name == "MainScenes") // 메인 게임 씬
        {
            GameObject.Find("OptionButton").GetComponent<Button>().onClick.AddListener(() => OptionButtonClick());
            optionSettingPanel = GameObject.Find("OptionSettingBox").transform.GetChild(0).gameObject;
        }

        else if(SceneManager.GetActiveScene().name == "1.StartScene") // 초기 화면
        {
            GameObject.Find("OptionButton").GetComponent<Button>().onClick.AddListener(() => OptionButtonClick());
            optionSettingPanel = GameObject.Find("Option").transform.GetChild(0).gameObject;
            GameObject.Find("GameStartButton").GetComponent<Button>().onClick.AddListener(() => GameStartButtonClick());
        }

        else if(SceneManager.GetActiveScene().name == "2.StoryModeScene") // 게임 선택 화면
        {
            GameObject.Find("OptionButton").GetComponent<Button>().onClick.AddListener(() => OptionButtonClick());
            optionSettingPanel = GameObject.Find("Option").transform.GetChild(0).gameObject;
        }

        if (BGMSlider)
        {
            if (PlayerPrefs.HasKey("BGM"))
            {
                BGMSlider.value = PlayerPrefs.GetFloat("BGM");
                SFXSlider.value = PlayerPrefs.GetFloat("SFX");
                SoundManager.Instance.InitVoumes(BGMSlider.value, SFXSlider.value);
            }
            else
            {
                BGMSlider.value = -10;
                BGMSlider.value = -10;
                PlayerPrefs.SetFloat("BGM", -10);
                PlayerPrefs.SetFloat("SFX", -10);
                SoundManager.Instance.InitVoumes(-10, -10);
            }
        }
    }


    void OptionButtonClick()
    {
        if (!optionSettingPanel.activeSelf)
        {
            optionSettingPanel.SetActive(true);
            GameObject.Find("ExitButton").GetComponent<Button>().onClick.AddListener(() => OptionButtonClick());
        }
        else
        {
            optionSettingPanel.SetActive(false);
        }
    }

    void GameStartButtonClick()
    {
        SceneManager.LoadScene(1);
    }

    public void SetBGMVolume(float value)
    {
        SoundManager.Instance.SetVolumes(SoundType.BGM, value);
        PlayerPrefs.SetFloat("BGM", value);
    }

    public void SetSFXVolume(float value)
    {
        SoundManager.Instance.SetVolumes(SoundType.EFFECT, value);
        PlayerPrefs.SetFloat("SFX", value);
    }
}
