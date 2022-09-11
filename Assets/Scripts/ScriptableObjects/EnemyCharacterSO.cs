using UnityEngine;

/// <summary>
/// Enemy character data class.
/// </summary>
[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/Enemy Character")]
public class EnemyCharacterSO : CharacterSO
{
    [Range(1, 20)] public float lineOfSightRange;

    [Range(1, 10)] public float patrollWaitTime = 1f;
}
