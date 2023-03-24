using UnityEngine;
using Mirror;

public class PlayerEntity : NetworkBehaviour
{
    [SerializeField] private PlayerMovementController controller;
    [SerializeField] private UIHolder holder;
    public UIHolder UIHolder { get; private set; }
    [SerializeField] private Projectile projectile;
    private const float projectileInterval = 0.6f;
    [SerializeField] private int maxChargeCount;
    [SerializeField] private float chargeRestoreTime;
    private float timeSinceChargeRestored;
    private int currentChargeCount;
    private float timeSinceProjectile;
    public override void OnStartLocalPlayer()
    {
        UIHolder = Instantiate(holder, Vector3.zero, Quaternion.Euler(20.0f, 0.0f, 0.0f));
        UIHolder.Camera.FollowObject = transform;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(UIHolder.Camera.FollowObject.gameObject);
    }
    private void Update()
    {
        if(!isLocalPlayer) return;
        
        Vector2 attackDirection = controller.InputWay == PlayerMovementController.InputMode.PC ? new Vector2(Input.mousePosition.x - Screen.width/2, Input.mousePosition.y - Screen.height/2) : UIHolder.AttackJoystick.Horizontal * Vector2.right + UIHolder.AttackJoystick.Vertical * Vector2.up;

        bool entryCondition = controller.InputWay == PlayerMovementController.InputMode.PC ? Input.GetKey(KeyCode.Mouse0) : true;
        
        if(entryCondition && attackDirection != Vector2.zero && timeSinceProjectile >= projectileInterval && currentChargeCount > 0)
        {
            timeSinceProjectile = 0.0f;
            attackDirection.Normalize();
            currentChargeCount--;
            print(currentChargeCount);
            UIHolder.ChargeCounter.text = currentChargeCount.ToString();
            SpawnBulletServer(attackDirection);
        }
        if(currentChargeCount < maxChargeCount)
        {
            if(timeSinceChargeRestored < chargeRestoreTime) timeSinceChargeRestored += Time.deltaTime;
            else
            {
                currentChargeCount++;
                timeSinceChargeRestored = 0.0f;
                UIHolder.ChargeCounter.text = currentChargeCount.ToString();
            }
        }
    }
    [Command]
    private void SpawnBulletServer(Vector2 spawnPos)
    {
        SpawnBulletClients(vectorPos: spawnPos);
    }
    [ClientRpc]
    private void SpawnBulletClients(Vector2 vectorPos)
    {
        var proj = Instantiate(projectile, transform.position, Quaternion.identity);
        proj.Owner = this;
        

        proj.MoveVector = new Vector3(vectorPos.x, 0.0f, vectorPos.y);
    }
    protected virtual void FixedUpdate()
    {
        if(!isLocalPlayer) return;
        if(timeSinceProjectile < projectileInterval) timeSinceProjectile += Time.fixedDeltaTime;
    }
    public virtual void OnDie()
    {
        controller.enabled = false;
        transform.position = new Vector3(Random.Range(-3.0f, 3.0f), 0.0f, Random.Range(-3.0f, 3.0f));
        controller.enabled = true;
    }
}
