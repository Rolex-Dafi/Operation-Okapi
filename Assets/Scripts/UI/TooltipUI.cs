using TMPro;
using UnityEngine;

/// <summary>
/// Manager for UI tooltips.
/// </summary>
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

    /// <summary>
    /// Sets the text of the tooltip.
    /// </summary>
    /// <param name="text">The string to set the text to</param>
    public void SetText(string text)
    {
        tooltipText.text = text;
    }

    /// <summary>
    /// Show the tooltip.
    /// </summary>
    /// <param name="show"></param>
    public void ShowToolTip(bool show)
    {
        Debug.Log("displaying tooltip: " + tooltipText.text + ", setting show to " + show);
        // TODO tween
        canvasGroup.alpha = show ? 1 : 0;
    }
}
