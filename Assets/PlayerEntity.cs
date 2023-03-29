using UnityEngine;
using Mirror;

public class PlayerEntity : NetworkBehaviour
{
    [SerializeField] private PlayerMovementController controller;
    [SerializeField] private UIHolder holder;
    public UIHolder UIHolder { get; private set; }
    [SerializeField] private Projectile projectile;
    [SerializeField] private FollowCanvas followCanvas;
    public FollowCanvas FollowCanvas;
    [SerializeField] private float projectileInterval = 0.3f;
    [SerializeField] private int maxChargeCount;
    [SerializeField] private float chargeRestoreTime;
    private float timeSinceChargeRestored;
    private int currentChargeCount;
    private float timeSinceProjectile;
    [SerializeField] private float chargePenaltyTime = 0.3f;
    private float timeSinceShootAction;
    [SyncVar] public string Nickname;
    [field:SerializeField, SyncVar] public int MaxHealth { get; private set; }
    [field:SerializeField, SyncVar] public int CurrentHealth { get; private set; }
    public override void OnStartLocalPlayer()
    {
        SetupLocalPlayer();
    }
    protected virtual void FixedUpdate()
    {
        if(!isLocalPlayer) return;

        if(timeSinceProjectile < projectileInterval) timeSinceProjectile += Time.fixedDeltaTime;
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
            FollowCanvas.SetChargeCount(currentChargeCount);
            timeSinceShootAction = 0.0f;
            CmdSpawnBullet(transform.position, netId, attackDirection);
        }
        
        if(timeSinceShootAction >= chargePenaltyTime)
        {
            ChargeRestoring();
        }
        else timeSinceShootAction += Time.deltaTime;
    }
    private void ChargeRestoring()
    {
        if(currentChargeCount < maxChargeCount)
            if(timeSinceChargeRestored < chargeRestoreTime) timeSinceChargeRestored += Time.deltaTime;
            else
            {
                currentChargeCount++;
                FollowCanvas.SetChargeCount(currentChargeCount);
                timeSinceChargeRestored = 0.0f;
            }
    }
    private void SetupLocalPlayer()
    {
        SetCurrentHealthCmd(gameObject, MaxHealth);
        CmdSetNickname(gameObject, GameLevelController.Nicknames[Random.Range(0, GameLevelController.Nicknames.Length)]);
        
        
        UIHolder = Instantiate(holder, Vector3.zero, Quaternion.Euler(20.0f, 0.0f, 0.0f));
        UIHolder.Camera.FollowObject = transform;
        
        CmdSpawnCanvas(gameObject);
        UIHolder.FollowCanvas = FollowCanvas;
    }
    public void SetupLocalCanvases()
    {
        if(!isLocalPlayer)
        {
            FollowCanvas.SetupCanvas(isEnemy: true);
        }
        else 
        {
            FollowCanvas.SetupCanvas(isEnemy: false);
            FollowCanvas.ShowChargeCount();
        }
        FollowCanvas.SetNickname(Nickname);
    }
    [Command]
    private void SetCurrentHealthCmd(GameObject player, int health) => player.GetComponent<PlayerEntity>().CurrentHealth = health;
    [Command]
    private void CmdSetNickname(GameObject player, string nickname) => player.GetComponent<PlayerEntity>().Nickname = nickname;
    [Command]
    private void CmdSpawnCanvas(GameObject playerObject) 
    {
        FollowCanvas obj = Instantiate(followCanvas.gameObject).GetComponent<FollowCanvas>();
        PlayerEntity player = playerObject.GetComponent<PlayerEntity>();

        obj.FollowObject = playerObject.transform;
        
        NetworkServer.Spawn(obj.gameObject, gameObject);
    }
    [Command]
    private void CmdSpawnBullet(Vector3 spawnPosition, uint ownerNetId, Vector2 moveVector)
    {
        var proj = Instantiate(projectile, spawnPosition, Quaternion.identity);
        proj.Owner = ownerNetId;
        

        proj.MoveVector = new Vector3(moveVector.x, 0.0f, moveVector.y);
        NetworkServer.Spawn(proj.gameObject);
    }
    public void Damage(int damage)
    {
        if(CurrentHealth - damage <= 0) OnDie();
        else CurrentHealth -= damage;
        
        FollowCanvas.CmdUpdateHealthbar(gameObject, CurrentHealth, MaxHealth);
    }
    public virtual void OnDie()
    {
        GameLevelController.ReloadScene();
    }
    public void OnGameLoad()
    {
        SetCurrentHealthCmd(gameObject, MaxHealth);
        controller.enabled = false;
        transform.position = new Vector3(Random.Range(-3.0f, 3.0f), 0.0f, Random.Range(-3.0f, 3.0f));
        controller.enabled = true;
    }
}
