using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using RPG.Attributes;

namespace RPG.Control
{
	public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5.0f;
        [SerializeField] float suspicionTime = 2.0f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointStoppingDistance = 1.0f;
        [SerializeField] float waypointStoppingTime = 1.0f;
        [Range(0.0f, 1.0f)] [SerializeField] float patrolSpeedFraction = 0.2f;

        GameObject player;

        private Fighter fighter;
        private Health health;
        private Mover mover;
        private ActionScheduler actionScheduler;

        Vector3 guardInitialPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivalAtWaypoint = Mathf.Infinity;
        int currentWaypoint;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            actionScheduler = GetComponent<ActionScheduler>();
        }

        private void Start()
        {
            guardInitialPosition = transform.position;
        }

        private void Update()
        {
            if (health.GetIsDead)
                return;

            if (IsInAttackRange() && fighter.CanAttack(player)) {
                AttackBehaviour();
            } else if (timeSinceLastSawPlayer < suspicionTime) {
                SuspicionBehaviour();
            } else {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivalAtWaypoint += Time.deltaTime;
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0.0f;
            fighter.Attack(player);
        }

        private void SuspicionBehaviour()
        {
            actionScheduler.CancelCurrentAction();
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardInitialPosition;

            if (patrolPath) {
                if (AtWaypoint()) {
                    timeSinceArrivalAtWaypoint = 0.0f;
                    CycleWaypoint();
                }

                nextPosition = GetCurrentWaypoint();
            }


            if (timeSinceArrivalAtWaypoint >= waypointStoppingTime)
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointStoppingDistance;
        }

        private void CycleWaypoint()
        {
            currentWaypoint = patrolPath.GetNextWaypointIndex(currentWaypoint);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypoint);
        }

        private bool IsInAttackRange()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance;
        }
    }
}
