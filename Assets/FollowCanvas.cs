using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class FollowCanvas : NetworkBehaviour
{
    [SyncVar] public Transform FollowObject;
    [SerializeField] private Transform ElementsHolder;
    [SerializeField] public Image healthBar;
    [SerializeField] private Text nickname;
    private System.Collections.Generic.List<RectTransform> elements = new System.Collections.Generic.List<RectTransform>();
    private bool _IsEnemy;
    public void SetNickname(string name)
    {
        nickname.text = name;
        int healthbar;
    }
    private void Start()
    {
        FollowObject.GetComponent<PlayerEntity>().FollowCanvas = this;
    }
    private void Update()
    {
        Vector3 offset = Vector3.up * 1.6f + Vector3.forward;
        if( FollowObject != null && transform.position != FollowObject.transform.position + offset)
            transform.position = FollowObject.transform.position + offset;
    }
    public void SetupCanvas(bool isEnemy)
    {
        _IsEnemy = isEnemy;
        if(_IsEnemy)
        {
            healthBar.color = Color.red;
            nickname.color = Color.red;
        }
        else
        {
            healthBar.color = Color.green;
            nickname.color = Color.green;
        }
    }
    [Command]
    public void CmdUpdateHealthbar(GameObject player, int currentHealth, int maxHealth) => RpcUpdateHealthBar(player, currentHealth, maxHealth);
    [ClientRpc]
    private void RpcUpdateHealthBar(GameObject player, int currentHealth, int maxHealth)
    {
        player.GetComponent<PlayerEntity>().FollowCanvas.healthBar.fillAmount = currentHealth/(float)maxHealth;
    }
    public void CommitRemovedElements()
    {
        for(int i = 0; i < elements.Count; i++) if(elements[i] == null) elements.RemoveAt(i);
    }
    public RectTransform PushElement(RectTransform element, bool shouldSpawnOntop)
    {
        element.SetParent(ElementsHolder);
        if(shouldSpawnOntop) element.SetSiblingIndex(0);
        elements.Add(element);
        return element;
    }
}
