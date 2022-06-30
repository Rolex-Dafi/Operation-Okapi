using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/Enemy Character")]
public class EnemyCharacterSO : CharacterSO
{
    [Range(1, 10)] public float lineOfSightRange;

    [Range(1, 10)] public float reloadTime;

}
