using UnityEngine;

public class ZombieLogic : MonoBehaviour
{
    public float health = 100f;
    public Animator anim;
    public float moveSpeed = 2f;
    
    private Transform player;
    private bool isDead = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return; // Stop everything if dead

        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance > 1.5f) // If far away, walk toward player
            {
                anim.SetBool("isWalking", true);
                transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                anim.SetBool("isWalking", false);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;
        anim.SetTrigger("Hit"); // Play the hit animation

        if (health <= 0)
        {
            StartCoroutine(HandleDeath());
        }
    }

    System.Collections.IEnumerator HandleDeath()
    {
        isDead = true;
        anim.SetTrigger("Die"); // Play death animation
        
        // Disable the collider so the player doesn't trip over the corpse
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(5f); // Wait 5 seconds before disappearing
        Destroy(gameObject);
    }
}