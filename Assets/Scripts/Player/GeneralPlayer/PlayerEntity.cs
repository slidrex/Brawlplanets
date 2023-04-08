using UnityEngine;
using Mirror;

public class PlayerEntity : NetworkBehaviour
{
    [SerializeField] private UIHolder holder;
    public UIHolder UIHolder { get; private set; }
    [SerializeField] private FollowCanvas followCanvas;
    public FollowCanvas FollowCanvas;
    [SyncVar] public string Nickname;
    [field:SerializeField, SyncVar] public int MaxHealth { get; private set; }
    [SyncVar(hook = nameof(ChangeHealthCallback))] private int _currentHealth;
    public int CurrentHealth { get => _currentHealth; }
    public override void OnStartLocalPlayer()
    {
        SetupLocalPlayer();
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
    private void ChangeHealthCallback(int oldHealth, int newHealth)
    {
        if(_currentHealth <= 0 && isOwned) ReloadScene();
        FollowCanvas.RpcUpdateHealthBar(gameObject, CurrentHealth, MaxHealth);
    }
    public void Damage(int damage)
    {
        _currentHealth -= damage;
    }
    [Command]
    private void CmdSetHealth(uint netId, int health)
    {
        PlayerEntity player = NetworkServer.spawned[netId].GetComponent<PlayerEntity>();
        player._currentHealth = health;
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
            player._currentHealth = player.MaxHealth;
            player.transform.position = new Vector3(Random.Range(-10.0f, 10.0f), 0.0f, Random.Range(-10.0f, 10.0f));
            
            player.OnGameReload();
        }
    }
    public void OnGameReload()
    {
        
    }
}
