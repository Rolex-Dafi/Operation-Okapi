using System;
using System.Collections.Generic;
using UnityEngine;

public enum EItemEffectType
{
    ChangeSpeed,
    ChangeDamage,

}

[Serializable]
public class ItemEffect
{
    public EItemEffectType Type;

    // type dependent properties
    public float ValueChange;
}


public class ItemSO : ScriptableObject
{
    public int ID;
    public string ItemName;
    public List<ItemEffect> ItemEffects;
    public Sprite UISprite;
    public Sprite WorldSprite;

}
