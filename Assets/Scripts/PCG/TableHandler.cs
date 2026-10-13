using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = System.Object;
using Random = System.Random;

public class TableHandler : MonoBehaviour
{
    [Header("Table dimensions")] public int width = 2;
    public int height = 1;

    [Header("Chair objects to manipulate.")]
    public GameObject chairUp;
    public GameObject chairDown;

    public void SetTableVariant(bool down, Sprite mainTable = null, Sprite supportTable = null ,bool chair = true)
    {
        if (chair)
        {
            if (down)
                chairDown.SetActive(true);
            else
                chairUp.SetActive(true);
        }

        if (mainTable == null || supportTable == null) return;
        gameObject.GetComponent<SpriteRenderer>().sprite = mainTable;
        gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = supportTable;
    }
}
