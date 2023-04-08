using UnityEngine;

public sealed class UltimateRestoreOverTime : UltimateRestoreModel
{
    [SerializeField] private float _ultimateRestoreTime;
    [SerializeField] private float _restoreAmountPerCycle;
    private float _timeSinceRestored;
    protected override void OnStartRestoring()
    {
        _timeSinceRestored = 0.0f;
    }
    private void Update()
    {
        if (IsRestoring)
        {
            if(_timeSinceRestored < _ultimateRestoreTime)
            {
                _timeSinceRestored += Time.deltaTime;
            }
            else
            {
                _timeSinceRestored = 0.0f;
                AddUltimate(_restoreAmountPerCycle);
            }
        }
    }
    protected override void OnEndRestoring()
    {
        
    }

}
