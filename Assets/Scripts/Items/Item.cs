/*public enum EItemState
{
    Full,      // full hp
    Damaged,
    Cracked,   // hp == 1 -> display cracked gfx in HUD
    Destroyed  // hp == 0
}*/

/// <summary>
/// Only for equippable items. 
/// </summary>
public class Item
{
    private Health health;

    public ItemSO Data { get; }
    //public EItemState ItemState { get; private set; }

    public int CurrentHealth => health.GetCurrent();


    public Item(ItemSO data)
    {
        Data = data;
        health = new Health(data.Health);
        //ItemState = EItemState.Full;
    }

    
    public void ReceiveDamage(int damage)
    {
        health.ChangeCurrent(-damage);
        var current = health.GetCurrent();
        //ItemState = current <= 0 ? EItemState.Destroyed : current == 1 ? EItemState.Cracked : EItemState.Damaged;
    }
}
