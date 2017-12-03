using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public static Camera MainCamera { get; protected set; }

    public float m_maxPanSpeed = 0.25f;
    public float m_panUpDownThreshold = 0.2f;
    public float m_panLeftRightThreshold = 0.2f;

    public Vector3 m_cameraBounds;

    protected float m_shakeIntensity = 0.05f;
    protected float m_shakeDuration = 0.0f;

    protected void Awake()
    {
        MainCamera = Camera.main;
    }

    protected void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, m_cameraBounds);
    }

    public void Shake(float intensity = 0.05f, float duration = 0.25f)
    {
        m_shakeIntensity = intensity;
        m_shakeDuration = duration;
    }

    public void UpdateCamera()
    {
        Vector3 dir = Vector3.zero;

        Vector3 mousePos = Input.mousePosition;
        float leftRigthSize = Screen.width * m_panLeftRightThreshold;
        float upDownSize = Screen.height * m_panUpDownThreshold;
        if (mousePos.x < leftRigthSize)
        {
            // pan left
            dir += Vector3.left * (1.0f - Mathf.Clamp01(mousePos.x / leftRigthSize));
        }
        else if (mousePos.x > Screen.width - leftRigthSize)
        {
            // pan right
            dir += Vector3.right * Mathf.Clamp01((mousePos.x - (Screen.width - leftRigthSize)) / leftRigthSize);
        }

        if (mousePos.y < upDownSize)
        {
            // pan down
            dir += Vector3.down * (1.0f - Mathf.Clamp01(mousePos.y / upDownSize));
        }
        else if (mousePos.y > Screen.height - upDownSize)
        {
            // pan up
            dir += Vector3.up * Mathf.Clamp01((mousePos.y - (Screen.height - upDownSize)) / upDownSize);
        }

        Vector3 pos = transform.position + dir * m_maxPanSpeed;
        float halfXBounds = m_cameraBounds.x * 0.5f;
        float halfYBounds = m_cameraBounds.y * 0.5f;
        pos.x = Mathf.Clamp(pos.x, -halfXBounds, halfXBounds);
        pos.y = Mathf.Clamp(pos.y, -halfYBounds, halfYBounds);
        transform.position = pos;
        
        if(m_shakeDuration > 0.0f)
        {
            MainCamera.transform.localPosition = Vector3.Lerp(MainCamera.transform.localPosition, Random.onUnitSphere * m_shakeIntensity, Time.deltaTime * 10.0f);
            m_shakeDuration -= Time.deltaTime;
        }
        else
        {
            MainCamera.transform.localPosition = Vector3.Lerp(MainCamera.transform.localPosition, Vector3.zero, Time.deltaTime);
        }
    }
}
