using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {
    public Transform m_tileContainer;

    public float m_tileSize = 1.0f;

    public GameObject m_skinPrefab;
    public GameObject m_rashPrefab;
    
    public int m_height { get; protected set; }
    public int m_width { get; protected set; }

    protected BaseTile[,] m_tiles;

    protected void Awake()
    {
        if(m_tileContainer == null)
        {
            m_tileContainer = transform;
        }
    }

    public void GeneratePlayArea(int height, int width)
    {
        m_height = height;
        m_width = width;

        m_tiles = new BaseTile[width, height];

        for(int x = 0; x < width; ++x)
        {
            for(int y = 0; y < height; ++y)
            {
                GameObject skinObj = Instantiate(m_skinPrefab, m_tileContainer);
                BaseTile tile = skinObj.GetComponent<BaseTile>();
                tile.transform.position = new Vector3(x * m_tileSize, y * m_tileSize, 0);
                m_tiles[x, y] = tile;
            }
        }
    }
}
