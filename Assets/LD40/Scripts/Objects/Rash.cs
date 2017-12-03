using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rash : MonoBehaviour {
    public const float RASH_GROWTH_AMOUNT = 0.1f;
    public void Grow()
    {
        transform.localScale += Vector3.one * RASH_GROWTH_AMOUNT;
    }
}
