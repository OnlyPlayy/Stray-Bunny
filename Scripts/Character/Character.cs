using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading.Tasks;

public class Character : MonoBehaviour
{
	// Object variables.
	[Header("Objects")]
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Transform colliderTransform;
	[SerializeField] private AudioClip deathAudioClip;
	[SerializeField] private AudioClip walkAudioClip;
	[SerializeField] private AudioClip singleJumpAudioClip;
	[SerializeField] private AudioClip doubleJumpAudioClip;

	[Header("Settings")]
	[Range(0, 10)] public int additionalJumps = 1;
	[SerializeField] [Range(1, 10)] private float movementSpeed = 5;
	[SerializeField] [Range(1, 30)] private float additionalJumpHeight = 15;
	[SerializeField] [Range(1, 30)] private float fallVelocity = 8;
	[SerializeField] [Range(1, 30)] private float jumpVelocity = 5;

	[HideInInspector] public int jumpCount;
	[HideInInspector] public bool canJump;
	[HideInInspector] public float idleTimer;
	private float transferSpeed;
	private bool isMoving, facingRight = true, playerSlowed = false;
	private SpriteRenderer furSprite;
	private Rigidbody2D rigidBody2D;
	private Animator characterAnimator;
	private string currentState;
	private float characterSpeed, landingDelay, castDistance;
	private float maxMovementSpeed;

	void Awake()
	{
		maxMovementSpeed = movementSpeed;
	}

	void Start()
	{
		// Move the player object to the location of the start object with height offset.
		Vector2 startPosition = GameObject.Find("Start").transform.position;
		transform.position = new Vector2(startPosition.x, startPosition.y + 1.5f);

		// Extract rigid body and animator components from the character object.
		rigidBody2D = GetComponent<Rigidbody2D>();
		characterAnimator = GetComponent<Animator>();
		// Set the cast distance for collision detection.
		castDistance = 0.725f;
		
		// Extract the sprite renderer component from the character object and set color.
		furSprite = GetComponent<SpriteRenderer>();
        furSprite.color = new Color(1, 1, 1, 1);
	}

	void Update()
	{
		landingDelay -= Time.deltaTime;
		Vector2 raycastDistance = colliderTransform.position;
		raycastDistance.x += castDistance;

		// Change animations based current action.
		if (isMoving && currentState != "JumpingOut" && currentState != "JumpingIn")
		{
			// Change the animation to walking when the player is not falling, jumping and hitting an object belonging to the ground layer.
			characterSpeed = transferSpeed;
			RaycastHit2D raycastHit = Physics2D.Linecast(colliderTransform.position, raycastDistance, 1 << LayerMask.NameToLayer("Ground"));

			if (raycastHit.collider == null && currentState != "Falling" && currentState != "Jumping" && canJump)
			{
				idleTimer = 0;

				if (landingDelay <= 0)
				{
					ChangeAnimationState("Walking");
				}
			}
			else if (raycastHit.collider != null && currentState != "Falling" && currentState != "Jumping" && currentState != "Idle" && currentState != "Sit" && currentState != "Landing" && currentState != "Additional")
			{
				ChangeAnimationState("Idle");
			}
		}
		else if (currentState != "Falling" && currentState != "Jumping" && currentState != "Idle" && currentState != "Sit" && currentState != "Landing" && currentState != "Additional" && currentState != "JumpingOut" && currentState != "JumpingIn")
		{
			ChangeAnimationState("Idle");
		}

		Flip();
		Ground();
		Idle();
	}

	void FixedUpdate()
	{
		// Set the character movement speed.
		rigidBody2D.velocity = new Vector2(characterSpeed, rigidBody2D.velocity.y);

		if (rigidBody2D.velocity.y < 0 && !canJump && currentState != "JumpingOut" && currentState != "JumpingIn")
		{
			// If character velocity is below 0, change animation to falling.
			idleTimer = 0;
			rigidBody2D.velocity += Vector2.up * Physics2D.gravity.y * (fallVelocity - 1) * Time.deltaTime;
			ChangeAnimationState("Falling");
		}
		else if (rigidBody2D.velocity.y > 0 && !canJump && currentState != "JumpingOut" && currentState != "JumpingIn")
		{
			// If character animation is above 0, change animation to jumping.
			idleTimer = 0;
			rigidBody2D.velocity += Vector2.up * Physics2D.gravity.y * (jumpVelocity - 1) * Time.deltaTime;
		}
	}

	public void Left()
	{
		// Move the character at a set speed and raycast in set direction.
		transferSpeed = -movementSpeed;
		isMoving = true;
		castDistance = -0.725f;
	}

	public void Right()
	{
		// Move the character at a set speed and raycast in set direction.
		transferSpeed = movementSpeed;
		isMoving = true;
		castDistance = 0.725f;
	}

	void Walk()
	{
		// Play the walk sound as one shot to avoid the sound being overridden on animation event trigger.
		AudioManager.scriptInstance.walkSource.PlayOneShot(walkAudioClip);
	}

	void SingleJump()
	{
		// Play the single jump sound as one shot to avoid the sound being overridden on animation event trigger.
		AudioManager.scriptInstance.singleJumpSource.PlayOneShot(singleJumpAudioClip);
	}

