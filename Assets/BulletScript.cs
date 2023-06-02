// not used, delete later
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

using UnityEngine;

public class BulletScript : NetworkBehaviour
{
    [SerializeField] private float speed = 10f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        GetComponent<Rigidbody>().velocity = this.transform.forward * speed;
    }
}
