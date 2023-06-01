using UnityEngine;

namespace Complete
{
    public class Item : MonoBehaviour
    {


        private Rigidbody m_Rigidbody;              // Reference used to move the tank.
        private Collider m_Collider;
        public int num_Hp;
        public float num_Speed;
        public float num_Launch;
        public LayerMask m_TankMask;// Used to filter what the explosion affects, this should be set to "Players".
        //private GameObject m_particalGameObject;
        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Collider = GetComponent<Collider>();
            m_Collider.isTrigger = true;             // Set the collider to be a trigger.
        }

        private void OnTriggerEnter(Collider other)
        {
            // Kiểm tra nếu va chạm với tank
            if (other.GetComponent<TankHealth>())
            {
                int randomNumber = Random.Range(1, 4);
                if (randomNumber == 1)// increase HP
                {
                    Transform electroTransform = other.transform.Find("Love_aura"); // Tìm Prefab con "Love_aura" trong Prefab "tank"

                    //if (electroTransform != null)
                    //{
                    GameObject electroPrefab = electroTransform.gameObject;

                    ParticleSystem particle_elec = electroPrefab.GetComponent<ParticleSystem>();
                    if (particle_elec != null)
                    {
                        particle_elec.Play();
                    }
                    TankHealth health = other.GetComponent<TankHealth>();
                    if (health != null)
                    {
                        // Do something when the tank of the same player enters the trigger area.
                        // Example: heal the tank.
                        health.BuffHP(num_Hp);
                    }
                }
                else if (randomNumber == 2)//increase Speed
                {
                    TankMovement move = other.GetComponent<TankMovement>();
                    Transform electroTransform = other.transform.Find("Electro"); // Tìm Prefab con "Electro" trong Prefab "tank"

                    //if (electroTransform != null)
                    //{
                    GameObject electroPrefab = electroTransform.gameObject;

                    ParticleSystem particle_elec = electroPrefab.GetComponent<ParticleSystem>();
                    if (particle_elec != null)
                    {
                        particle_elec.Play();
                    }
                    //}
                    if (move != null)
                    {
                        // Do something when the tank of the same player enters the trigger area.
                        // Example: heal the tank.
                        move.setSpeed(num_Speed);

                    }
                }
                else if (randomNumber == 3)//increase Damage of shell
                {
                    TankShooting shoot = other.GetComponent<TankShooting>();
                    if (shoot != null)
                    {
                        // Do something when the tank of the same player enters the trigger area.
                        // Example: heal the tank.
                        shoot.setLaunchForce(num_Launch);
                    }
                }
                gameObject.SetActive(false);
                /*
                // Kiểm tra xem đối tượng Item có tồn tại không
                Item item = other.gameObject.GetComponent<Item>();
                if (item != null)
                {
                    // Biến mất item
                    Destroy(item.gameObject);
                }*/
            }
        }


        private void OnEnable()
        {
            // When the tank is turned on, make sure it's not kinematic.
            m_Rigidbody.isKinematic = false;
        }


        private void OnDisable()
        {
            // When the tank is turned off, set it to kinematic so it stops moving.
            m_Rigidbody.isKinematic = true;
        }


       /* private void Start()
        {

        }


        private void Update()
        {

        }*/

    }
}
