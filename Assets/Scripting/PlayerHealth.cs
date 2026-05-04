using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{   
    private float health;
    private float lerpTimer;
    [Header("Health Bar")]

    private float maxHealth = 100;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;
    public TextMeshProUGUI healthText;
    [Header("Health Bar")]

    public Image overlay;
    public float duration;
    public float fadeSpeed;

    private float durationTimer;

    void Start()
    {
        health = maxHealth;
              overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);

    }

    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
      
      if (overlay.color.a > 0)
      {
          durationTimer += Time.deltaTime;
          if (durationTimer > duration)
          {
              float tempAlpha = overlay.color.a;
              tempAlpha -= Time.deltaTime * fadeSpeed;
              overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
          }
      }
    }

    public void UpdateHealthUI()
    {
       Debug.Log(health); 
       float fillF = frontHealthBar.fillAmount;
       float fillB = backHealthBar.fillAmount;
       float hFraction = health / maxHealth;    
       if (fillB > hFraction)
       {
           frontHealthBar.fillAmount = hFraction;
           backHealthBar.color = Color.red;
           lerpTimer += Time.deltaTime;
           float percentComplete = lerpTimer / chipSpeed;
           percentComplete = percentComplete * percentComplete;
           backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
       }
       if (fillF < hFraction)
       {
           backHealthBar.fillAmount = hFraction;
           backHealthBar.color = Color.green;
           lerpTimer += Time.deltaTime;
           float percentComplete = lerpTimer / chipSpeed;
           percentComplete = percentComplete * percentComplete;
           frontHealthBar.fillAmount = Mathf.Lerp(fillF, hFraction, percentComplete);
       }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
        duration = 0;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);

    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }
}
