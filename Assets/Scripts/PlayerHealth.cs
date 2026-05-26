using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isDead = false; 

    [Header("UI References")]
    public Slider healthSlider;
    public GameObject gameOverScreen; 

    void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (gameOverScreen != null) gameOverScreen.SetActive(false);
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player Has Died!");

        PlayerMovement movementScript = GetComponent<PlayerMovement>();
        if (movementScript != null) movementScript.enabled = false;

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
    }
}