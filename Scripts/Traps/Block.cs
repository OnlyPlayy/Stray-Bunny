using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour
{
	// Object variables.
	[Header("Objects")]
	[SerializeField] private AudioClip breakAudioClip;

	[Header("Settings")]
	[SerializeField] [Range(1, 100)] private float shakeSpeed = 50;
	[SerializeField] [Range(1, 10)] private float destroyTime = 1;

	private Vector2 startingPosition;
	private bool playerCollided;
	private GameObject childSprite;

	void Start()
	{
		// Get the first child object.
		childSprite = transform.GetChild(0).gameObject;
		// Get the current position of the object.
		startingPosition = transform.position;
	}

	void Update()
	{
		// Start randomly shaking the child object at a set rate when player collides wit the box collider.
		if (playerCollided)
		{
			childSprite.transform.position = new Vector2(Mathf.Sin(Time.time * shakeSpeed) * 0.1f + startingPosition.x, startingPosition.y);
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		// If the collided object was a player, enable the child object animator to start the breaking animation.
		if (collision.gameObject.tag == "Player" && !playerCollided)
		{
			childSprite.GetComponent<Animator>().enabled = true;
			playerCollided = true;

			// Adjust the animation speed to match the destroy time.
			float speedDifference = (childSprite.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length / destroyTime) * 100;
			childSprite.GetComponent<Animator>().speed = speedDifference / 100;

			StartCoroutine(Destroy());
		}
	}

	IEnumerator Destroy()
	{
		// Wait for the length of the destroy time before continuing.
		yield return new WaitForSeconds(destroyTime);
		// Play the breaking sound as one shot to avoid the sound being overridden.
		AudioManager.scriptInstance.breakSource.PlayOneShot(breakAudioClip);
		// Disable the sprite and collision of the object and enable the particle system.
		transform.GetChild(0).gameObject.SetActive(false);
		transform.GetChild(1).gameObject.SetActive(true);
		transform.GetComponent<Collider2D>().enabled = false;
		// Destroy the object after 3 seconds to save on performance.
		Destroy(gameObject, 3);
	}
}
