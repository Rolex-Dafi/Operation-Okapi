using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FancyTie : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private float waitTime = 1f;
    
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    public IEnumerator Receive()
    {
        canvasGroup.DOFade(1, waitTime/2);
        transform.DOScale(3, waitTime);
        yield return new WaitForSeconds(waitTime);
        
        canvasGroup.DOFade(0, waitTime/2);
        transform.DOScale(1, waitTime);
        yield return new WaitForSeconds(waitTime);
        
        Debug.Log("hello from fancy tie");
    }
}
