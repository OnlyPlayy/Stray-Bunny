using UnityEngine;

public class Manager : MonoBehaviour
{
	// Object variables.
	[Header("Objects")]
	[SerializeField] private GameObject cameraBoundary;

	private BoxCollider2D boxCollider2D;
	private Transform playerCharacter;

	void Start()
	{
		// Extract box collider component from the manager object.
		boxCollider2D = GetComponent<BoxCollider2D>();
	}

	void Update()
	{
		// Look for a player object.
		playerCharacter = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
		Boundary();
	}

	void Boundary()
	{
		if (playerCharacter != null)
		{
			// Execute the function as long as the player object exists.
			// When the player is colliding with the object keep it active to maintain the camera within this area, otherwise disable it and stop following the player.
			if (boxCollider2D.bounds.min.x < playerCharacter.position.x && playerCharacter.position.x < boxCollider2D.bounds.max.x && boxCollider2D.bounds.min.y < playerCharacter.position.y && playerCharacter.position.y < boxCollider2D.bounds.max.y)
			{
				cameraBoundary.SetActive(true);
			}
			else
			{
				cameraBoundary.SetActive(false);
			}
		}
	}
}
