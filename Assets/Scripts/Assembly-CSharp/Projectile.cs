using UnityEngine;
public class Projectile : ConfigurableNetworkObject
{
    public ConfigurableNetworkObject objectToSpawn;

    protected bool hasSpawned;

    public string spawnItemOverride = string.Empty;

    private TeslaShield tesla;

    public bool ignoreOwner;

    public bool explodeOnCollision = true;

    public bool explodeOnTriggerEnter = true;

    [SerializeField]
    protected bool _tryToSpawnOnDisable;

    [SerializeField]
    protected bool _orientX;

    [SerializeField]
    protected bool _orientY;

    [SerializeField]
    protected bool _orientZ;

    [SerializeField]
    protected bool shakeScreen;

    [SerializeField]
    protected float shakeDuration;

    [SerializeField]
    protected float shakeStrength;

    [SerializeField]
    protected float explosionShakeRadius;

    [SerializeField]
    protected float shakeFunctionExponent = 3f;

    protected string EquipmentNames
    {
        get
        {
            return equipmentNames;
        }
    }

    protected virtual void OnTriggerEnter(Collider c)
    {
        tesla = c.GetComponent<TeslaShield>();
        if ((!(tesla != null) || !tesla.PlayerOnOwnersTeam(base.OwnerID)) && explodeOnTriggerEnter)
        {
            Explode(c.gameObject);
        }
    }

    protected virtual void OnCollisionEnter(Collision c)
    {
        if (explodeOnCollision)
        {
            Explode(c.gameObject);
        }
    }

    public virtual void Explode(GameObject objectHit)
    {
        if (ignoreOwner)
        {
            PlayerController component = objectHit.GetComponent<PlayerController>();
            if (component != null && component.OwnerID == base.OwnerID)
            {
                return;
            }
        }
        if (objectToSpawn != null && !hasSpawned)
        {
            CreateExplosion();
            hasSpawned = true;
        }
        DoCameraShake();
        TryDestroy();
    }

    protected virtual void CreateExplosion()
    {
        if (objectToSpawn == null)
        {
            Debug.LogError("Projectile: objectToSpawn prefab is null!");
            return;
        }

        // Pass OwnerID as instantiation data
        object[] instantiationData = new object[] { base.OwnerID };

        // Spawn projectile over Photon
        GameObject proj = PhotonNetwork.Instantiate(
            objectToSpawn.gameObject.name,
            transform.position,
            Quaternion.identity,
            0,
            instantiationData
        );

        if (proj == null)
        {
            Debug.LogError("Projectile: PhotonNetwork.Instantiate returned null!");
            return;
        }

        // Get the networked component
        ConfigurableNetworkObject component = proj.GetComponent<ConfigurableNetworkObject>();
        if (component != null)
        {
            component.DamageMultiplier = base.DamageMultiplier;
            component.SetItemOverride(spawnItemOverride);
            component.SetEquipmentNames(equipmentNames);
        }
        else
        {
            Debug.LogWarning("Projectile: spawned object has no ConfigurableNetworkObject component!");
        }

        // Orient projectile if Rigidbody exists
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null && (_orientX || _orientY || _orientZ))
        {
            proj.transform.LookAt(transform.position + rb.velocity.normalized);
            Vector3 eulerAngles = proj.transform.rotation.eulerAngles;
            if (!_orientX) eulerAngles.x = 0f;
            if (!_orientY) eulerAngles.y = 0f;
            if (!_orientZ) eulerAngles.z = 0f;
            proj.transform.eulerAngles = eulerAngles;
        }

        // Ignore Tesla collisions locally
        if (tesla != null)
        {
            Collider projCollider = proj.GetComponent<Collider>();
            if (projCollider != null)
            {
                Collider teslaCollider = tesla.PlayerController.DamageReceiver.GetComponent<Collider>();
                if (teslaCollider != null)
                    Physics.IgnoreCollision(teslaCollider, projCollider);
            }
        }

        // Optional: trigger camera shake
        DoCameraShake();
    }



    protected void DoCameraShake()
    {
        if (shakeScreen && Camera.main != null)
        {
            float f = (explosionShakeRadius - Vector3.Distance(base.transform.position, Camera.main.transform.position)) / explosionShakeRadius;
            float num = Mathf.Pow(f, shakeFunctionExponent);
            if (num > 0f)
            {
                ShakeCamera shakeCamera = Camera.main.gameObject.AddComponent<ShakeCamera>();
                shakeCamera.shakeDuration = shakeDuration;
                shakeCamera.shakeStrength = shakeStrength * num;
            }
        }
    }

    public virtual void TryDestroy()
    {
        Object.Destroy(base.gameObject);
    }

    public virtual void OnDisable()
    {
        if (_tryToSpawnOnDisable && objectToSpawn != null && !hasSpawned)
        {
            CreateExplosion();
        }
    }
}
