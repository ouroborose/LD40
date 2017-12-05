using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : Singleton<Main> {
    public const float MAX_SANITY = 100.0f;
    public const int MAX_SPREAD_DEPTH = 50;

    public static Collider2D[] s_collider2Ds = new Collider2D[10];

    public CameraManager m_cameraManager;
    public UIManager m_uiManager;
    public Hand m_hand;
    public float m_handRotationSpeed = 2.0f;

    public float m_scratchDeltaThreshold = 0.25f;
    public GameObject[] m_rashPrefabs;

    public List<Rash> m_rashes = new List<Rash>();

    public float m_remainingDuration = 60.0f;

    public float m_timeBetweenRashGrowth = 1.0f;
    public float m_rashGrowthIntensityDivider = 5.0f;

    public float m_itchSanityRecovery = 0.1f;
    public float m_minSanityLoss = 1.0f;
    public float m_sanityLossScaler = 1.0f;
    public float m_itchRashIntensityValue = 0.1f;
    public float m_spreadRashIntensityValue = 0.01f;
    public float m_scratchRashIntensityValue = 0.001f;
    public float m_itchMissSanityPenalty = 0.01f;

    protected Plane m_interactionPlane;
    protected List<Rash> m_scratchedThisFrame = new List<Rash>();
    protected List<Rash> m_itchPoints = new List<Rash>();

    protected float m_scratchDelta = 0;
    protected Vector3 m_lastScratchPos;

    protected float m_rashGrowthTimer = 0.0f;

    protected float m_currentSanity = 100.0f;
    protected float m_targetSanity = 100.0f;
    protected float m_rashIntensity = 0.0f;

    protected float m_totalDuration;

    public enum GameState
    {
        Title,
        Game,
        Win,
        Lose,
    }

    public GameState m_currentGameState = GameState.Title;

    override protected void Awake()
    {
        base.Awake();

        m_interactionPlane = new Plane(Vector3.back, 0);
        for(int i = 0; i < m_rashes.Count; ++i)
        {
            AddItch(m_rashes[i]);
        }

        m_totalDuration = m_remainingDuration;
    }

    protected void Update()
    {

        UpdatePlayerControls();

        switch (m_currentGameState)
        {
            case GameState.Game:
                UpdateRash();
                UpdateSanity();

                UpdateProgress();
                break;
            case GameState.Lose:
                // show lose screen
                //SceneManager.LoadScene(0);
                m_cameraManager.Shake(3);
                break;
            case GameState.Win:
                if(m_rashes.Count > 0)
                {
                    Rash rash = m_rashes[Random.Range(0, m_rashes.Count)];
                    m_rashes.Remove(rash);
                    Destroy(rash.gameObject);
                }
                break;
        }

        if(m_currentGameState != GameState.Title)
        {
            m_cameraManager.UpdateCamera();
        }
    }


    public void StartGame()
    {
        m_currentGameState = GameState.Game;
    }

    public void LoseGame()
    {
        m_currentGameState = GameState.Lose;
        m_uiManager.HandleLose();
    }

    public void WinGame()
    {
        m_currentGameState = GameState.Win;
        m_uiManager.HandleWin();
    }
    
    protected void UpdateProgress()
    {
        m_remainingDuration -= Time.deltaTime;
        if (m_remainingDuration <= 0.0f)
        {
            WinGame();
        }
        m_uiManager.SetProgressIndicator(1.0f - m_remainingDuration / m_totalDuration);
    }

    protected void UpdateRash()
    {
        m_rashGrowthTimer += Time.deltaTime;
        if(m_rashGrowthTimer > m_timeBetweenRashGrowth)
        {
            m_rashGrowthTimer -= m_timeBetweenRashGrowth;
            int numRashesToGrow = Mathf.CeilToInt(m_rashIntensity / m_rashGrowthIntensityDivider);
            for(int i = 0; i < numRashesToGrow; ++i)
            {
                Scratch(m_rashes[Random.Range(0, m_rashes.Count)]);
            }
        }
    }

    protected void UpdateSanity()
    {
        m_targetSanity -= (m_minSanityLoss + m_sanityLossScaler * m_rashIntensity) * Time.deltaTime;
        if(m_targetSanity <= 0.0f)
        {
            m_targetSanity = 0.0f;
        }

        m_currentSanity = Mathf.Lerp(m_currentSanity, m_targetSanity, Time.deltaTime * 5.0f);
        if (m_currentSanity <= 0.05f)
        {
            // lose
            m_currentSanity = 0.0f;
            LoseGame();
        }
        m_uiManager.SetSanitySliderValue(m_currentSanity / MAX_SANITY);

        
    }

    protected void UpdatePlayerControls()
    {
        Vector3 mousePos = Input.mousePosition;

        Ray mouseRay = CameraManager.MainCamera.ScreenPointToRay(mousePos);
        float d;
        m_interactionPlane.Raycast(mouseRay, out d);
        
        m_hand.transform.position = mouseRay.GetPoint(d - 0.5f);
        
        if (Input.GetMouseButtonDown(0))
        {
            m_hand.StartScratching();
            m_lastScratchPos = mouseRay.GetPoint(d);
            m_scratchDelta = 0;
            HandleScratch(m_lastScratchPos);
        }
        else if(Input.GetMouseButton(0))
        {
            Vector3 currentScratchPos = mouseRay.GetPoint(d);
            m_scratchDelta += Vector3.Distance(m_lastScratchPos, currentScratchPos);
            if (m_scratchDelta >= m_scratchDeltaThreshold)
            {
                HandleScratch(currentScratchPos);
                m_scratchDelta = 0;
            }
            m_lastScratchPos = currentScratchPos;
        }
        else
        {
            Vector3 handUp = CameraManager.MainCamera.transform.position - m_hand.transform.position;
            handUp.z = 0;
            handUp.Normalize();
            m_hand.transform.rotation = Quaternion.Lerp(m_hand.transform.rotation, Quaternion.LookRotation(transform.forward, handUp), Time.deltaTime * m_handRotationSpeed);
            m_hand.StopScratching();
        }

        // clear stratch state
        for (int i = 0; i < m_scratchedThisFrame.Count; ++i)
        {
            m_scratchedThisFrame[i].m_scratchedThisFrame = false;
        }
        m_scratchedThisFrame.Clear();
    }

    public void HandleScratch(Vector3 pos)
    {
        int hitCount = Physics2D.OverlapPointNonAlloc(pos, s_collider2Ds);
        if (hitCount > 0)
        {
            m_hand.DoScratchFeedback();
        }

        if (m_currentGameState != GameState.Game)
        {
            return;
        }

        int rashHits = 0;
        bool itchHit = false;
        for (int i = 0; i < hitCount; ++i)
        {
            Collider2D collider = s_collider2Ds[i];
            Rash rash = collider.GetComponentInParent<Rash>();
            if (rash != null)
            {
                rashHits++;
                Scratch(rash);
                if (m_itchPoints.Contains(rash))
                {
                    itchHit = true;
                    m_targetSanity += MAX_SANITY * m_itchSanityRecovery;
                    if(m_targetSanity > MAX_SANITY)
                    {
                        m_targetSanity = MAX_SANITY;
                    }

                    // give sanity
                    rash.ReduceItch();
                    // give itch relief feedback
                    m_uiManager.DoItchFeedback();

                    if (rash.m_itchAmount <= 0)
                    {
                        m_itchPoints.Remove(rash);

                        // pick new itch point
                        AddItch(m_rashes[Random.Range(0, m_rashes.Count)]);
                    }
                }
                else
                {
                    // give itch point hint
                    GiveHint(pos);
                }
            }
        }

        

        if (rashHits <= 0 && hitCount > 0)
        {
            // spawn new rash here
            SpawnRash(pos);
        }

        if(itchHit)
        {
            m_cameraManager.Shake(1.0f);
        }
        else if(rashHits >= 2)
        {
            m_cameraManager.Shake(0.5f);
        }
        else if(rashHits >= 1)
        {
            m_cameraManager.Shake(0.25f);
        }
        else
        {
            m_cameraManager.Shake(0.15f);
        }

        if(!itchHit)
        {
            m_targetSanity -= m_itchMissSanityPenalty;
        }
    }

    public void AddItch(Rash rash)
    {
        rash.RandomizeItchAmount();
        m_itchPoints.Add(rash);
        m_rashIntensity += m_itchRashIntensityValue;
    }

    public void Scratch(Rash rash)
    {
        if(rash.m_scratchedThisFrame)
        {
            return;
        }

        rash.m_scratchedThisFrame = true;
        rash.Scratch();
        m_scratchedThisFrame.Add(rash);
        m_rashIntensity += m_scratchRashIntensityValue;
    }

    public void TryToSpreadRash(Rash source, float minRadius, float maxRadius)
    {
        Vector2 center = source.transform.position;
        int attempts = 0;
        while(attempts < 2)
        {
            if (Random.value > 0.5f)
            {
                return; // random chance each attempt to early out
            }

            Vector2 spawnPos = center + Random.insideUnitCircle.normalized * Random.Range(minRadius, maxRadius);
            int hits = Physics2D.OverlapPointNonAlloc(spawnPos, s_collider2Ds);
            if(hits > 0)
            {
                int rashHits = 0;
                for (int i = 0; i < hits; ++i)
                {
                    Collider2D collider = s_collider2Ds[i];
                    Rash rash = collider.GetComponentInParent<Rash>();
                    if (rash != null)
                    {
                        rashHits++;
                        Scratch(rash);
                    }
                }

                if (rashHits <= 0)
                {
                    // hit skin so spawn a new rash
                    SpawnRash(spawnPos);
                    m_rashIntensity += m_spreadRashIntensityValue;
                }
                return;
            }
            
            attempts++;
        }
    }

    public void SpawnRash(Vector3 pos)
    {
        GameObject rashObj = Instantiate(m_rashPrefabs[Random.Range(0, m_rashPrefabs.Length)], transform);
        rashObj.transform.position = pos;
        rashObj.transform.Rotate(Vector3.forward, Random.value * 360.0f);
        Rash newRash = rashObj.GetComponent<Rash>();
        m_rashes.Add(newRash);
    }

    public void GiveHint(Vector3 pos)
    {
        Rash closestItch = FindClosestItch(pos);
        if(closestItch != null)
        {
            // give hint
            Vector3 delta = closestItch.transform.position - pos;
            if(Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if(delta.x < 0)
                {
                    m_uiManager.GiveHint(UIManager.FeedBackType.LEFT);
                }
                else
                {
                    m_uiManager.GiveHint(UIManager.FeedBackType.RIGHT);
                }
            }
            else
            {
                if (delta.y < 0)
                {
                    m_uiManager.GiveHint(UIManager.FeedBackType.DOWN);
                }
                else
                {
                    m_uiManager.GiveHint(UIManager.FeedBackType.UP);
                }
            }
        }
    }

    public Rash FindClosestItch(Vector3 pos)
    {
        Rash closestItch = null;
        float closestDist = float.MaxValue;
        for (int i = 0; i < m_itchPoints.Count; ++i)
        {
            Rash rash = m_itchPoints[i];
            float dist = Vector3.SqrMagnitude(rash.transform.position - pos);
            if(dist < closestDist)
            {
                closestItch = rash;
                closestDist = dist;
            }
        }

        return closestItch;
    }
}
