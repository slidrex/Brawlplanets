using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Text healthText;
    [SerializeField] private Image healthValue;
    public void SetHealth(int currentHealth, int maxHealth)
    {
        healthText.text = currentHealth.ToString();
        healthValue.fillAmount = currentHealth/(float)maxHealth;
    }
    public void SetHealthBarColor(bool isEnemy)
    {
        healthValue.color = isEnemy? Color.red : new Color(0.0f, 0.6f, 1.0f);
    }
}
