using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TableHandler : MonoBehaviour
{
    [Header("Table dimensions")] public int width = 2;
    public int height = 1;

    public GameObject chairUp;
    public GameObject chairDown;

    private SpriteRenderer mainTableSprite;
    private SpriteRenderer supportTableSprite;

    private List<Sprite[]> tablesUp;
    private List<Sprite[]> tablesDown;

    private void Start()
    {
        mainTableSprite = gameObject.GetComponent<SpriteRenderer>();
        supportTableSprite = gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();
        
        // get table sprites from resources
    }

    public void SetTableVariant(bool down ,bool chair = true)
    {
        if (chair)
        {
            if(down)
                chairDown.SetActive(true);
            else
                chairUp.SetActive(true);
        }
    }
}
