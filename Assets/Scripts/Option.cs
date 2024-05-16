using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Option : MonoBehaviour
{
    GameObject optionSettingPanel;

    private void Start()
    {
        Initialized();
    }

    private void Initialized()
    {
        if (SceneManager.sceneCount == 1) // ∏ﬁ¿Œ ∞‘¿” æ¿
        {
            GameObject.Find("OptionButton").GetComponent<Button>().onClick.AddListener(() => OptionButtonClick());
            optionSettingPanel = GameObject.Find("OptionSettingBox").transform.GetChild(0).gameObject;
        }

        else if(SceneManager.sceneCount == 0)
        {

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
}
