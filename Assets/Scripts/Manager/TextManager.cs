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
    private float textPrintSpeed;
    private bool isPrint;
    private bool isSkipText;

    public bool isTalk;

    public TextManager()
    {
        isTalk = false;
        isPrint = false;
        isSkipText = false;
        currentSn = 0;
        currentTextCount = 0;
    }

    private void SetDictionary()
    {
        deClear_Text = CSVReader.Read("SN_" + currentSn);
        clear_Text = CSVReader.Read("SN_" + currentSn + "_Clear");
    }

    private bool SelectPrintText()
    {
        if (currentTexts.Count > 0) return true;
        if (currentSn != GameManager.currentScenario)   
        {
            currentSn = GameManager.currentScenario;
            SetDictionary();
        }

        if (!GameManager.isClear)
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

        if (currentTexts.Count == 0) return false;
        else return true; 
    }

    public bool StartTalk(bool isSkip = false, float TextPrintSpeed = 0.1f)
    {
        textPrintSpeed = TextPrintSpeed;
        if (isSkipText)
        {
            ClearTextBox();
            return false;
        }

        if (!SelectPrintText())
        {
            ClearTextBox();
            return false;
        }

        isTalk = true;

        if (textPrintBox == null) textPrintBox = GameObject.Find("TalkText").GetComponent<TMP_Text>();

        if (isSkip)
        {
            textPrintBox.text = currentTexts[0].ToString();
            isSkipText = true;
            return false;
        }
        if(!isPrint) CoroutineHelper.StartCoroutine(printText());
        else textPrintSpeed = 0;
        return true;
    }

    public void ClearTextBox()
    {
        isSkipText = false;
        isTalk = false;
        textPrintBox.text = "";
        if (GameManager.isClear)
        {
            GameManager.isClear = false;
            GameManager.StageClear();
        }
    }

    IEnumerator printText()
    {
        isPrint = true;
        textPrintBox.text = "";
        int count = 0;
        string text = currentTexts[currentTextCount].ToString();

        while (count != text.Length)
        {
            if (count < text.Length)
            {
                textPrintBox.text += text[count].ToString();
                count++;
            }
            yield return new WaitForSeconds(textPrintSpeed);
        }
        currentTextCount++;
        if (currentTexts.Count == currentTextCount)
        {
            currentTexts.Clear();
            currentTextCount = 0;
            isSkipText = true;
        }
        isPrint = false;
    }
}
