using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextManager
{
    private List<Dictionary<string, object>> deClear_Text;
    private List<Dictionary<string, object>> clear_Text;
    private List<string> currentTexts = new List<string>();
    private int currentTextCount = 0;
    private int currentSn;
    private TMP_Text textPrintBox;

    public bool isTalk;
    public bool isSkip;

    public TextManager()
    {
        isTalk = false;
        isSkip = false;
        currentSn = 0;
    }

    private void SetDictionary()
    {
        deClear_Text = CSVReader.Read("SN_" + currentSn);
        clear_Text = CSVReader.Read("SN_" + currentSn + "_Clear");
    }

    private void SelectPrintText()
    {
        if (currentSn != GameManager.currentScenario)
        {
            currentSn = GameManager.currentScenario;
            SetDictionary();
        }

        if (GameManager.isClear)
        {
            for (int count = 0; count < deClear_Text.Count; count++)
            {
                if (deClear_Text[count]["Stage"].ToString() == GameManager.currentStage.ToString())
                {
                    currentTexts.Add(deClear_Text[count]["Talk"].ToString());
                }
            }
        }

        else
        {
            for (int count = 0; count < clear_Text.Count; count++)
            {
                if (clear_Text[count]["Stage"].ToString() == GameManager.currentStage.ToString())
                {
                    currentTexts.Add(clear_Text[count]["Talk"].ToString());
                }
            }
        }
    }
    
    public void StartSkip()
    {
        if (isTalk)
        {
            isSkip = true;
        }
    }

    public void StartTalk(bool isSkip = false, float TextPrintSpeed = 0.1f)
    {
        isTalk = true;
        SelectPrintText();

        if (textPrintBox == null) textPrintBox = GameObject.Find("TalkText").GetComponent<TMP_Text>();

        if (isSkip)
        {
            textPrintBox.text = currentTexts[-1].ToString();
            isTalk = false;
            return;
        }
    }

    IEnumerator printText(float TextPrintSpeed)
    {
        textPrintBox.text = "";
        int count = 0;
        string text = currentTexts[currentTextCount].ToString();

        while (count != text.Length)
        {
            if (isSkip) TextPrintSpeed = 0;
            if (count < text.Length)
            {
                textPrintBox.text += text[count].ToString();
                count++;
            }
            yield return new WaitForSeconds(TextPrintSpeed);
        }

        isSkip = false;
        isTalk = false;
    }
}
