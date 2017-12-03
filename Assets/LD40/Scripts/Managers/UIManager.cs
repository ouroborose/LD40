using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI m_feedbackText;

    public float m_itchFeedbackShowTime = 0.5f;
    public string[] m_itchFeedback;

    public float m_hintShowTime = 0.2f;
    public string[] m_upHints;
    public string[] m_downHints;
    public string[] m_leftHints;
    public string[] m_rightHints;

    public enum HintDirection : int
    {
        NONE = -1,
        UP = 0 ,
        DOWN,
        LEFT,
        RIGHT,
        NUM_DIRECTIONS,
    }
    
    protected string[][] directionHints;

    protected float m_showTimer = 0.0f;
    protected int m_lastTextIndex = -1;

    protected void Awake()
    {
        directionHints = new string[][]
        {
            m_upHints,
            m_downHints,
            m_leftHints,
            m_rightHints,
        };
    }

    protected void Update()
    {
        m_showTimer -= Time.deltaTime;
    }

    public void DoItchFeedback()
    {
        ShowFeedback(m_itchFeedback, m_itchFeedbackShowTime);
    }

    public void GiveHint(HintDirection direction)
    {
        ShowFeedback(directionHints[(int)direction], m_hintShowTime);
    }

    public void ShowFeedback(string[] strings, float showTime = 0.0f)
    {
        if(m_showTimer > 0.0f)
        {
            return;
        }

        m_showTimer = showTime;
        int textIndex = Random.Range(0, strings.Length);
        while(textIndex == m_lastTextIndex && strings.Length > 1)
        {
            textIndex = Random.Range(0, strings.Length);
        }
        m_lastTextIndex = textIndex;
        m_feedbackText.text = strings[textIndex];
    }
}
