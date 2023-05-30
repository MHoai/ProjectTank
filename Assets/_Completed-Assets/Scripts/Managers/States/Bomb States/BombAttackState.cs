using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Complete
{
public class BombAttackState : State
{
    public BombChaseState chaseState;
    public float m_ExplosionForce = 1000f;              // The amount of force added to a tank at the centre of the explosion.
    public float m_ExplosionRadius = 10f;
    public float m_MaxDamage = 100f;
    public LayerMask m_TankMask;

    public override State RunCurrentState(int m_TypeMob, Transform PlayerLocation, GameObject m_Instance, Transform m_MobSpawner, Transform m_MobGoTo, bool oneTankLeft) 
    {

        Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody> ();
            if (!targetRigidbody)
                continue;

            targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);

            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth> ();

            if (!targetHealth)
                continue;

            targetHealth.TakeDamage (30);
        }

        TankHealth MobHealth = m_Instance.GetComponent<TankHealth> ();
        MobHealth.TakeDamage (100);


        return this;
    }
}
}
