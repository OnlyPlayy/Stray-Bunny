using UnityEngine;
using UnityEngine.SceneManagement;

public class Follow : MonoBehaviour
{
    // Object variables.
    [Header("Objects")]
    [SerializeField] private BoxCollider2D[] cameraBoundaries;

    [Header("Settings")]
    [Range(1, 25)] public float cameraSpeed = 5;
    [SerializeField] private bool multipleBounds;

    [HideInInspector] public static Follow scriptInstance;
    private Bounds cameraBounds;
    private Transform boundaryPosition;
    private BoxCollider2D boxCollider2D;
    private Camera mainCamera;
    private float ratioX;
    private float ratioY;
    private float sizeX;
    private float sizeY;

    void Awake()
    {
        // Create a referencing instance if null.
        if (scriptInstance == null)
        {
            scriptInstance = this;
        }
    }

    void Start()
    {
        // Extract box collider and camera components from the camera object.
        boxCollider2D = GetComponent<BoxCollider2D>();
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        Size();
    }

	void FixedUpdate()
	{
        Track();
    }

    void Size()
    {
        // Keep track of the screen size to maintain correct aspect ratio for the camera boundary.
        ratioX = (float)Screen.width / (float)Screen.height;
        ratioY = (float)Screen.height / (float)Screen.width;
        sizeX = (Mathf.Abs(mainCamera.orthographicSize) * 2 * ratioX);
        sizeY = sizeX * ratioY;
        boxCollider2D.size = new Vector2(sizeX, sizeY);
    }

    void Track()
    {
        // Look for player object in order to keep the camera following the player.
        Transform playerCharacter = GameObject.Find("Player").GetComponent<Transform>();
        if (GameObject.Find("Player") && GameObject.Find("Boundary"))
        {
            if (multipleBounds == false)
            {
                BoxCollider2D boundary = GameObject.Find("Boundary").GetComponent<BoxCollider2D>();
                cameraBounds = boundary.bounds;
                boundaryPosition = boundary.GetComponent<Transform>();
            }
            else
            {
                for (int i = 0; i < cameraBoundaries.Length; i++)
                {
                    if (playerCharacter.position.x > cameraBoundaries[i].bounds.min.x && playerCharacter.position.x < cameraBoundaries[i].bounds.max.x && playerCharacter.position.y > cameraBoundaries[i].bounds.min.y && playerCharacter.position.y < cameraBoundaries[i].bounds.max.y)
                    {
                        cameraBounds = cameraBoundaries[i].bounds;
                        boundaryPosition = cameraBoundaries[i].GetComponent<Transform>();
                    }
                }
            }

            // Make the camera smoothly follow the player character at a set smoothing speed.
            float targetPositionX = boxCollider2D.size.x < cameraBounds.size.x ? Mathf.Clamp(playerCharacter.position.x, cameraBounds.min.x + boxCollider2D.size.x / 2, cameraBounds.max.x - boxCollider2D.size.x / 2) : boundaryPosition.transform.position.x;
            float targetPositionY = boxCollider2D.size.y < cameraBounds.size.y ? Mathf.Clamp(playerCharacter.position.y, cameraBounds.min.y + boxCollider2D.size.y / 2, cameraBounds.max.y - boxCollider2D.size.y / 2) : boundaryPosition.transform.position.y;
            Vector3 targetPosition = new Vector3(targetPositionX, targetPositionY, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed * Time.deltaTime);
        }
    }
}