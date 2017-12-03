using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : Singleton<Main> {

    public static RaycastHit2D[] s_raycastHits = new RaycastHit2D[100];

    public CameraManager m_cameraManager;
    public Hand m_hand;
    public float m_scratchDeltaThreshold = 0.25f;
    public GameObject m_rashPrefab;

    public List<Rash> m_rashes = new List<Rash>();

    protected Plane m_interactionPlane;
    protected List<Rash> m_scratchedThisFrame = new List<Rash>();
    protected List<Rash> m_itchPoints = new List<Rash>();

    protected float m_scratchDelta = 0;
    protected Vector3 m_lastScratchPos;

    override protected void Awake()
    {
        base.Awake();

        m_interactionPlane = new Plane(Vector3.back, 0);
        for(int i = 0; i < m_rashes.Count; ++i)
        {
            AddItch(m_rashes[i]);
        }

    }

    protected void Update()
    {
        m_cameraManager.UpdateCamera();
        UpdatePlayerControls();
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
        else if (Input.GetMouseButtonUp(0))
        {
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
        m_hand.DoScratchFeedback();
        int hitCount = Physics2D.RaycastNonAlloc(pos, Vector2.zero, s_raycastHits);
        int rashHits = 0;
        for (int i = 0; i < hitCount; ++i)
        {
            RaycastHit2D hit = s_raycastHits[i];
            Rash rash = hit.collider.GetComponentInParent<Rash>();
            if (rash != null)
            {
                rashHits++;
                Scratch(rash);

                if (m_itchPoints.Contains(rash))
                {
                    // give sanity
                    rash.ReduceItch();
                    if (rash.m_itchAmount <= 0)
                    {
                        m_itchPoints.Remove(rash);

                        // pick new itch point
                        AddItch(m_rashes[Random.Range(0, m_rashes.Count)]);
                    }
                    else
                    {
                        // give itch relief feedback

                    }
                }
                else
                {
                    // give itch point hint
                }
            }
        }

        if (rashHits <= 0 && hitCount > 0)
        {
            // spawn new rash here
            SpawnRash(pos);
        }
    }

    public void AddItch(Rash rash)
    {
        rash.RandomizeItchAmount();
        m_itchPoints.Add(rash);
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
    }

    public void TryToSpreadRash(Rash source, float minRadius, float maxRadius)
    {
        Vector2 center = source.transform.position;
        int attempts = 0;
        while(attempts < 50)
        {
            Vector2 spawnPos = center + Random.insideUnitCircle.normalized * Random.Range(minRadius, maxRadius);
            int hits = Physics2D.RaycastNonAlloc(spawnPos, Vector2.zero, s_raycastHits);
            if(hits > 0)
            {
                int rashHits = 0;
                for (int i = 0; i < hits; ++i)
                {
                    RaycastHit2D hit = s_raycastHits[i];
                    Rash rash = hit.collider.GetComponentInParent<Rash>();
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
                }
                return;
            }
            
            attempts++;
        }
    }

    public void SpawnRash(Vector3 pos)
    {
        GameObject rashObj = Instantiate(m_rashPrefab, transform);
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
                    // hint left
                }
                else
                {
                    // hint right
                }
            }
            else
            {
                if (delta.y < 0)
                {
                    // hint down
                }
                else
                {
                    // hint up
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
