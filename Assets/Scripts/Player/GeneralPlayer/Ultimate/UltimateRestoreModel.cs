using UnityEngine;


public abstract class UltimateRestoreModel : MonoBehaviour
{
    public System.Action<float> OnUltimateRestored;
    protected bool IsRestoring { get; private set; }
    public void StartRestoring()
    {
        IsRestoring = true;
        OnStartRestoring();
    }
    public void StopRestoring()
    {
        IsRestoring = false;
        OnEndRestoring();
    }
    protected abstract void OnStartRestoring();
    protected abstract void OnEndRestoring();
    protected void AddUltimate(float value)
    {
        if (!IsRestoring) return;

        if(value > 0)
        {
            OnUltimateRestored.Invoke(value);
        }
    }
}
