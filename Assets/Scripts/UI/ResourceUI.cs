using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the UI of any resource. Expects either a Slider or a Text component (or both) on one of this object's children.
/// </summary>
public class ResourceUI : MonoBehaviour
{
    private Resource resource;

    [SerializeField] private bool hideWhenFull = false;

    private Slider bar;
    private TMPro.TextMeshProUGUI text;

    /// <summary>
    /// Initializes this resource's UI. Should be called from a HUD manager for each resource's UI, or from a character spawner
    /// for world space UI.
    /// </summary>
    /// <param name="resource">The resource whose UI we're initializing.</param>
    public void Init(Resource resource)
    {
        this.resource = resource;

        bar = GetComponentInChildren<Slider>();
        text = GetComponentInChildren<TMPro.TextMeshProUGUI>();

        resource.onChangedMax.AddListener(ChangeMax);
        resource.onChangedCurrent.AddListener(ChangeCurrent);

        ChangeMax(resource.GetMax());
        ChangeCurrent(resource.GetCurrent());
    }

    private void ChangeMax(int value)
    {
        if (bar != null) bar.maxValue = value;
    }

    private void ChangeCurrent(int value)
    {
        if (bar != null) bar.value = value;

        if (text != null) text.text = value.ToString();

        if (hideWhenFull)
        {
            gameObject.SetActive(!resource.IsMaxed());
        }

    }

}
