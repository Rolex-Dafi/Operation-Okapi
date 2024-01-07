using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Wrapper for the fancy tie item UI illustration which the player receives from the merchant as a gift.
/// </summary>
public class FancyTie : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private float waitTime = 1f;
    
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    /// <summary>
    /// Shows the UI for the item.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Receive()
    {
        canvasGroup.DOFade(1, waitTime/2);
        transform.DOScale(3, waitTime);
        yield return new WaitForSeconds(waitTime);
        
        canvasGroup.DOFade(0, waitTime/2);
        transform.DOScale(1, waitTime);
        yield return new WaitForSeconds(waitTime);
    }
}
