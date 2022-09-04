using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialRoom : MonoBehaviour
{
    [SerializeField] private Transform entrance;
    [SerializeField] private Interactable exitTrigger;

    public Transform Entrance => entrance;
    public Interactable ExitTrigger => exitTrigger;
}
