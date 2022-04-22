using UnityEngine.Events;

/// <summary>
/// Base wrapper class for all resources.
/// </summary>
[System.Serializable]
public class Resource
{
    private int currentValue;
    private int maxValue;

    public UnityEvent<int> onChangedCurrent;  // any script reacting to changes of the currentValue 
                                              // of this resource should listen to this
    public UnityEvent<int> onChangedMax;      // any script reacting to changes of the maxValue 
                                              // of this resource should listen to this

    public Resource(int startingValue, int maxValue)
    {
        currentValue = startingValue;
        this.maxValue = maxValue;
        onChangedCurrent = new UnityEvent<int>();
        onChangedMax = new UnityEvent<int>();
    }

    public int GetCurrent() => currentValue;

    public int GetMax() => maxValue;

    public int ChangeCurrent(int value)
    {
        currentValue += value;
        currentValue = currentValue < 0 ? 0 : currentValue > maxValue ? maxValue : currentValue;
        onChangedCurrent.Invoke(currentValue);
        return currentValue;
    }

    public int ChangeMax(int value)
    {
        maxValue = value >= 0 ? value : 0;
        onChangedMax.Invoke(maxValue);
        return maxValue;
    }

}
