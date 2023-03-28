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
    private const float projectileInterval = 0.6f;
    [SerializeField] private int maxChargeCount;
    [SerializeField] private float chargeRestoreTime;
    private float timeSinceChargeRestored;
    private int currentChargeCount;
    private float timeSinceProjectile;
    [SerializeField] private int maxHealth;
    [SyncVar] public string Nickname;
    [field:SerializeField, SyncVar] public int CurrentHealth { get; private set; }
    private string[] nicknames = new string[]
    {
        "Fox5",
        "Mishka",
        "robot236",
        "ubiyca_porazheniy",
        "killer_laro",
        "saga_o_lirexe",
        "wolk",
        "limon_mikrofon"
    };
    public override void OnStartLocalPlayer()
    {
        SetCurrentHealthCmd(gameObject, maxHealth);
        CmdSetNickname(gameObject, nicknames[Random.Range(0, nicknames.Length)]);
        
        
        UIHolder = Instantiate(holder, Vector3.zero, Quaternion.Euler(20.0f, 0.0f, 0.0f));
        UIHolder.Camera.FollowObject = transform;
        
        CmdSpawnCanvas(transform.position, gameObject);
        UIHolder.FollowCanvas = FollowCanvas;
        
    }
    protected virtual void Start()
    {
        print("global start");
        int delayedAction;
        StartCoroutine(DelayedAction());
    }
    private System.Collections.IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(0.2f);
        if(!isLocalPlayer)
        {
            FollowCanvas.SetupCanvas(true);
        }
        else FollowCanvas.SetupCanvas(false);
        FollowCanvas.SetNickname(Nickname);
    }
    [Command]
    private void SetCurrentHealthCmd(GameObject player, int maxHealth)
    {
        player.GetComponent<PlayerEntity>().CurrentHealth = maxHealth;
    }
    [Command]
    private void CmdSetNickname(GameObject player, string nickname)
    {
        player.GetComponent<PlayerEntity>().Nickname = nickname;
    }
    [Command]
    private void CmdSpawnCanvas(Vector3 position, GameObject playerObject) 
    {
        FollowCanvas obj = Instantiate(followCanvas.gameObject).GetComponent<FollowCanvas>();
        PlayerEntity player = playerObject.GetComponent<PlayerEntity>();
        
        obj.FollowObject = playerObject.transform;
        
        NetworkServer.Spawn(obj.gameObject, gameObject);
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
    public void Damage(int damage)
    {
        if(CurrentHealth - damage <= 0) OnDie();
        else CurrentHealth -= damage;
        
        FollowCanvas.CmdUpdateHealthbar(gameObject, CurrentHealth, maxHealth);
    }
    [Command]
    public void ReloadScene()
    {
        PlayerEntity[] players = FindObjectsOfType<PlayerEntity>();
        foreach(PlayerEntity player in players)
        {
            player.CurrentHealth = player.maxHealth;
            RpcUpdateCanvas(player.gameObject);
        }
    }
    private void RpcUpdateCanvas(GameObject player)
    {
        player.GetComponent<PlayerEntity>().FollowCanvas.CmdUpdateHealthbar(player.GetComponent<PlayerEntity>().gameObject, CurrentHealth, maxHealth);
    }
    public virtual void OnDie()
    {
        controller.enabled = false;
        transform.position = new Vector3(Random.Range(-3.0f, 3.0f), 0.0f, Random.Range(-3.0f, 3.0f));
        controller.enabled = true;
        ReloadScene();
    }
}
