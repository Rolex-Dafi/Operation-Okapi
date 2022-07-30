using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PCGDictionaryData", menuName = "ScriptableObjects/PCG Dictionary")]
public class PCGDictionarySO : ScriptableObject
{
    public Dictionary<string, string> dict;
}
