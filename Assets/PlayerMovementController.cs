using UnityEngine;
using Mirror;

public class PlayerMovementController : NetworkBehaviour
{
    public enum InputMode 
    {
        PC,
        Mobile
    }
    public InputMode InputWay;
    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerEntity entity;
    public float MovementSpeed;
    protected Vector3 MovementVector;
    protected void Update()
    {
        if(!isLocalPlayer) return;
        
        MovementVector = InputWay == InputMode.PC ? new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")) : new Vector3(entity.UIHolder.MovementJoystick.Horizontal, 0.0f, entity.UIHolder.MovementJoystick.Vertical);
        if(MovementVector != Vector3.zero)
            controller.Move(MovementVector * MovementSpeed * Time.deltaTime);
    }
    protected void FixedUpdate()
    {
    }
}
