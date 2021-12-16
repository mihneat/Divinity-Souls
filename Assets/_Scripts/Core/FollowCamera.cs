using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform target;

        Vector3 offset;

        private Camera mainCam;

        private void Awake()
        {
            mainCam = Camera.main;
            offset = mainCam.transform.position - target.position;
        }

        private void LateUpdate()
        {
            mainCam.transform.position = target.position + offset;
        }
    }
}
