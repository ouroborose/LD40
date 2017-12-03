using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    public static readonly int ANIM_ID_SCRATCH = Animator.StringToHash("IsScratching");

    public ParticleSystem m_particles;
    public Animator m_animator;

    protected void Awake()
    {
        if(m_animator == null)
        {
            m_animator = GetComponentInChildren<Animator>();
        }

        if(m_particles == null)
        {
            m_particles = GetComponentInChildren<ParticleSystem>();
        }
    }

    public void StartScratching()
    {
        m_animator.SetBool(ANIM_ID_SCRATCH, true);
    }

    public void DoScratchFeedback()
    {
        m_particles.Emit(Random.Range(10, 20));
    }

    public void StopScratching()
    {
        m_animator.SetBool(ANIM_ID_SCRATCH, false);
    }
}
