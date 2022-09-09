
using UnityEngine;

[CreateAssetMenu(fileName = "TrapData", menuName = "ScriptableObjects/Trap")]
public class TrapSO : AbilitySO
{
    public int ID;  // used to find the correct trap controller in the scene
}
