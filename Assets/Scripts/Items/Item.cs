/// <summary>
/// Only for equippable items. 
/// </summary>
public class Item
{
    private Health health;

    public ItemSO Data { get; }
    //public EItemState ItemState { get; private set; }

    public int CurrentHealth => health.GetCurrent();


    /// <summary>
    /// Creates an item instance.
    /// </summary>
    /// <param name="data"></param>
    public Item(ItemSO data)
    {
        Data = data;
        health = new Health(data.Health);
        //ItemState = EItemState.Full;
    }

    /// <summary>
    /// Deal damage to this item.
    /// </summary>
    /// <param name="damage"></param>
    public void ReceiveDamage(int damage)
    {
        health.ChangeCurrent(-damage);
        var current = health.GetCurrent();
        //ItemState = current <= 0 ? EItemState.Destroyed : current == 1 ? EItemState.Cracked : EItemState.Damaged;
    }
}
