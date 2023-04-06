using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class FollowCanvas : NetworkBehaviour
{
    [SerializeField] private Transform ElementsHolder;
    [SerializeField] private Text chargeCount;
    public HealthBar Healthbar;
    [SerializeField] private Text nickname;
    [SyncVar] public Transform FollowTransform; 
    private PlayerEntity followPlayer;
    private bool _IsEnemy;
    public void SetNickname(string name) => nickname.text = name;
    private void Start()
    {
        followPlayer = FollowTransform.GetComponent<PlayerEntity>();
        followPlayer.FollowCanvas = this;
        if(isOwned) OnCanvasCreated();
        /*if(followPlayer.netIdentity.isOwned) SetupCanvas(isEnemy: false);
        else SetupCanvas(isEnemy: true);*/
    }
    [Command]
    public void OnCanvasCreated() 
    {
        UpdatePlayersCanvases();
    }
    [ClientRpc]
    private void UpdatePlayersCanvases()
    {
        PlayerEntity[] players = GameLevelController.GetAllLevelPlayers();
        foreach(PlayerEntity player in players)
        {
            if(player.isOwned) player.SetupLocalCanvas(false);
            else player.SetupLocalCanvas(true);
            //RpcUpdateHealthBar(player.gameObject, player.CurrentHealth, player.MaxHealth);
        }
    }
    private void Update()
    {
        Vector3 offset = Vector3.up * 3.3f + Vector3.forward;
        if(followPlayer != null && transform.position != followPlayer.transform.position + offset)
            transform.position = followPlayer.transform.position + offset;
    }
    public void ShowChargeCount()
    {
        chargeCount.gameObject.SetActive(true);
    }
    public void SetChargeCount(int count)
    {
        chargeCount.text = count.ToString();
    }
    public void SetupCanvas(bool isEnemy)
    {
        _IsEnemy = isEnemy;
        Healthbar.SetHealthBarColor(isEnemy);
        nickname.color = isEnemy? Color.red : new Color(0.0f, 0.6f, 1.0f);
        //SetNickname(followPlayer.Nickname);
        if(isEnemy == false)
        {
            //ShowChargeCount();
            //followPlayer.UIHolder.FollowCanvas = this;
        }
    }
    [ClientRpc]
    public void RpcUpdateHealthBar(GameObject player, int currentHealth, int maxHealth)
    {
        FollowCanvas canvas = player.GetComponent<PlayerEntity>().FollowCanvas;
        
        canvas.Healthbar.SetHealth(currentHealth, maxHealth);
    }
    public RectTransform PushElement(RectTransform element, int placeOrder)
    {
        element.SetParent(ElementsHolder);
        element.SetSiblingIndex(placeOrder);
        return element;
    }
}
