using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    [SyncVar, HideInInspector] public Vector3 MoveVector;
    [SyncVar, HideInInspector] public uint Owner;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject collisionEffect;
    private Transform Transform;
    private float timeSinceSpawn;
    protected virtual void Start()
    {
        Transform = transform;
        Transform.rotation = Quaternion.Euler(0, Mathf.Atan2(-MoveVector.x, -MoveVector.z) * Mathf.Rad2Deg, 0);
    }
    protected virtual void FixedUpdate()
    {
        rb.velocity = (MoveVector.normalized * speed);
        if(timeSinceSpawn <= lifeTime) timeSinceSpawn += Time.fixedDeltaTime;
        else Destroy(gameObject);
    }
    protected virtual void OnTriggerEnter(Collider collider)
    {
        Destroy(Instantiate(collisionEffect, transform.position, Quaternion.identity), 1);
        if(collider.TryGetComponent<PlayerEntity>(out PlayerEntity entity))
        {
            if(entity.netId == Owner) return;
            else if(entity.isOwned)
            {
                entity.Damage(50);
            }
        }
        StartCoroutine(pidr());
    }
    private System.Collections.IEnumerator pidr()
    {
        yield return new WaitForSeconds(0.2f);
        int pidr;
        Destroy(gameObject);
    }
}
