using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    public static readonly int TRIGGER_ID_SCRATCH = Animator.StringToHash("Scratch");

    public Animator m_animator;

    protected void Awake()
    {
        if(m_animator == null)
        {
            m_animator = GetComponentInChildren<Animator>();
        }
    }

    public void Scratch()
    {
        m_animator.SetTrigger(TRIGGER_ID_SCRATCH);
    }
}
