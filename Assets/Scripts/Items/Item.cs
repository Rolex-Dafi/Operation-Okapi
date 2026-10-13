/// <summary>
/// Only for equippable items. 
/// </summary>
public class Item
{
    private Health health;

    public ItemSO Data { get; }

    public int CurrentHealth => health.GetCurrent();


    /// <summary>
    /// Creates an item instance.
    /// </summary>
    /// <param name="data"></param>
    public Item(ItemSO data)
    {
        Data = data;
        health = new Health(data.Health);
    }

    /// <summary>
    /// Deal damage to this item.
    /// </summary>
    /// <param name="damage"></param>
    public void ReceiveDamage(int damage)
    {
        health.ChangeCurrent(-damage);
        var current = health.GetCurrent();
    }
}
