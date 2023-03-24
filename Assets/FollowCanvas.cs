using UnityEngine;
using Mirror;

public class FollowCanvas : MonoBehaviour 
{
    public Transform FollowTarget;
    private void Update()
    {
        transform.position = FollowTarget.position + new Vector3(0.0f, 7.0f, -7.0f);
    }
}
