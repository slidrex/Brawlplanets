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
        if(!isOwned) return;
        if(collider.TryGetComponent<PlayerEntity>(out PlayerEntity entity))
        {
            if(entity.netId == Owner) return;
            CmdHit(entity.netId, 50);
        }
        Destroy(Instantiate(collisionEffect, transform.position, Quaternion.identity), 1);
        NetworkServer.Destroy(gameObject);
    }
    [Command]
    private void CmdHit(uint playerNetId, int damage)
    {
        NetworkServer.spawned[playerNetId].gameObject.GetComponent<PlayerEntity>().Damage(damage);
    }
}
