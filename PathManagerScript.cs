using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class PathManagerScript : MonoBehaviour
{
    public Path[] paths;
    public bool startAtPosition0;
    [HideInInspector] public bool usingNavMesh;
    private NavMeshAgent agent;
    public UnityEvent onPathStart;
    public UnityEvent onPathPaused;
    public UnityEvent onPathStop;
    public UnityEvent onPathEnd;

    private bool pathStarted;
    private int pathIndex;
    private bool foundDestination;

    // Start is called before the first frame update
    void Start()
    {
        usingNavMesh = TryGetComponent<NavMeshAgent>(out NavMeshAgent component);
        if (usingNavMesh == true)
        {
            agent = component;
        }

        pathIndex = 0;
        foundDestination = false;

        foreach(Path path in paths)
        {
            path.startingWaitTime = path.waitTime;
        }
    }

    #region //Path Functions
    //Starts the path.
    public void StartPath()
    {
        onPathStart.Invoke();
        if (startAtPosition0 == true)
        {
            transform.position = paths[0].pathPoint.position;
            pathIndex = 1;
        }
        pathStarted = true;
    }

    //Pauses the path.
    public void PausePath(bool isPaused)
    {
        if (isPaused)
        {
            onPathPaused.Invoke();
            pathStarted = false;
        }
        else
        {
            pathStarted = true;
        }
    }

    //Stops the path completely.
    public void StopPath(bool resetPaths)
    {
        onPathStop.Invoke();
        pathStarted = false;
        if (resetPaths == true)
        {
            ResetPaths(false);
        }
    }

    //Goes to the next path point in the list of paths.
    public void GoToNextPathPoint()
    {
        pathIndex += 1;
        foundDestination = false;
    }

    //Changes the destination of the character such that it goes to a specific path point.
    public void GoToPathPoint(int pathNumber)
    {
        pathIndex = pathNumber;
        foundDestination = false;
    }

    //Resets all the pathpoint wait times (if they have any).
    //Start After Reset will start the path after everything value in the path has been reset.
    public void ResetPaths(bool startAfterReset)
    {
        pathIndex = 0;
        foundDestination = false;

        for (int i = 0; i < paths.Length; i++)
        {
            if (paths[i].waitBeforeNextPoint == true)
            {
                paths[i].waitTime = paths[i].startingWaitTime;
            }
        }

        if (startAfterReset == true)
        {
            StartPath();
        }
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        if (pathStarted == true)
        {
            //Not using NavMeshAgent
            if (!usingNavMesh)
            {
                if (transform.position != paths[pathIndex].pathPoint.position)
                {
                    transform.position = Vector3.MoveTowards(transform.position, paths[pathIndex].pathPoint.position, paths[pathIndex].travelSpeed * Time.deltaTime);
                }
                else
                {
                    paths[pathIndex].onHitPoint.Invoke();
                    if (paths[pathIndex].waitBeforeNextPoint == true)
                    {
                        paths[pathIndex].waitTime -= Time.deltaTime;
                        if (paths[pathIndex].waitTime <= 0)
                        {
                            if (pathIndex < paths.Length - 1)
                            {
                                GoToNextPathPoint();
                            }
                            else
                            {
                                onPathEnd.Invoke();
                                pathStarted = false;
                            }
                        }
                    }
                    else
                    {
                        if (pathIndex < paths.Length - 1)
                        {
                            GoToNextPathPoint();
                        }
                        else
                        {
                            onPathEnd.Invoke();
                            pathStarted = false;
                        }
                    }
                }
                
            }

            //Using NavMesh
            else
            {
                if (!foundDestination)
                {
                    agent.destination = paths[pathIndex].pathPoint.position;
                    foundDestination = true;
                }
                else
                {
                    if (transform.position == agent.destination)
                    {
                        paths[pathIndex].onHitPoint.Invoke();
                        if (paths[pathIndex].waitBeforeNextPoint == true)
                        {
                            paths[pathIndex].waitTime -= Time.deltaTime;
                            if (paths[pathIndex].waitTime <= 0)
                            {
                                if (pathIndex < paths.Length - 1)
                                {
                                    GoToNextPathPoint();
                                }
                                else
                                {
                                    onPathEnd.Invoke();
                                    pathStarted = false;
                                }
                            }
                        }
                        else
                        {
                            if (pathIndex < paths.Length - 1)
                            {
                                GoToNextPathPoint();
                            }
                            else
                            {
                                onPathEnd.Invoke();
                                pathStarted = false;
                            }
                        }
                    }
                }         
            }
        }
    }   
}

[System.Serializable]
public class Path
{
    [Tooltip("The point in path the character will walk to.")]public Transform pathPoint;
    [Tooltip("The travel speed walking from one point to the next.")]public float travelSpeed;
    [Tooltip("If true, the character will wait at this point for the set amount of time listed in WaitTime.")] public bool waitBeforeNextPoint; 
    [Tooltip("If WaitBeforeNextPoint is true, this is the amount of time the character will wait at the current point before traveling to the next point.")] public float waitTime;
    [HideInInspector] public float startingWaitTime;
    [Tooltip("Event trigger for when the character arrives at this point in the path.")] public UnityEvent onHitPoint;
}
