using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : Singleton<Main> {
    public CameraManager m_cameraManager;
    public Hand m_hand;

    public int m_playAreaWidth = 100;
    public int m_playAreaHeight = 100;


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
        }
    }
}
