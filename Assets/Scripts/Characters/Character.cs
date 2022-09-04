using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all in-game characters.
/// </summary>
public class Character : MonoBehaviour
{
    // scriptable objects
    [SerializeField] protected CharacterSO data;

    // resources
    protected Money money;

    // exposed vars
    public Animator Animator { get; private set; }
    public Money Money { get => money; private set => money = value; }
    public CharacterSO Data => data;

    public virtual void Init()
    {
        Money = new Money(data.money);
        Animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        money.CleanUp();
    }

}
