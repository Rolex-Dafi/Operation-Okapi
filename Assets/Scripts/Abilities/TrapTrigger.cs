using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    [SerializeField] private TrapController trapController;
    
    private void OnTriggerEnter(Collider other)
    {
        trapController.ActivateTrap();
    }
}
