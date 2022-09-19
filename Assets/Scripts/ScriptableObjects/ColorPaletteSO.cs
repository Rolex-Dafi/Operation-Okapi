using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game color palette data.
/// </summary>
[CreateAssetMenu(fileName = "ColorPalette", menuName = "ScriptableObjects/Color Palette")]
public class ColorPaletteSO : ScriptableObject
{
    public Color healthGrey;
    public Color healthRed;
    public Color healthBrightRed;
}
