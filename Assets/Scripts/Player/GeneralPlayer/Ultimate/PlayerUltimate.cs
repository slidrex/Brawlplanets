using Mirror;
using UnityEngine;

public abstract class PlayerUltimate : NetworkBehaviour
{
    public float CurrentUltimateStatus { get; private set; }
    public PlayerEntity PlayerEntity { get; private set; }
    private UltimateJoystick joystick;
    [SerializeField] private UltimateRestoreModel[] _ultimateRestoreModels;
    protected virtual void Awake()
    {
        PlayerEntity = GetComponent<PlayerEntity>();
    }
    protected virtual void Start()
    {
        joystick = PlayerEntity.UIHolder.UltimateJoystick;
        joystick.OnUltimateRelease += OnUltimateRelease;
        EnableUltimateRestoring();
    }
    private void OnEnable()
    {
        foreach (var model in _ultimateRestoreModels)
        {
            model.OnUltimateRestored += OnUltimateRestored;
        }
    }
    private void OnUltimateRelease()
    {
        CurrentUltimateStatus = 0;
        PlayerEntity.UIHolder.UltimateJoystick.SetJoystickType(isReady: false);
        OnUltimateUse(joystick.GetActiveJoystickDirection());
        EnableUltimateRestoring();
    }
    public virtual void OnUltimateReady()
    {

    }
    public abstract void OnUltimateUse(Vector3 joystickDirection);
    private void OnDisable()
    {
        foreach (var model in _ultimateRestoreModels)
        {
            model.OnUltimateRestored -= OnUltimateRestored;
        }
    }
    private void EnableUltimateRestoring()
    {
        foreach (var model in _ultimateRestoreModels)
        {
            model.StartRestoring();
        }
    }
    private void DisableUltimateRestoring()
    {
        foreach (var model in _ultimateRestoreModels)
        {
            model.StopRestoring();
        }
    }
    private void OnUltimateRestored(float delta)
    {
        CurrentUltimateStatus = Mathf.Clamp(CurrentUltimateStatus + delta, 0, 100.0f);
        joystick.SetJoystickFillAmount(CurrentUltimateStatus, 100.0f);
        if (CurrentUltimateStatus >= 100.0f)
        {
            OnUltimateReady();
            PlayerEntity.UIHolder.UltimateJoystick.SetJoystickType(isReady: true);
            DisableUltimateRestoring();
        }
    }
}
