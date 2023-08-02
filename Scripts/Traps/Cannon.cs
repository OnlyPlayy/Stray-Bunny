using UnityEngine;
using System.Threading.Tasks;

public class Cannon : MonoBehaviour
{
    // Object variables.
    [Header("Objects")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject particleEffect;

    [Header("Settings")]
    [SerializeField] private float shootingRate = 1;
    private Animator cannonAnimation;
    
    private float shootCooldown;

    void Start() 
    {
        cannonAnimation = gameObject.GetComponent<Animator>();
        shootCooldown = 0;
    }

    void Update()
    {
        Shoot();

        // Keep counting down as long as shoot cooldown is above 0.
        if (shootCooldown > 0)
        {
			shootCooldown -= Time.deltaTime;
		}
    }

    async void Shoot()
    {
        if (shootCooldown <= 0)
        {
            // Make animation, and revert to neutral state
            cannonAnimation.Play("Shoot", -1, 0);

            // Create a bullet prefab on fire point and create a particle system object whenever the cannon shoots.
            GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bulletObject.name = "Bullet";
            GameObject particleSystem = Instantiate(particleEffect, transform.position, Quaternion.identity);
            particleSystem.name = "Particles";

            // Reset the cooldown timer.
            shootCooldown = shootingRate;

            // Destroy the particle system object after 1 second.
            await Task.Delay(1000);
            DestroyImmediate(particleSystem);
        }
    }
}