	void DoubleJump()
	{
		// Play the double jump sound as one shot to avoid the sound being overridden on animation event trigger.
		AudioManager.scriptInstance.doubleJumpSource.PlayOneShot(doubleJumpAudioClip);
	}

	void Death()
	{
		// Play the death sound as one shot to avoid the sound being overridden on animation event trigger.
		AudioManager.scriptInstance.deathSource.PlayOneShot(deathAudioClip);
	}

	public void Up(float jumpHeight)
	{
		if (currentState != "Death" && currentState != "JumpingIn")
		{
			// If the player has jumped, allow to jump as many times as the value has been set to.
			// Save the values to player prefs for statistics.
			if (canJump)
			{
				rigidBody2D.velocity = Vector2.up * jumpHeight;
				ChangeAnimationState("Jumping");
				PlayerPrefs.SetInt("Jumps", PlayerPrefs.GetInt("Jumps") + 1);
			}
			else if (!canJump && additionalJumps != jumpCount)
			{
				jumpHeight = additionalJumpHeight;
				rigidBody2D.velocity = Vector2.up * jumpHeight;
				jumpCount++;
				ChangeAnimationState("Additional");
				PlayerPrefs.SetInt("Additional", PlayerPrefs.GetInt("Additional") + 1);
			}
		}
	}

	public void Stop()
	{
		// Make the character stop moving.
		characterSpeed = 0;
		isMoving = false;
	}

	void Flip()
	{
		// Flip the character sprite and charging bar in the opposite direction when walking direction changes.
		if (characterSpeed > 0 && !facingRight || characterSpeed < 0 && facingRight)
		{
			facingRight = !facingRight;
			Vector3 facingDirection = transform.localScale;
			facingDirection.x *= -1;
			transform.localScale = facingDirection;
			transform.GetChild(1).localScale = facingDirection;
		}
	}

	void Ground()
	{
		// Check if the character is touching an object with its feet which belongs to the ground layer.
		canJump = false;
		Collider2D[] collidingObjects = Physics2D.OverlapCircleAll(colliderTransform.position, 0.4f, groundLayer);

		if (collidingObjects.Length > 0)
		{
			// Reset the jump counter and allow to jump.
			canJump = true;
			jumpCount = 0;
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		// Get overlapping objects.
		Collider2D[] collidingObjects = Physics2D.OverlapCircleAll(colliderTransform.position, 0.4f, groundLayer);

		if (collidingObjects.Length > 0 && currentState == "Falling" || currentState == "Jumping")
		{
			// Play landing animation if the character was falling or jumping.
			ChangeAnimationState("Landing");
			landingDelay = characterAnimator.GetCurrentAnimatorStateInfo(0).length;
		}

        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Water")
        {
			// Execute die function on collision with an object tagged enemy or water.
            Die();
        }
	}

    void OnParticleCollision(GameObject collision)
    {
		// Execute slow player function on collision with particles.
        SlowPlayer();
    }

	async void SlowPlayer() 
	{
		if (!playerSlowed)
		{
			// Slow the walking speed for 3 seconds.
			playerSlowed = true;
        	furSprite.color = new Color(0.46f, 0.86f, 0.46f, 1);
			movementSpeed = maxMovementSpeed / 3;
			await Task.Delay(3000);
        	furSprite.color = new Color(1, 1, 1, 1);
			movementSpeed = maxMovementSpeed;
			playerSlowed = false;
		}
	}

	void Idle()
	{
		if (currentState != "JumpingOut" && currentState != "JumpingIn")
		{
			// If standing still, start the idle timer.
			if (rigidBody2D.velocity.sqrMagnitude <= 0.01f && idleTimer <= 5)
			{
				idleTimer += Time.deltaTime;
			}
			else if (idleTimer >= 5 && currentState != "Sit")
			{
				// Once idle timer goes above or is equal to 5, play sit animation.
				idleTimer = 0;
				ChangeAnimationState("Sit");
			}
		}
	}

	public void ChangeAnimationState(string newState)
	{
		if (currentState != "Death")
		{
			// Play a new animation as long as it is not the same.
			if (currentState == newState)
			{
				return;
			}

			characterAnimator.Play(newState);
			currentState = newState;
		}
	}

	public void Die()
	{
		// Add 1 to transfer object and player prefs for player statistics and stop the camera from following the character object.
		PlayerPrefs.SetInt("Deaths", PlayerPrefs.GetInt("Deaths") + 1);
		Follow.scriptInstance.cameraSpeed = 0;
		Transfer.scriptInstance.deathsCounter = Transfer.scriptInstance.deathsCounter + 1;
		// Disable the collider component to allow the bunny to fall out of the map and play the death animation.
		GetComponent<Collider2D>().enabled = false;
		ChangeAnimationState("Death");
		// Change the sorting layer to 6 to make the character object visible above other layers.
		gameObject.GetComponent<SpriteRenderer>().sortingOrder = 6;
		// Play death animation to hide the HUD.
		GameObject.Find("HUD").GetComponent<Animator>().Play("Death");
		StartCoroutine(Sequence());
		// Throw the character slightly upwards.
		rigidBody2D.velocity = Vector2.up * 12;
	}

	IEnumerator Sequence()
	{
		// Wait 1 second before loading the same scene.
		yield return new WaitForSeconds(1);
		string sceneName = SceneManager.GetActiveScene().name;
		GetComponent<Load>().Loading(sceneName);
	}
}
