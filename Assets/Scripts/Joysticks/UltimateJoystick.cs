using UnityEngine;
using UnityEngine.UI;

public class UltimateJoystick : MonoBehaviour
{
    [SerializeField] private FixedJoystick ultimateJoystick;
    [SerializeField] private GameObject activeJoystick;
    [SerializeField] private GameObject inactiveJoystick;
    [SerializeField] private Image ultimatefillAmount;
    public System.Action OnUltimateRelease;
    private void OnUltimateJoystickRelease() => OnUltimateRelease.Invoke();
    private void Start()
    {
        ultimateJoystick.OnJoystickUp += OnUltimateJoystickRelease;
    }
    public void SetJoystickType(bool isReady)
    {
        activeJoystick.SetActive(isReady);
        inactiveJoystick.SetActive(!isReady);
        if(isReady) OnActiveJoystickActivated();
    }
    private void OnActiveJoystickActivated()
    {
        
    }
    public void SetJoystickFillAmount(float current, float max) => ultimatefillAmount.fillAmount = current/max;
    public Vector2 GetActiveJoystickDirection() => ultimateJoystick.Direction;
}