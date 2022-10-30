using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public float health;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttcked;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake() {
        player = GameObject.Find("character_3").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsGround);

        if(!playerInSightRange && !playerInAttackRange)
            Patroling();
        if(playerInSightRange && !playerInAttackRange)
            ChasePlayer();
        if(playerInSightRange && playerInAttackRange)
            AttackPlayer();
    }

    private void Patroling() {
        if(!walkPointSet)
            SearchWalkPoint();

        if(walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Distance reached
        if(distanceToWalkPoint.magnitude < 1f) 
        walkPointSet = false;
    }

    private void SearchWalkPoint() {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer() {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer() {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if(!alreadyAttcked) {
            // Attack code here
            Debug.Log("Boom! I attacked you!");

            alreadyAttcked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack() {
        alreadyAttcked = false;
    }

    public void TakeDamage(int damage) {
        health -= damage;
        if(health <= 0) 
            Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy() {
        Destroy(gameObject);
    }
}
