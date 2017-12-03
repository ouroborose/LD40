using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    public static readonly int TRIGGER_ID_SCRATCH = Animator.StringToHash("Scratch");

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

    public void Scratch()
    {
        m_animator.SetTrigger(TRIGGER_ID_SCRATCH);
        m_particles.Emit(Random.Range(10, 20));
    }
}
