using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;

public class EnemyLogic : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform player;
    [SerializeField] float patrolSpeed, chaseSpeed;
    [SerializeField] Transform[] points;
    int patrolIndex;
    public float dis2target;

    [SerializeField] Animator anim;
    [Range(1f, 5.5f)]
    [SerializeField] float minDisToAttack;
    [Range(3.5f, 8.5f)]
    [SerializeField] float minDis;

    [SerializeField] PlayerMove target;

    [SerializeField] PostProcessVolume volume;
    private ChromaticAberration aberration;

    // Start is called before the first frame update
    public void Start()
    {
        volume.profile.TryGetSettings(out aberration);
    }

    // Update is called once per frame
    void Update()
    {
        PatrolPoints();
    }
    public void Patrol()
    {
        anim.SetBool("isAttacking", false);
        if(!agent.pathPending && agent.remainingDistance <= 0.5f)
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
        target.takingDamage(0.5f);
        agent.destination = this.transform.position;
        aberration.active = true;
        yield return new WaitForSeconds(2.3f);
        aberration.active = false;
    }
    public void FollowPlayer()
    {
        agent.speed = chaseSpeed;
        agent.stoppingDistance = 3.5f;
        agent.destination = player.transform.position;
        transform.LookAt(player);
    }
}
