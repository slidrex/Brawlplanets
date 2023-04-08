using Mirror;
using System;
using UnityEngine;

public abstract class PlayerChargeAttack : NetworkBehaviour
{
    protected PlayerEntity Player { get; private set; }
    [SerializeField, SyncVar] private float _chargeRestoreTime;
    [field: SerializeField] public ushort MaxChargeCount { get; private set; }
    private float _timeSinceChargeRestored;
    [SerializeField] private float _attackInterval;
    private float _timeSinceAttacked;
    [SyncVar] private ushort _currentChargeCount;
    [SerializeField] private KeyCode _attackKey;
    public uint GetCurrentChargeCount() => _currentChargeCount;
    private void Start()
    {
        Player = GetComponent<PlayerEntity>();
    }
    public void SetCurrentChargeCount(ushort value)
    {
        int old = _currentChargeCount;
        _currentChargeCount = value;
        OnChangedChargeCount(_currentChargeCount - old);
    }
    public void AddCharge(int value)
    {
        SetCurrentChargeCount((ushort)(_currentChargeCount + value));
    }
    protected virtual void Update()
    {
        if (!isLocalPlayer) return;

        Vector2 attackDirection = PlayerConfig.InputWay == PlayerConfig.InputMode.PC ? GetPointAttackVector() : Player.UIHolder.AttackJoystick.Horizontal * Vector2.right + Player.UIHolder.AttackJoystick.Vertical * Vector2.up;

        bool entryCondition = PlayerConfig.InputWay == PlayerConfig.InputMode.PC ? Input.GetKey(KeyCode.Mouse0) : true;

        if (entryCondition && attackDirection != Vector2.zero && _currentChargeCount > 0 && _attackInterval <= _timeSinceAttacked)
        {
            attackDirection.Normalize();
            Vector3 direction = (Input.mousePosition - transform.position).normalized;
            AddCharge(-1);
            _timeSinceAttacked = 0.0f;
            Player.FollowCanvas.SetChargeCount(_currentChargeCount);
            OnAttack(direction);
        }
        if(_timeSinceAttacked < _attackInterval)
        {
            _timeSinceAttacked += Time.deltaTime;
        }
    }
    private Vector2 GetPointAttackVector()
    {
        Ray ray = Player.UIHolder.Camera.RenderCamera.ScreenPointToRay(Input.mousePosition);

        Vector3 rayOriginPosition = ray.origin;
        Vector3 rayEndPosition = rayOriginPosition;

        Vector3 delta = ray.direction;
        float dCount = (transform.position.y - rayEndPosition.y) / delta.y;
        dCount = Mathf.Abs(dCount);
        rayEndPosition += delta * dCount;

        Vector3 distance = rayEndPosition - transform.position;

        distance.y = distance.z;
        return distance;
    }
    protected virtual void FixedUpdate()
    {
        if (_currentChargeCount < MaxChargeCount)
        {
            RestoreCharges();
        }
    }
    private void RestoreCharges()
    {
        if (_timeSinceChargeRestored < _chargeRestoreTime) _timeSinceChargeRestored += Time.fixedDeltaTime;
        else
        {
            AddCharge(1);
            Player.UIHolder.FollowCanvas.SetChargeCount(_currentChargeCount);
        }
    }
    protected virtual void OnChangedChargeCount(int delta) { }
    protected abstract void OnAttack(Vector3 direction);
}
