using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    [SyncVar, HideInInspector] public Vector3 MoveVector;
    [SyncVar, HideInInspector] public uint Owner;
    private Transform Transform;
    private float timeSinceSpawn;
    protected virtual void Start()
    {
        Transform = transform;
    }
    protected virtual void FixedUpdate()
    {
        Transform.Translate(MoveVector * speed * Time.fixedDeltaTime);
        if(timeSinceSpawn <= lifeTime) timeSinceSpawn += Time.fixedDeltaTime;
        else Destroy(gameObject);
    }
    protected virtual void OnTriggerEnter(Collider collider)
    {
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
