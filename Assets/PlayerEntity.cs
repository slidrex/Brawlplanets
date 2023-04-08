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
    [SerializeField] private Animator _animator;
    private float timeSinceChargeRestored;
    private int currentChargeCount;
    private float timeSinceProjectile;
    [SerializeField] private float chargePenaltyTime = 0.3f;
    private float timeSinceShootAction;
    [SyncVar] public string Nickname;
    [SyncVar] private float ultimateStatus;
    public float UltimateStatus { get => ultimateStatus; set => ultimateStatus = value; }
    private const float UltimateTreshold = 100.0f;
    [SerializeField] private float ultimateRestoreSpeed;
    [field:SerializeField, SyncVar] public int MaxHealth { get; private set; }
    [SyncVar(hook = nameof(ChangeHealthCallback))] public int CurrentHealth;
    public override void OnStartLocalPlayer()
    {
        SetupLocalPlayer();
        UIHolder.UltimateJoystick.OnUltimateRelease += OnUltimateJoystickReleased;
    }
    protected virtual void FixedUpdate()
    {
        if(!isLocalPlayer) return;

        if(timeSinceProjectile < projectileInterval) timeSinceProjectile += Time.fixedDeltaTime;
    }
    private void Update()
    {
        if(!isLocalPlayer) return;

        if(UltimateStatus < 100) HandleUltimate();


        Vector2 attackDirection = controller.InputWay == PlayerMovementController.InputMode.PC ? GetPointAttackVector() : UIHolder.AttackJoystick.Horizontal * Vector2.right + UIHolder.AttackJoystick.Vertical * Vector2.up;

        bool entryCondition = controller.InputWay == PlayerMovementController.InputMode.PC ? Input.GetKey(KeyCode.Mouse0) : true;
        
        if(entryCondition && attackDirection != Vector2.zero && timeSinceProjectile >= projectileInterval && currentChargeCount > 0)
        {
            _animator.SetTrigger("Attack");
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
    private Vector2 GetPointAttackVector()
    {
        Ray ray = UIHolder.Camera.RenderCamera.ScreenPointToRay(Input.mousePosition);
        
        Vector3 rayOriginPosition = ray.origin;
        Vector3 rayEndPosition = rayOriginPosition;
        
        Vector3 delta = ray.direction;
        float dCount = (transform.position.y - rayEndPosition.y)/delta.y;
        dCount = Mathf.Abs(dCount);
        rayEndPosition += delta * dCount;

        Vector3 distance = rayEndPosition - transform.position;
        
        distance.y = distance.z;
        return distance;
    }
    private void HandleUltimate()
    {
        UltimateStatus += Time.deltaTime * ultimateRestoreSpeed;
        UIHolder.UltimateJoystick.SetJoystickFillAmount(UltimateStatus, UltimateTreshold);
        if(UltimateStatus >= 100)
        {
            UIHolder.UltimateJoystick.SetJoystickType(isReady: true);
        }
    }
    private void OnUltimateJoystickReleased()
    {
        if(ultimateStatus >= 100) UseUltimate();
    }
    private void UseUltimate()
    {
        ultimateStatus = 0;
        currentChargeCount = 3;
        FollowCanvas.SetChargeCount(currentChargeCount);
        UIHolder.UltimateJoystick.SetJoystickType(isReady: false);
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
        CmdSetNickname(gameObject, GameLevelController.Nicknames[Random.Range(0, GameLevelController.Nicknames.Length)]);
        
        UIHolder = Instantiate(holder, Vector3.zero, Quaternion.Euler(20.0f, 0.0f, 0.0f));
        UIHolder.Camera.FollowObject = transform;
        
        CmdSpawnCanvas(gameObject);
    }
    [Command]
    private void CmdSetNickname(GameObject player, string nickname) => player.GetComponent<PlayerEntity>().Nickname = nickname;
    [Command]
    private void CmdSpawnCanvas(GameObject player) 
    {
        FollowCanvas obj = Instantiate(followCanvas);
        
        obj.FollowTransform = player.transform;

        NetworkServer.Spawn(obj.gameObject, player.GetComponent<NetworkIdentity>().connectionToClient);
    }
    public void SetupLocalCanvas(bool isEnemy)
    {
        FollowCanvas.SetupCanvas(isEnemy: isEnemy);
        
        if(!isEnemy)
        {
            FollowCanvas.ShowChargeCount();
            UIHolder.FollowCanvas = FollowCanvas;
        }
        
        FollowCanvas.SetNickname(Nickname);
        if(isOwned) CmdSetHealth(netId, MaxHealth);
    }
    [Command]
    private void CmdSpawnBullet(Vector3 spawnPosition, uint ownerNetId, Vector2 moveVector)
    {
        var proj = Instantiate(projectile, spawnPosition, Quaternion.identity);
        proj.Owner = ownerNetId;
        

        proj.MoveVector = new Vector3(moveVector.x, 0.0f, moveVector.y);
        NetworkServer.Spawn(proj.gameObject, NetworkServer.spawned[ownerNetId].connectionToClient);
    }
    private void ChangeHealthCallback(int oldHealth, int newHealth)
    {
        if(CurrentHealth <= 0 && isOwned) ReloadScene();
        FollowCanvas.RpcUpdateHealthBar(gameObject, CurrentHealth, MaxHealth);
    }
    public void Damage(int damage)
    {
        CurrentHealth -= damage;
    }
    [Command]
    private void CmdSetHealth(uint netId, int health)
    {
        PlayerEntity player = NetworkServer.spawned[netId].GetComponent<PlayerEntity>();
        player.CurrentHealth = health;
    }
    public virtual void OnDie()
    {
        if(isOwned || isServer)
            ReloadScene();
    }
    [Command]
    private void ReloadScene()
    {
        PlayerEntity[] players = GameLevelController.GetAllLevelPlayers();
        foreach(PlayerEntity player in players) 
        {
            player.CurrentHealth = player.MaxHealth;
            player.controller.enabled = false;
            player.transform.position = new Vector3(Random.Range(-10.0f, 10.0f), 0.0f, Random.Range(-10.0f, 10.0f));
            player.controller.enabled = true;
            player.OnGameReload();
        }
    }
    public void OnGameReload()
    {
        
    }
}
