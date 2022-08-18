using System;
using System.Collections.Generic;
using UnityEngine;

public enum EItemEffectType
{
    ChangeSpeed,
    ChangeMeleeDamage,
    ChangeRangedDamage,

}

[Serializable]
public class ItemEffect
{
    public EItemEffectType Type;

    // type dependent properties
    public float ValueModifier; // in percent - ex. +0.1 means change value by adding 10% of it's base value to it
}


public class ItemSO : ScriptableObject
{
    public int ID;
    public string ItemName;
    public List<ItemEffect> ItemEffects;
    public Sprite UISprite;
    public Sprite WorldSprite;
    public int Health; // on the last health - display cracked gfx in HUD

}
