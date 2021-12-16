using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
	[RequireComponent(typeof(NavMeshAgent))]
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxSpeed = 5.66f;

        private NavMeshAgent navMeshAgent;
        private Animator animator;
        private ActionScheduler actionScheduler;
        private Health health;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            health = GetComponent<Health>();
        }

        private void Update()
        {
            navMeshAgent.enabled = !health.GetIsDead;

            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;

            animator.SetFloat("forwardSpeed", speed);
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            actionScheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        public object CaptureState()
        {
            return new SerializableVector3[] { new SerializableVector3(transform.position),
                new SerializableVector3(transform.rotation.eulerAngles)};
        }

        public void RestoreState(object state)
        {
            SerializableVector3[] newPosition = (SerializableVector3[])state;

            navMeshAgent.enabled = false;
            transform.position = newPosition[0].ToVector();
            transform.rotation = Quaternion.Euler(newPosition[1].ToVector());
            navMeshAgent.enabled = true;

            actionScheduler.CancelCurrentAction();
        }
    }
}
