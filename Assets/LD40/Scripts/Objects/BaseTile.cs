using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTile : MonoBehaviour {
    public enum TileType
    {
        Skin,
        Rash,
    }

    public TileType m_type = TileType.Skin;
}
