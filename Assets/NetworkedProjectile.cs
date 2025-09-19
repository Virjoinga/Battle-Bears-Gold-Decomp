using UnityEngine;

public class NetworkedProjectile : Photon.MonoBehaviour
{
    public int ownerID = -1;
    public float projectileSpeed = 10f;

    private Rigidbody rb;
    private Vector3 networkPosition;
    private Vector3 networkVelocity;
    private bool isOwner;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        isOwner = photonView.isMine;

        if (!isOwner)
        {
            rb.isKinematic = true; // Non-owners do not simulate physics
        }
    }

    void Start()
    {
        networkPosition = transform.position;
        if (rb != null)
            networkVelocity = rb.velocity;
    }

    void Update()
    {
        if (!isOwner)
        {
            // Smoothly interpolate position & velocity
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 15f);
            if (rb != null)
                rb.velocity = networkVelocity;
        }
        else
        {
            // Owner periodically sends state to others
            if (rb != null)
                photonView.RPC("UpdateNetworkState", PhotonTargets.Others, transform.position, rb.velocity);

        }
    }


    public void UpdateNetworkState(Vector3 pos, Vector3 vel)
    {
        networkPosition = pos;
        networkVelocity = vel;
    }

    public void Initialize(int owner, Vector3 initialVelocity)
    {
        ownerID = owner;
        if (rb != null)
        {
            rb.velocity = initialVelocity;
            if (initialVelocity != Vector3.zero)
                transform.LookAt(transform.position + initialVelocity);
        }
    }
}
