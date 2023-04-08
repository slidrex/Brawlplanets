using Mirror;
using UnityEngine;

public sealed class CharacterConstructor : MonoBehaviour
{
    [SerializeField] private PlayerEntity Player;
    [SerializeField] private PlayerMovementController _movementController;
    [SerializeField] private PlayerUltimate _ultimate;
    [SerializeField] private PlayerChargeAttack _attack;
    [SerializeField] private NetworkTransform networkTransform;
    [SerializeField] private NetworkAnimator networkAnimator;
}
