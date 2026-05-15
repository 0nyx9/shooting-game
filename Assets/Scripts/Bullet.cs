using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 3f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = -transform.right * speed;
        Destroy(gameObject, lifeTime);
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     Destroy(gameObject);
    // }

    void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Zombie"))
    {
        ZombieLogic zombie = collision.gameObject.GetComponent<ZombieLogic>();
        if (zombie != null)
        {
            zombie.TakeDamage(25f); 
        }
    }
    Destroy(gameObject); 
}
}
