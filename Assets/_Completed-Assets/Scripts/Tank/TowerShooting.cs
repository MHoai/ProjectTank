using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class TowerShooting : MonoBehaviour
    {
        public Rigidbody m_Shell;                   // Prefab of the shell.
        public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
        public Transform m_FireTransform_1;           // A child of the tower where the shells are spawned.
        public Transform m_FireTransform_2;           // A child of the tower where the shells are spawned.
        public Transform m_FireTransform_3;           // A child of the tower where the shells are spawned.
        public Transform m_FireTransform_4;           // A child of the tower where the shells are spawned.

        public float m_FireInterval = 2f;            // Interval between automatic firing.
        private float m_NextFireTime;                // Time when the next shot can be fired.
        public float m_MinLaunchForce = 25f;        // The force given to the shell if the fire button is not held.
        public float m_MaxLaunchForce = 10f;        // The force given to the shell if the fire button is held for the max charge time.
      

              // How fast the launch force increases, based on the max charge time.


        private void OnEnable()
        {
        }


        private void Start()
        {
            m_NextFireTime = Time.time;
        }


        private void Update()
        {
            // Check if the next shot time has been reached.
            if (Time.time >= m_NextFireTime)
            {
                // Fire the shell.
                Fire();

                // Calculate the time for the next shot.
                m_NextFireTime = Time.time + m_FireInterval;
            }
        }


        private void Fire()
        {
            // Create an instance of the shell and store a reference to it's rigidbody.
            Rigidbody shellInstance_1 =
                Instantiate(m_Shell, m_FireTransform_1.position, m_FireTransform_1.rotation) as Rigidbody;
            Rigidbody shellInstance_2 =
                Instantiate(m_Shell, m_FireTransform_2.position, m_FireTransform_2.rotation) as Rigidbody;
            Rigidbody shellInstance_3 =
                Instantiate(m_Shell, m_FireTransform_3.position, m_FireTransform_3.rotation) as Rigidbody;
            Rigidbody shellInstance_4 =
                Instantiate(m_Shell, m_FireTransform_4.position, m_FireTransform_4.rotation) as Rigidbody;

            // Set the shell's velocity to the launch force in the fire position's forward direction.
            shellInstance_1.velocity = m_MinLaunchForce * m_FireTransform_1.forward;
            shellInstance_2.velocity = m_MinLaunchForce * m_FireTransform_2.forward;
            shellInstance_3.velocity = m_MaxLaunchForce * m_FireTransform_3.forward;
            shellInstance_4.velocity = m_MaxLaunchForce * m_FireTransform_4.forward;

        }
    }
}