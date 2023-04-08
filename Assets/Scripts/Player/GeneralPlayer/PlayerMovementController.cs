using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController), typeof(BoxCollider), typeof(Rigidbody))]
public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    private CharacterController controller;
    [SerializeField] private PlayerEntity entity;
    public float MovementSpeed;
    protected Vector3 MovementVector;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    protected void Update()
    {
        if(!isLocalPlayer) return;
        
        MovementVector = PlayerConfig.InputWay == PlayerConfig.InputMode.PC ? new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")) : new Vector3(entity.UIHolder.MovementJoystick.Horizontal, 0.0f, entity.UIHolder.MovementJoystick.Vertical);
        if(MovementVector != Vector3.zero)
        {
            Vector2.ClampMagnitude(MovementVector, 0.5f);
            controller.Move(MovementVector * MovementSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-MovementVector), Time.deltaTime * 10);
        }
        animator.SetInteger("MoveX", (int)MovementVector.x);
        animator.SetInteger("MoveZ", (int)MovementVector.z);
    }
}
