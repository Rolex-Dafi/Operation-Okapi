using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Base class for all in-game characters.
/// </summary>
public class MerchantCharacter : Character
{
    private List<ItemSO> currentShop;

    public override void Init()
    {
        currentShop = (data as MerchantCharacterSO)?.shop.ToList();
        base.Init();
    }

    /// <summary>
    /// Sell given item if the provided amount is enough.
    /// </summary>
    /// <param name="money"></param>
    /// <returns></returns>
    public ItemSO Sell(int money)
    {
        return null;
    }

}
