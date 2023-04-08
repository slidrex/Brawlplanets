using Mirror;
using Mirror.Examples.Tanks;
using UnityEngine;

public sealed class BrockAttack : PlayerChargeAttack
{
    [SerializeField] private Projectile rocket;
    protected override void OnAttack(Vector3 direction)
    {
        if (isOwned) CmdSpawnRocket(transform.position, Player.netIdentity.netId, direction);
    }
    [Command]
    private void CmdSpawnRocket(Vector3 spawnPosition, uint ownerNetId, Vector2 moveVector)
    {
        var proj = Instantiate(rocket, spawnPosition, Quaternion.identity);
        proj.Owner = ownerNetId;


        proj.MoveVector = new Vector3(moveVector.x, 0.0f, moveVector.y);
        NetworkServer.Spawn(proj.gameObject, NetworkServer.spawned[ownerNetId].connectionToClient);
    }
}
