using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private CanvasGroup canvasGroup;
    
    public void Init(string text = "")
    {
        if (text != "") tooltipText.text = text;
        
        // hidden by default
        canvasGroup.alpha = 0;
    }

    public void ShowToolTip(bool show)
    {
        Debug.Log("displaying tooltip: " + tooltipText.text + ", setting show to " + show);
        // TODO tween
        canvasGroup.alpha = show ? 1 : 0;
    }
}
