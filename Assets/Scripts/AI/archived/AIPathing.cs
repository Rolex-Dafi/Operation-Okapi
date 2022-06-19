using Pathfinding;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Seeker), typeof(AggressiveCharacter))]
public class AIPathing : MonoBehaviour
{
    // required components
    private AggressiveCharacter character;
    private Seeker seeker;

    // pathfinding
    private bool currentlyMoving = false;
    private Path currentPath;
    private int nextWaypoint = 0;
    private float nextWaypointSqrtDistance = 0.01f;
    private UnityAction onDestinationReached;
    // for debug purposes
    private Vector3 targetPosition;


    void Start()
    {
        character = GetComponent<AggressiveCharacter>();
        seeker = GetComponent<Seeker>();
    }

    private void Update()
    {
        /*if (GetTarget())
        {
            seeker.StartPath(transform.position, targetPosition, OnPathComplete);
        }*/

        if (currentlyMoving)
        {
            FollowPath();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="onDestinationReached"></param>
    public void MoveTo(Transform target, UnityAction onDestinationReached)
    {
        if (currentlyMoving) return;

        this.onDestinationReached = onDestinationReached;

        seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    private void OnPathComplete(Path path)
    {
        if (path.error) return;

        currentlyMoving = true;
        currentPath = path;
        nextWaypoint = 0;
    }

    private void FollowPath()
    {
        // check if we want to switch to next waypoint
        float distToWaypoint = 0;
        while(true)
        {
            distToWaypoint = (transform.position - currentPath.vectorPath[nextWaypoint]).sqrMagnitude;
            if (distToWaypoint < nextWaypointSqrtDistance)
            {
                if (nextWaypoint + 1 < currentPath.vectorPath.Count)
                {
                    ++nextWaypoint;
                }
                else
                {
                    currentlyMoving = false;
                    character.Move(Vector2.zero);
                    onDestinationReached.Invoke();
                    Debug.Log("End of path reached");
                    return;
                }
            }
            else
            {
                break;
            }
        }

        Vector2 movementDir = (currentPath.vectorPath[nextWaypoint] - transform.position).normalized;
        character.Move(movementDir);
    }



    // for debug purposes
    private bool GetTarget()
    {
        if (Input.GetMouseButtonDown(0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
            Debug.Log("mouse world: " + targetPosition);
            return true;
        }
        return false;
    }
}
