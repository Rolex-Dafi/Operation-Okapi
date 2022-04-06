using UnityEngine.Events;

[System.Serializable]
public class Respect
{
    private int currentValue;

    public UnityEvent<int> onChanged;  // any script reacting to respect changes should listen to this

    public Respect(int startingValue)
    {
        currentValue = startingValue;
        onChanged = new UnityEvent<int>();
    }

    public int GetCurrent() => currentValue;

    public int ChangeCurrent(int value)
    {
        currentValue += value;
        currentValue = currentValue < 0 ? 0 : currentValue;
        onChanged.Invoke(currentValue);
        return currentValue;
    }
}
