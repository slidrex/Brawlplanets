using Mirror;
using UnityEngine;

public sealed class BrockUltimateController : PlayerUltimate
{
    public override void OnUltimateUse(Vector3 joystickDirection)
    {
        if (PlayerEntity.isOwned) CmdRestoreCharges(PlayerEntity.gameObject);
    }
    [Command]
    private void CmdRestoreCharges(GameObject player)
    {
        var attack = player.GetComponent<PlayerChargeAttack>();
        attack.SetCurrentChargeCount(attack.MaxChargeCount);
    }
}
