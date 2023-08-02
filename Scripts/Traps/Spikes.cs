using UnityEngine;

public class Spikes : MonoBehaviour
{
	// Object variables.
	[Header("Settings")]
	[SerializeField] [Range(1, 60)] private float waitingTime;
	[SerializeField] [Range(1, 60)] private float resetTime;

	private float delayTimer;
	private BoxCollider2D boxCollider2D;
	private Animator spikesAnimator;
	private bool spikesReady;

	void Start()
	{
		// Extract box collider and animator components from the spikes object.
		boxCollider2D = transform.GetComponent<BoxCollider2D>();
		spikesAnimator = GetComponent<Animator>();

		// If the waiting and reset times are not equal to 0, disable the box collider component.
		if (waitingTime != 0 && resetTime != 0)
		{
			boxCollider2D.enabled = false;
		}
	}

	void Update()
	{
		// If the waiting and reset times are not equal to 0, allow the function to run.
		if (waitingTime != 0 && resetTime != 0)
		{
			if (spikesReady)
			{
				// Use modulo operator to get accurate time by finding the reminder when divided.
				delayTimer += Time.deltaTime % 60;

				// If the timer runs out, reset the spikes by disabling the box collider and playing the animation.
				if (waitingTime <= delayTimer)
				{
					delayTimer = 0;
					spikesReady = !spikesReady;
					boxCollider2D.enabled = spikesReady;
					spikesAnimator.Play("Reset");
				}
			}
			else
			{
				// Use modulo operator to get accurate time by finding the reminder when divided.
				delayTimer += Time.deltaTime % 60;

				// If the timer runs out, trigger the spikes by enabling the box collider and playing the animation.
				if (resetTime <= delayTimer)
				{
					delayTimer = 0;
					spikesReady = !spikesReady;
					boxCollider2D.enabled = spikesReady;
					spikesAnimator.Play("Trigger");
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		// If the collided object was a player, call die function in player object.
		if (collision.tag == "Player")
		{
			collision.GetComponent<Character>().Die();
		}
	}
}
