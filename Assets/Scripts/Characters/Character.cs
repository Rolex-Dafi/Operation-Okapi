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

    // inventory/drops

    // components
    [HideInInspector] protected Animator animator;

    // exposed vars
    public Animator Animator { get => animator; set => animator = value; }
    public Money Money { get => money; protected set => money = value; }
    public CharacterSO Data { get => data; protected set => data = value; }


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
