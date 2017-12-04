using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public const float TEXT_HIDE_THRESHOLD = -0.5f;
    public Slider m_sanitySlider;

    public RectTransform m_textContainer;
    public TextMeshProUGUI m_feedbackText;

    public Image m_face;

    public Sprite[] m_satisfiedFaceSprites;
    public Sprite[] m_faceSprites;

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

    protected float m_feedbackShowTimer = 0.0f;
    protected int m_lastTextIndex = -1;

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
        if(m_lastFeedBackType != FeedBackType.NONE && m_feedbackShowTimer > TEXT_HIDE_THRESHOLD)
        {
            m_feedbackShowTimer -= Time.deltaTime;
            if (m_feedbackShowTimer <= TEXT_HIDE_THRESHOLD)
            {
                m_textContainer.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack);
            }
        }
        
        if(m_lastFeedBackType != FeedBackType.ITCH_FEEDBACK || m_feedbackShowTimer <= 0)
        {
            m_face.sprite = m_faceSprites[m_faceSprites.Length - Mathf.CeilToInt(Mathf.Clamp01(m_sanitySlider.value + 0.01f) * m_faceSprites.Length)];
        }
    }

    public void SetSanitySliderValue(float value)
    {
        value = Mathf.Clamp01(value);
        m_sanitySlider.value = value;
    }

    public void DoItchFeedback()
    {
        ShowFeedback(m_itchFeedback, m_itchFeedbackShowTime, m_lastFeedBackType != FeedBackType.ITCH_FEEDBACK);
        m_face.sprite = m_satisfiedFaceSprites[Random.Range(0, m_satisfiedFaceSprites.Length)];
        m_lastFeedBackType = FeedBackType.ITCH_FEEDBACK;
    }

    public void GiveHint(FeedBackType direction)
    {
        if(m_feedbackShowTimer > 0)
        {
            return;
        }

        int stringIndex = (int)direction;
        if(stringIndex >= 0)
        {
            ShowFeedback(directionHints[stringIndex], m_hintShowTime);
            m_lastFeedBackType = direction;
        }
    }

    public void ShowFeedback(string[] strings, float showTime = 0.0f, bool forceShow = false)
    {
        if(!forceShow && m_feedbackShowTimer > 0.0f)
        {
            return;
        }

        m_textContainer.localScale = Vector3.zero;
        m_textContainer.DOScale(Vector3.one, showTime * 0.25f).SetEase(Ease.OutBack);

        m_feedbackShowTimer = showTime;
        int textIndex = Random.Range(0, strings.Length);
        while(textIndex == m_lastTextIndex && strings.Length > 1)
        {
            textIndex = Random.Range(0, strings.Length);
        }
        m_lastTextIndex = textIndex;
        m_feedbackText.text = strings[textIndex];
    }
}
