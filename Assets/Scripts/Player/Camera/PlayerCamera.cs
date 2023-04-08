using UnityEngine;
using Mirror;
using Cinemachine;

public class PlayerCamera : NetworkBehaviour
{
    public Camera RenderCamera;
    [HideInInspector] public Transform FollowObject;
    protected Transform Transform;
    [SerializeField] private CinemachineVirtualCamera vc;
    private void Start()
    {
        Transform = transform;
        vc.Follow = FollowObject;
        vc.m_Follow = FollowObject;
    }
}
