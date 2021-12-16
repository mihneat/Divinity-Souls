using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        [Header("Gizmos")]
        [SerializeField] float waypointSphereSize = 0.5f;
        [SerializeField] Color waypointSphereColor = Color.white;

        private void OnDrawGizmos()
        {
            Gizmos.color = waypointSphereColor;
            for (int i = 0; i < transform.childCount; ++i) {
                Vector3 currentWaypoint = GetWaypoint(i);
                Vector3 nextWaypoint = GetWaypoint(GetNextWaypointIndex(i));

                Gizmos.DrawSphere(currentWaypoint, waypointSphereSize);
                Gizmos.DrawLine(currentWaypoint, nextWaypoint);
            }
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }

        public int GetNextWaypointIndex(int i)
        {
            return (i + 1) % transform.childCount;
        }
    }
}
