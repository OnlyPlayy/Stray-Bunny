using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class AllergiCat : MonoBehaviour
{
    // Object variables.
    [Header("Objects")]
    [SerializeField] private GameObject exclamationMark;
    [SerializeField] private Transform castPoint;
    [SerializeField] private Collider2D bodyCollider;
    [SerializeField] private Transform floorDetector;
    [SerializeField] private LayerMask layerGround;
    [SerializeField] private LayerMask layerLimiter;
    [SerializeField] private GameObject sneezeParticle;
    [SerializeField] private GameObject enemyDieParticle;

    [Header("Settings")]
    [SerializeField] [Range(1, 6)] private float enemySpeed = 2;
    [SerializeField] [Range(1, 6)] private float speedMultiplier = 3;
    [SerializeField] [Range(0.1f, 10)] private float edgeFallingHeight = 0.6f;
    [SerializeField] [Range(3, 20)] private float viewField = 10;
    [SerializeField] [Range(0.01f, 2)]private float distanceToWallTurn = 0.2f;
    [SerializeField] [Range(0.1f, 2)] private float exclamationTime = 0.6f;
    [SerializeField] [Range(0.1f, 2)] private float waitBeforeSneeze = 1;
    [SerializeField] [Range(0.1f, 2)] private float waitAfterSneeze = 1;

    private bool isMovingRight = true, isChasing, noticedPlayer, running, haoticBool, fallingDown;
    private RaycastHit2D obstacleInfo;
    private SpriteRenderer exclamationSprite;
    private float pos1, pos2;
	private Animator allergicatAnimator;

    void Start()
    {
        // Extract the sprite renderer component from the enemy object and set color.
        exclamationSprite = exclamationMark.GetComponent<SpriteRenderer>();
        exclamationSprite.color = new Color(1, 1, 1, 0);
        allergicatAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // Change behaviour on action.
        if (!isChasing)
        {
            PatrolMove();
        }

        if (isChasing && running)
        {
            HaoticRun();
        }

        if (CanSeePlayer(viewField) && !isChasing)
        {
            StartCoroutine(ChasePlayer());
        } 

    }
 
    void PatrolMove()
    {
        // Move the enemy in a given direction at a set speed.
        allergicatAnimator.Play("Walk");
        transform.Translate(Vector2.right * enemySpeed * Time.deltaTime);
        RaycastHit2D groundInfo = Physics2D.Raycast(floorDetector.position, Vector2.down, edgeFallingHeight, ~layerLimiter);
        obstacleInfo = Physics2D.Raycast(floorDetector.position, Vector2.right * (isMovingRight ? 1 : -1), distanceToWallTurn, ~layerLimiter);

        if (bodyCollider.IsTouchingLayers(layerGround))
        {
            if (groundInfo.collider == false)
            {
                // Turn the enemy in the opposite direction if it gets close to an edge.
                TurnAround();
            }
            
            if (obstacleInfo.collider != null)
            {
                if (obstacleInfo.collider.tag == "Ground" || obstacleInfo.collider.tag == "Enemy")
                {
                    // If the enemy raycast hits an object tagged ground or enemy, turn it in the opposite direction.
                    TurnAround();
                }
            }
        }
    }

    async void HaoticRun() 
    {
        // Make the enemy object move faster over time.
        allergicatAnimator.Play("Run");
        transform.Translate(Vector2.right * enemySpeed * Time.deltaTime * speedMultiplier);

        // Execute enemy die function if it falls of the edge.
        if (!bodyCollider.IsTouchingLayers(layerGround)){
            fallingDown = true;
        }

        if (fallingDown && bodyCollider.IsTouchingLayers(layerGround))
        {
            EnemyDie();
        }

        // Execute enemy die function if it hits a heavy object.
        if (haoticBool == false)
        {
            pos1 = transform.position.x;
            haoticBool = !haoticBool;
        }
        else
        {
            pos1 = pos2;
            pos2 = transform.position.x;
        }
        
        if (pos1 != 0 && pos2 != 0)
        {
            float calcPos = pos1 - pos2;
            float movingDir;
            movingDir = isMovingRight ? 1 : -1;
            
            if ((calcPos * movingDir) > 0.01f)
            {
                EnemyDie();
            }
        }

        await Task.Delay(0);
    }

    void TurnAround()
    {
        // Turn the enemy in a given direction.
        if (isMovingRight == true)
        {
            gameObject.transform.eulerAngles = new Vector3(0, -180, 0);
            isMovingRight = false;
        }
        else
        {
            gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
            isMovingRight = true;
        }
    }

    async public void EnemyDie()
    {
        // Create a particle system object at the position of the enemy object and destroy the enemy object.
        GameObject enemyDiePart = Instantiate(enemyDieParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);

        // Destroy the particle system object after 1 second.
        await Task.Delay(1000);
        DestroyImmediate(enemyDiePart);
    }

    bool CanSeePlayer(float distance)
    {
        // Check if the enemy sees the player.
        bool val = false;
        float castDist = distance;

        if (!isMovingRight)
        { 
            castDist = -distance;
        }

        Vector2 endPos = castPoint.position + Vector3.right * castDist;
        RaycastHit2D hit = Physics2D.Linecast(castPoint.position, endPos, ~layerLimiter);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                val = true;
                Debug.DrawLine(castPoint.position, hit.point, Color.green);
            }
            else
            {
                val = false;
                Debug.DrawLine(castPoint.position, hit.point, Color.red);
            }
        }
        else
        {
            Debug.DrawLine(castPoint.position, endPos, Color.blue);
        }

        return val;
    }

    IEnumerator ChasePlayer()
    {
        // Start slowly changing the exclamation mark opacity to indicate its charging progress.
        if (!isChasing && !noticedPlayer)
        {
            noticedPlayer = true;
            exclamationSprite.color = new Color(1, 1, 1, 0.3f);

            yield return new WaitForSeconds(exclamationTime);
            exclamationSprite.color = new Color(1, 1, 1, 0);
            
            if (CanSeePlayer(viewField) && !isChasing)
            {
                isChasing = true;
                allergicatAnimator.Play("Sneeze");
                exclamationSprite.color = new Color(1, 1, 1, 1);
                yield return new WaitForSeconds(waitBeforeSneeze);

                // Create a particle system object at the position of the enemy object.
                GameObject sneezePart = Instantiate(sneezeParticle, transform.position, Quaternion.identity);

                // Start charging in the direction of the player character and destroy the particle system object.
                yield return new WaitForSeconds(waitAfterSneeze);
                running = true;
                DestroyImmediate(sneezePart);
            }

            noticedPlayer = false;
        }
    }
}