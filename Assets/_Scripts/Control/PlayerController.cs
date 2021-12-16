using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;

namespace RPG.Control
{
	public class PlayerController : MonoBehaviour
    {
        private Mover mover;
        private Fighter fighter;
        private Camera mainCam;
        private Health health;

        private void Awake()
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mainCam = Camera.main;
        }

        private void Update()
        {
            if (health.GetIsDead)
                return;

            if (InteractWithCombat())
                return;

            if (InteractWithMovement())
                return;
        }

        private bool InteractWithCombat()
        {
            if (!fighter)
                return false;

            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            foreach (RaycastHit hit in hits) {
                CombatTarget hitTarget = hit.transform.GetComponent<CombatTarget>();

                if (!hitTarget)
                    continue;

                if (!fighter.CanAttack(hitTarget.gameObject))
                    continue;

                if (Input.GetMouseButton(0)) {
                    fighter.Attack(hitTarget.gameObject);
                }

                return true;
            }

            return false;
        }

        private bool InteractWithMovement()
        {
            if (!mover)
                return false;

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            if (hasHit && Input.GetMouseButton(0)) {
                mover.StartMoveAction(hit.point, 1.0f);
            }

            return hasHit;
        }

        private Ray GetMouseRay()
        {
            return mainCam.ScreenPointToRay(Input.mousePosition);
        }
    }
}
