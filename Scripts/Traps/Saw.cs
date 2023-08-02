using UnityEngine;

public class Saw : MonoBehaviour
{
	// Object variables.
	[Header("Settings")]
	[SerializeField] [Range(1, 10)] private float moveSpeed = 1;
	[SerializeField] [Range(0, 100)] private float horizontalDistance;
	[SerializeField] [Range(0, 100)] private float verticalDistance;

	private Vector2 startingPosition;
	private bool movingRight = true;

	void Start()
	{
		// Get the current position of the object.
		startingPosition = transform.position;
	}

	void Update()
	{
		// Move to object at a set speed for a set distance in vertical or horizontal direction.
		// Once the distance is reached, either turn the saw or make it move back to its starting position.
		if (movingRight)
		{
			if (transform.position.x < startingPosition.x + horizontalDistance)
			{
				transform.position = new Vector2(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y);
			}
			else if (transform.position.y < startingPosition.y + verticalDistance)
			{
				transform.position = new Vector2(transform.position.x, transform.position.y + moveSpeed * Time.deltaTime);
			}
			else
			{
				// Change bool to opposite.
				movingRight = !movingRight;
			}
		}
		else
		{
			if (transform.position.x > startingPosition.x)
			{
				transform.position = new Vector2(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y);
			}
			else if (transform.position.y > startingPosition.y)
			{
				transform.position = new Vector2(transform.position.x, transform.position.y - moveSpeed * Time.deltaTime);
			}
			else
			{
				// Change bool to opposite.
				movingRight = !movingRight;
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
