using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxHealth(float maxHP)
    {
        slider.maxValue = maxHP;
        slider.value = maxHP;
    }

    public void SetHealth(float currentHP)
    {
        slider.value = currentHP;
    }
     public void SetByXDirection(float value)
    {
        Vector3 scale = slider.transform.localScale;
        if (value < 0)
        {
            scale.x = -Mathf.Abs(scale.x); 
        }
        else
        {
            scale.x = Mathf.Abs(scale.x); 
        }

        slider.transform.localScale = scale;
    }
}
