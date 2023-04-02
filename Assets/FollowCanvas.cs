using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class FollowCanvas : NetworkBehaviour
{
    [SyncVar] public Transform FollowObject;
    [SerializeField] private Transform ElementsHolder;
    [SerializeField] private Text chargeCount;
    public HealthBar Healthbar;
    [SerializeField] private Text nickname;
    private PlayerEntity followObjectPlayer;
    private bool _IsEnemy;
    public void SetNickname(string name) => nickname.text = name;
    [Command]
    public void OnCanvasCreated() => UpdatePlayersCanvases();
    [ClientRpc]
    private void UpdatePlayersCanvases()
    {
        PlayerEntity[] players = GameLevelController.GetAllLevelPlayers();
        foreach(PlayerEntity player in players)
        {
            if(player.isLocalPlayer) player.SetupLocalCanvas(false);
            else player.SetupLocalCanvas(true);
        }
    }
    private void Start()
    {
        followObjectPlayer = FollowObject.GetComponent<PlayerEntity>();
        CmdSetClientAuthority(gameObject, followObjectPlayer.connectionToClient);
        followObjectPlayer.FollowCanvas = this;
        OnCanvasCreated();
    }
    [Command]
    private void CmdSetClientAuthority(GameObject canvas, NetworkConnectionToClient connection)
    {
        canvas.GetComponent<FollowCanvas>().netIdentity.AssignClientAuthority(connection);
    }
    private void Update()
    {
        Vector3 offset = Vector3.up * 3.3f + Vector3.forward;
        if(FollowObject != null && transform.position != FollowObject.transform.position + offset)
            transform.position = FollowObject.transform.position + offset;
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
