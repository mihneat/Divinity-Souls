using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
	public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 3.0f;
        [SerializeField] bool isHoming = false;
        [SerializeField] float destroyTime = 0.0f;
        [SerializeField] float maxLifeTime = 30.0f;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] GameObject mainProjectileParticle = null;

        // [SerializeField] float rotationSpeed;
        // [SerializeField] float rotationMagnitude;

        Health target;
        float damage = 0.0f;
        GameObject instigator = null;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        private void Update()
        {
            if (!target)
                return;

            if (isHoming && !target.GetIsDead) {
                //transform.forward = Vector3.RotateTowards(
                //    transform.position.normalized,
                //    (target.transform.position - transform.position).normalized,
                //    rotationSpeed,
                //    rotationMagnitude
                //);
                
                transform.LookAt(GetAimLocation());
            }

            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
        }

        public void SetTarget(GameObject newInstigator, Health newTarget, float newDamage)
        {
            target = newTarget;
            damage = newDamage;
            instigator = newInstigator;

            Destroy(gameObject, maxLifeTime);
        }

        public Vector3 GetAimLocation()
        {
            CapsuleCollider targetCollider = target.GetComponent<CapsuleCollider>();
            if (!targetCollider)
                return target.transform.position;

            return target.transform.position + Vector3.up * targetCollider.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!target)
                return;

            if (other.GetComponent<Health>() != target)
                return;

            if (target.GetIsDead)
                return;

            target.TakeDamage(instigator, damage);
            StartCoroutine(DestroyProjectile());
        }
        
        private IEnumerator DestroyProjectile()
        {
            if (hitEffect)
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);

            if (mainProjectileParticle)
                Destroy(mainProjectileParticle);

            target = null;

            yield return new WaitForSeconds(destroyTime);

            Destroy(gameObject);
        }
    }
}
