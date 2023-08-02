using UnityEngine;
using System.Threading.Tasks;

public class CannonBullet : MonoBehaviour
{
    // Object variables.
    [Header("Objects")]
    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private GameObject particleEffect;

    [Header("Settings")]
    [SerializeField] private float bulletSpeed = 10;

    private float bulletWidth;

    void Start()
    {
        // Set the velocity and get the size of the bullet.
        rigidBody2D.velocity = transform.right * bulletSpeed * -1;
        bulletWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // If the collided object was a player or ground, destroy the bullet object.
        if (hitInfo.tag == "Ground" || hitInfo.tag == "Player")
        {
            SelfDestroy();
        }

        // If the collided object was a player, call die function in player object.
        if (hitInfo.tag == "Player")
		{
            hitInfo.GetComponent<Character>().Die();
		}
    }
    
    async void SelfDestroy()
    {
        // Create a particle system object at the position of the bullet collision point and destroy the bullet object.
        Vector3 particlePosition = new Vector3 (transform.position.x - (bulletWidth / 2), transform.position.y, 0);
        GameObject particleSystem = Instantiate(particleEffect, particlePosition, Quaternion.identity);
        particleSystem.name = "Particles";
        Destroy(gameObject);

        // Destroy the particle system object after 1 second.
        await Task.Delay(1000);
        DestroyImmediate(particleSystem);
    }
}