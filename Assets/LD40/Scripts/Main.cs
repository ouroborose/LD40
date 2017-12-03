using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : Singleton<Main> {
    public static RaycastHit2D[] s_raycastHits = new RaycastHit2D[100];

    public CameraManager m_cameraManager;
    public Hand m_hand;

    public GameObject m_rashPrefab;

    public List<Rash> m_rashes = new List<Rash>();

    protected Plane m_interactionPlane;

    override protected void Awake()
    {
        base.Awake();

        m_interactionPlane = new Plane(Vector3.back, 0);
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
            m_hand.Scratch();
            int hitCount = Physics2D.RaycastNonAlloc(m_hand.transform.position, Vector2.zero, s_raycastHits);
            int rashHits = 0;
            for (int i = 0; i < hitCount; ++i)
            {
                RaycastHit2D hit = s_raycastHits[i];
                Rash rash = hit.collider.GetComponentInParent<Rash>();
                if(rash != null)
                {
                    rashHits++;
                    rash.Grow();
                }
            }

            if(rashHits <= 0 && hitCount > 0)
            {
                // spawn new rash here
                GameObject rashObj = Instantiate(m_rashPrefab, transform);
                rashObj.transform.position = mouseRay.GetPoint(d);
                rashObj.transform.Rotate(Vector3.forward, Random.value * 360.0f);
                Rash newRash = rashObj.GetComponent<Rash>();
                m_rashes.Add(newRash);
            }
        }
    }
}
