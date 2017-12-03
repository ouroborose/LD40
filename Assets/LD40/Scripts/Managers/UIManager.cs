using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Slider m_sanitySlider;
    public TextMeshProUGUI m_feedbackText;

    public float m_itchFeedbackShowTime = 0.5f;
    public string[] m_itchFeedback;

    public float m_hintShowTime = 0.2f;
    public string[] m_upHints;
    public string[] m_downHints;
    public string[] m_leftHints;
    public string[] m_rightHints;

    public enum FeedBackType : int
    {
        ITCH_FEEDBACK = -2,
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
    
    protected float m_desiredSanity = 1.0f;

    protected FeedBackType m_lastFeedBackType = FeedBackType.NONE;

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

        m_sanitySlider.value = Mathf.Lerp(m_sanitySlider.value, m_desiredSanity, Time.deltaTime * 5.0f);
    }

    public void SetSanitySliderValue(float value)
    {
        m_desiredSanity = value;
    }

    public void DoItchFeedback()
    {
        ShowFeedback(m_itchFeedback, m_itchFeedbackShowTime, m_lastFeedBackType != FeedBackType.ITCH_FEEDBACK);
        m_lastFeedBackType = FeedBackType.ITCH_FEEDBACK;
    }

    public void GiveHint(FeedBackType direction)
    {
        int stringIndex = (int)direction;
        if(stringIndex >= 0)
        {
            ShowFeedback(directionHints[stringIndex], m_hintShowTime);
            m_lastFeedBackType = direction;
        }
    }

    public void ShowFeedback(string[] strings, float showTime = 0.0f, bool forceShow = false)
    {
        if(!forceShow && m_showTimer > 0.0f)
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
