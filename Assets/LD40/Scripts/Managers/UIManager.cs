using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public const float TEXT_SCRATCH_THRESHOLD = -3.0f;
    public const float TEXT_HIDE_THRESHOLD = -0.5f;

    public Image m_fader;
    public float m_fadeTime = 1.0f;

    public RectTransform m_title;
    public RectTransform m_lose;
    public RectTransform m_loseFace;
    public RectTransform m_win;
    public RectTransform m_winFace;

    public Slider m_sanitySlider;

    public RectTransform m_textContainer;
    public TextMeshProUGUI m_feedbackText;

    public RectTransform m_progressIndicator;
    public RectTransform m_progressStart;
    public RectTransform m_progressEnd;

    public Image m_face;
    public float m_faceAnimationSpeed = 5.0f;
    public float m_faceAnimationAngle = 5.0f;

    public Sprite[] m_satisfiedFaceSprites;
    public Sprite[] m_faceSprites;

    public float m_itchFeedbackShowTime = 0.5f;
    public string[] m_itchFeedback;

    public float m_hintShowTime = 0.2f;
    public string[] m_upHints;
    public string[] m_downHints;
    public string[] m_leftHints;
    public string[] m_rightHints;
    protected string[][] m_directionHints;

    public string[] m_scratch1;
    public string[] m_scratch2;
    public string[] m_scratch3;
    public string[] m_scratch4;
    public string[] m_scratch5;
    protected string[][] m_scratchHints;

    public enum FeedBackType : int
    {
        SCRATCH_ENCOURAGEMENT = -3,
        ITCH_FEEDBACK = -2,
        NONE = -1,
        UP = 0 ,
        DOWN,
        LEFT,
        RIGHT,
        NUM_DIRECTIONS,
    }
    

    protected float m_feedbackShowTimer = 0.0f;
    protected int m_lastTextIndex = -1;

    protected int m_faceIndex = 0;
    protected FeedBackType m_lastFeedBackType = FeedBackType.NONE;

    protected float m_faceAnimationTimer = 0;

    protected void Awake()
    {
        m_directionHints = new string[][]
        {
            m_upHints,
            m_downHints,
            m_leftHints,
            m_rightHints,
        };

        m_scratchHints = new string[][]
        {
            m_scratch1,
            m_scratch2,
            m_scratch3,
            m_scratch4,
            m_scratch5,
        };

        FadeIn();
    }

    protected void Update()
    {
        AnimateFace();

        if(Main.Instance.m_currentGameState != Main.GameState.Game)
        {
            return;
        }

        if (m_lastFeedBackType != FeedBackType.NONE && m_feedbackShowTimer > TEXT_HIDE_THRESHOLD)
        {
            m_feedbackShowTimer -= Time.deltaTime;
            if (m_feedbackShowTimer <= TEXT_HIDE_THRESHOLD)
            {
                m_textContainer.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack);
            }
        }
        else if(m_feedbackShowTimer > TEXT_SCRATCH_THRESHOLD)
        {
            m_feedbackShowTimer -= Time.deltaTime;
            if(m_feedbackShowTimer <= TEXT_SCRATCH_THRESHOLD)
            {
                if(ShowFeedback(m_scratchHints[m_faceIndex], 3.0f))
                {
                    m_lastFeedBackType = FeedBackType.SCRATCH_ENCOURAGEMENT;
                }
            }
        }
        
        if(m_lastFeedBackType != FeedBackType.ITCH_FEEDBACK || m_feedbackShowTimer <= 0)
        {
            int faceIndex = Mathf.Clamp(m_faceSprites.Length - Mathf.CeilToInt(m_sanitySlider.value * m_faceSprites.Length), 0, m_faceSprites.Length -1);
            if (faceIndex != m_faceIndex)
            {
                if(ShowFeedback(m_scratchHints[m_faceIndex], 5.0f, m_lastFeedBackType != FeedBackType.ITCH_FEEDBACK))
                {
                    m_lastFeedBackType = FeedBackType.SCRATCH_ENCOURAGEMENT;
                }
            }
            m_faceIndex = faceIndex;
            m_face.sprite = m_faceSprites[m_faceIndex];
        }
    }

    public void FadeIn(UnityAction onFadeIn = null)
    {
        m_fader.DOFade(0, m_fadeTime).OnComplete(() =>
        {
            if(onFadeIn != null)
            {
                onFadeIn.Invoke();
            }
            m_fader.gameObject.SetActive(false);
        });
    }

    public void FadeOut(UnityAction onFadeOut = null)
    {
        m_fader.gameObject.SetActive(true);
        m_fader.DOFade(0, m_fadeTime).OnComplete(() =>
        {
            if (onFadeOut != null)
            {
                onFadeOut.Invoke();
            }
        });
    }

    public void StartGame()
    {
        Main.Instance.StartGame();
        m_title.DOMoveY(Screen.height * 2, 0.5f).SetEase(Ease.InBack);
    }

    public void RestartGame()
    {
        FadeOut(() =>
        {
            SceneManager.LoadScene(0);
        });
    }

    public void HandleLose()
    {
        m_sanitySlider.gameObject.SetActive(false);
        m_lose.localPosition = Vector3.up * Screen.height * 2;
        m_lose.gameObject.SetActive(true);
        m_lose.DOMoveY(0, 0.5f).SetEase(Ease.OutBack);
    }

    public void HandleWin()
    {
        m_sanitySlider.gameObject.SetActive(false);
        m_win.localPosition = Vector3.up * Screen.height * 2;
        m_win.gameObject.SetActive(true);
        m_win.DOMoveY(0, 0.5f).SetEase(Ease.OutBack);
    }

    protected void AnimateFace()
    {
        float speedFactor = (m_faceIndex + 1);
        m_faceAnimationTimer += Time.deltaTime * m_faceAnimationSpeed * speedFactor;
        float angle = Mathf.Sin(m_faceAnimationTimer) * m_faceAnimationAngle * speedFactor;
        m_face.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if(Main.Instance.m_currentGameState == Main.GameState.Lose)
        {
            m_loseFace.transform.localRotation = m_face.transform.localRotation;
        }
        else if(Main.Instance.m_currentGameState == Main.GameState.Win)
        {
            m_winFace.transform.localRotation = m_face.transform.localRotation;
        }
    }

    public void SetProgressIndicator(float value)
    {
        m_progressIndicator.position = Vector3.Lerp(m_progressStart.position, m_progressEnd.position, Mathf.Clamp01(value));
    }

    public void SetSanitySliderValue(float value)
    {
        value = Mathf.Clamp01(value);
        m_sanitySlider.value = value;
    }

    public void DoItchFeedback()
    {
        if(ShowFeedback(m_itchFeedback, m_itchFeedbackShowTime, m_lastFeedBackType != FeedBackType.ITCH_FEEDBACK))
        {
            m_face.sprite = m_satisfiedFaceSprites[Random.Range(0, m_satisfiedFaceSprites.Length)];
            m_lastFeedBackType = FeedBackType.ITCH_FEEDBACK;
        }
    }

    public void GiveHint(FeedBackType direction)
    {
        if(m_lastFeedBackType != FeedBackType.ITCH_FEEDBACK && m_lastFeedBackType != FeedBackType.SCRATCH_ENCOURAGEMENT && m_feedbackShowTimer > 0)
        {
            return;
        }

        int stringIndex = (int)direction;
        if(stringIndex >= 0)
        {
            if(ShowFeedback(m_directionHints[stringIndex], m_hintShowTime, m_lastFeedBackType != FeedBackType.ITCH_FEEDBACK))
            {
                m_lastFeedBackType = direction;
            }
        }
    }

    public bool ShowFeedback(string[] strings, float showTime = 0.0f, bool forceShow = false)
    {
        if(!forceShow && m_feedbackShowTimer > 0.0f)
        {
            return false;
        }

        m_textContainer.localScale = Vector3.zero;
        m_textContainer.DOScale(Vector3.one, 0.33f).SetEase(Ease.OutBack);

        m_feedbackShowTimer = showTime;
        int textIndex = Random.Range(0, strings.Length);
        while(textIndex == m_lastTextIndex && strings.Length > 1)
        {
            textIndex = Random.Range(0, strings.Length);
        }
        m_lastTextIndex = textIndex;
        m_feedbackText.text = strings[textIndex];
        return true;
    }
}
