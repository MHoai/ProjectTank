using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Complete
{
public class EliteSpecialAttackState : State
{
    private float m_ExplosionForce = 6000f;
    private float m_ExplosionRadius = 5f;
    public LayerMask m_TankMask;
    public EliteChaseState chaseState;

    private bool run_once_lock = true;
    private Vector3 check_stop = new Vector3(0.0f, 0.0f, 0.0f);
    private float TimeLeft = 5f;

    public override State RunCurrentState(int m_TypeMob, Transform PlayerLocation, GameObject m_Instance, Transform m_MobSpawner, Transform m_MobGoTo, bool oneTankLeft) 
    {
        NavMeshAgent agent = m_Instance.GetComponent<NavMeshAgent> ();
        agent.speed = 500f;

        TankHealth targetHealth = m_Instance.GetComponent<TankHealth> ();

        Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody> ();
            if (!targetRigidbody)
                continue;

            targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);

            if (!targetHealth)
                continue;

            //targetHealth.TakeDamage (30);
        }

        if (run_once_lock == true)
        {
            MobMovement m_Movement = m_Instance.GetComponent<MobMovement> ();
            Vector3 vector_mob_to_player = PlayerLocation.position - transform.position;
            vector_mob_to_player = vector_mob_to_player.normalized;
            vector_mob_to_player = 10f * vector_mob_to_player;
            Vector3 desired_place = vector_mob_to_player + transform.position;
            check_stop = desired_place;

            m_Movement.Move_enemy(desired_place);

            run_once_lock = false;
        }
        //We are at disired place
        if ((transform.position.x > check_stop.x -0.1f && transform.position.x < check_stop.x + 0.1f &&
            transform.position.z > check_stop.z -0.1f && transform.position.z < check_stop.z + 0.1f)
            || TimeLeft < 0f)
            {
                run_once_lock = true;
                agent.speed = 5f;
                TimeLeft = 5f;
                return chaseState;
            }
        Debug.Log(TimeLeft);
        TimeLeft -= Time.deltaTime;
        return this;
    }
}
}
