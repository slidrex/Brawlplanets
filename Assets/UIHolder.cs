using UnityEngine;
using UnityEngine.UI;

public class UIHolder : MonoBehaviour
{
    public FollowCanvas FollowCanvas { get; set; }
    [field:SerializeField] public PlayerCamera Camera { get; private set; }
    [field:SerializeField] public FixedJoystick MovementJoystick { get; private set; }
    [field:SerializeField] public FixedJoystick AttackJoystick { get; private set; }
    [field:SerializeField] public UltimateJoystick UltimateJoystick { get; private set; }
}