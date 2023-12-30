using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 120f;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Light lightView;

    //enemy stuffs
    [SerializeField] Transform player;
    [SerializeField] float patrolSpeed, chaseSpeed;
    [SerializeField] Transform[] points;
    int patrolIndex;

    [SerializeField] Animator anim;
    [Range(1f, 5.5f)]
    [SerializeField] float minDisToAttack;
    [Range(3.5f, 8.5f)]
    [SerializeField] float minDis;

    [SerializeField] PlayerMove target;
    private void Start()
    {
        lightView.spotAngle = viewAngle;
        StartCoroutine(FindTargetsWithDelay(0.2f));
    }
    private void Update()
    {
        if(Vector3.Distance(transform.position, player.position) > minDis && Vector3.Distance(transform.position, player.position) > minDisToAttack)
        {
            Patrol();
        }
    }
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }
    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward,dirToTarget) < viewAngle/2)
            {
                float disToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, disToTarget, targetMask))
                {
                    if (disToTarget > minDisToAttack)
                    {
                        FollowPlayer();
                    }
                    else if(disToTarget <= minDisToAttack)
                    {
                        StartCoroutine(Attack());
                    }
                    else
                    {
                        Patrol();
                    }

                    visibleTargets.Add(target);
                }
            }
        }
    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees*Mathf.Deg2Rad),0f,Mathf.Cos(angleInDegrees*Mathf.Deg2Rad));
    }
    public void Patrol()
    {
        anim.SetBool("isAttacking", false);
        agent.stoppingDistance = 0f;
        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            PatrolPoints();
        }
    }
    public void PatrolPoints()
    {
        agent.speed = 3.5f;
        agent.SetDestination(points[patrolIndex].position);
        patrolIndex = (patrolIndex + 1) % points.Length;
    }
    public IEnumerator Attack()
    {
        anim.SetBool("isAttacking", true);
        StartCoroutine(target.takingDamage(0.5f));
        agent.destination = this.transform.position;
        yield return new WaitForSeconds(2.3f);
    }
    public void FollowPlayer()
    {
        agent.speed = chaseSpeed;
        agent.stoppingDistance = 3.5f;
        agent.destination = player.transform.position;
        transform.LookAt(player);
    }

}
