using UnityEngine.Events;

/// <summary>
/// Base wrapper class for all resources.
/// </summary>
[System.Serializable]
public class Resource
{
    private int currentValue;
    private int maxValue;

    public UnityEvent<int> onChanged;  // any script reacting to changes of the currentValue 
                                       // of this resource should listen to this

    public Resource(int startingValue, int maxValue)
    {
        currentValue = startingValue;
        this.maxValue = maxValue;
        onChanged = new UnityEvent<int>();
    }

    public int GetMax() => maxValue;

    public int GetCurrent() => currentValue;

    public int ChangeCurrent(int value)
    {
        currentValue += value;
        currentValue = currentValue < 0 ? 0 : currentValue > maxValue ? maxValue : currentValue;
        onChanged.Invoke(currentValue);
        return currentValue;
    }
}
