using UnityEngine;
using UnityEngine.UI;

public class UIHolder : MonoBehaviour
{
    public Text ChargeCounter;
    public FollowCanvas FollowCanvas { get; set; }
    [field:SerializeField] public PlayerCamera Camera { get; private set; }
    [field:SerializeField] public FixedJoystick MovementJoystick { get; private set; }
    [field:SerializeField] public FixedJoystick AttackJoystick { get; private set; }
}
