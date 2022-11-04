using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Main controller for every trap in the scene.
/// </summary>
public class TrapController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [FormerlySerializedAs("childrenTriggers")] [SerializeField] private TrapDamager[] childrenDamagers;

    [SerializeField] private TrapSO data;
    
    /// <summary>
    /// Initializes the controller.
    /// </summary>
    /// <param name="data">The trap data</param>
    public void Init(TrapSO data)
    {
        this.data = data;
        
        // the actual triggers are on child(ren) - or spawned at runtime if this is empty
        foreach (var child in childrenDamagers)
        {
            child.Init(data);
        }

        tag = Utility.enemyTagAndLayer;
    }

    /// <summary>
    /// Plays an animation which activates the trap.
    /// </summary>
    public void ActivateTrap()
    {
        // play animation - the animation should enable the hitbox
        animator.SetTrigger(Utility.activateTrigger);
    }

    /// <summary>
    /// Activates the trap while taking into account the target.
    /// </summary>
    /// <param name="target">The target of the trap</param>
    public void ActivateTrap(Vector3 target)
    {
        ActivateTrap();
        SpawnProjectiles(target);
    }
    
    private void SpawnProjectiles(Vector3 target)
    {
        ProjectileController instance = Instantiate(data.projectilePrefab);
        // Player and enemies have to be tagged correctly for this to work !
        instance.Init(data, tag);

        instance.ShootFromSky(target);
    }

}
