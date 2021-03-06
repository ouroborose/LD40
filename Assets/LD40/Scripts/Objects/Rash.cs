﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rash : MonoBehaviour
{
    public const float INITIAL_ALPHA = 0.1f;
    public const float DELTA_ALPHA = 0.7f;

    public const int MIN_ITCH_AMOUNT = 3;
    public const int MAX_ITCH_AMOUNT = 10;

    public const int MIN_SPREAD_SCRATCHES = 1;
    public const int MAX_SPREAD_SCRATCHES = 1;

    public const float RASH_GROWTH_AMOUNT = 0.05f;
    public const float INITIAL_SIZE = 1.0f;
    public const float MAX_SIZE = 2.0f;

    public bool m_scratchedThisFrame = false;

    public SpriteRenderer[] m_sprites;

    public int m_itchAmount = 0;

    protected int m_overScratchCount = 0;
    protected int m_nextSpreadCountRequirement = 0;

    protected float m_currentSize = 1.0f;

    protected void Awake()
    {
        m_sprites = GetComponentsInChildren<SpriteRenderer>();
        RandomizeNextSpreadRequirement();
        SetAlpha(INITIAL_ALPHA);
    }

    public void RandomizeNextSpreadRequirement()
    {
        m_nextSpreadCountRequirement = Random.Range(MIN_SPREAD_SCRATCHES, MAX_SPREAD_SCRATCHES);
    }

    public void RandomizeItchAmount()
    {
        m_itchAmount = Random.Range(MIN_ITCH_AMOUNT, MAX_ITCH_AMOUNT);
    }

    public void ReduceItch()
    {
        m_itchAmount--;
        if(m_itchAmount <= 0)
        {
            m_itchAmount = 0;
        }
    }

    public void Scratch()
    {
        if(TryToGrow())
        {
            m_overScratchCount++;
            if(m_overScratchCount > m_nextSpreadCountRequirement)
            {
                Bounds bounds = GetBounds();
                Main.Instance.TryToSpreadRash(this, bounds.extents.x*0.75f, bounds.extents.x * 1.25f);
                RandomizeNextSpreadRequirement();
            }
        }
    }

    public bool TryToGrow()
    {
        m_currentSize += RASH_GROWTH_AMOUNT;
        bool isMaxSize = m_currentSize >= MAX_SIZE;
        if (isMaxSize)
        {
            m_currentSize = MAX_SIZE;
        }
        SetAlpha(INITIAL_ALPHA + DELTA_ALPHA*((m_currentSize - INITIAL_SIZE)/(MAX_SIZE - INITIAL_SIZE)));
        transform.localScale = Vector3.one * m_currentSize;
        return isMaxSize;
    }

    public void SetAlpha(float alpha)
    {
        for(int i = 0; i < m_sprites.Length; ++i)
        {
            Color c = m_sprites[i].color;
            c.a = alpha;
            m_sprites[i].color = c;
        }
    }

    public Bounds GetBounds()
    {
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        for (int i = 0; i < m_sprites.Length; ++i)
        {
            bounds.Encapsulate(m_sprites[i].bounds);
        }
        return bounds;
    }
}
