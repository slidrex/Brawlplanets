using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class FollowCanvas : NetworkBehaviour
{
    [SyncVar] public Transform FollowObject;
    [SerializeField] private Transform ElementsHolder;
    [SerializeField] private Text chargeCount;
    public Image healthBar;
    public Text healthText;
    [SerializeField] private Text nickname;
    private System.Collections.Generic.List<RectTransform> elements = new System.Collections.Generic.List<RectTransform>();
    private bool _IsEnemy;
    public void SetNickname(string name) => nickname.text = name;
    private void Start()
    {
        FollowObject.GetComponent<PlayerEntity>().FollowCanvas = this;
    }
    private void Update()
    {
        Vector3 offset = Vector3.up * 1.6f + Vector3.forward;
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
        if(_IsEnemy)
        {
            healthBar.color = Color.red;
            nickname.color = Color.red;
        }
        else
        {
            healthBar.color = new Color(0.0f, 0.6f, 1.0f);
            nickname.color = new Color(0.0f, 0.6f, 1.0f);
        }
    }
    [Command]
    public void CmdUpdateHealthbar(GameObject player, int currentHealth, int maxHealth) => RpcUpdateHealthBar(player, currentHealth, maxHealth);
    [ClientRpc]
    private void RpcUpdateHealthBar(GameObject player, int currentHealth, int maxHealth)
    {
        FollowCanvas canvas = player.GetComponent<PlayerEntity>().FollowCanvas.GetComponent<FollowCanvas>();
        canvas.healthBar.fillAmount = currentHealth/(float)maxHealth;
        canvas.healthText.text = currentHealth.ToString();
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
