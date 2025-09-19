using System.Collections;
using UnityEngine;

public class DelayedGravityProjectile : Photon.MonoBehaviour
{
    public float delay = 0.5f;
    public float gravityAmount = 500f;
    public bool lookAtDirection = true;

    [HideInInspector]
    public int ownerID = -1;

    public Rigidbody myRigidbody;
    public Transform myTransform;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myTransform = transform;

        if (!lookAtDirection)
            enabled = false;
    }

    void Start()
    {
        PhotonView pv = GetComponent<PhotonView>();
        if (pv != null && pv.instantiationData != null)
        {
            ownerID = (int)pv.instantiationData[0];
        }
        else
        {
            Debug.LogWarning("Projectile: PhotonView missing or no instantiation data!");
        }
    }

    private void OnNetworkDelay(float timeAlreadyElapsed)
    {
        StartCoroutine(delayedGravity(delay - timeAlreadyElapsed));
    }

    private IEnumerator delayedGravity(float actualDelay)
    {
        yield return new WaitForSeconds(actualDelay);
        ConstantForce c = gameObject.AddComponent<ConstantForce>();
        c.force = new Vector3(0, -gravityAmount, 0);
    }

    private void OnCollisionEnter()
    {
        enabled = false;
    }

    private void OnTriggerEnter()
    {
        enabled = false;
    }

    void LateUpdate()
    {
        if (myRigidbody != null)
            myTransform.LookAt(myTransform.position + myRigidbody.velocity);
    }
}
