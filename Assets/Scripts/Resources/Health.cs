using UnityEngine.Events;

[System.Serializable]
public class Health
{
    private int maxValue;
    private int currentValue;

    public UnityEvent<int> onChanged;  // any script reacting to health changes should listen to this

    public Health(int maxValue)
    {
        this.maxValue = maxValue;
        currentValue = maxValue;
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
