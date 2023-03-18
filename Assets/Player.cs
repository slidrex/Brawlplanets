using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SerializeField] private CharacterController controller;
    private void Update()
    {
        if(isLocalPlayer == false) return;
        
        Vector3 moveVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
        controller.Move(moveVector * Time.deltaTime);
    }
}
