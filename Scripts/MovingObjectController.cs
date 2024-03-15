
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 0.1f;
    public float moveRange = 1.0f;
    private float startPositionX;
    //private bool isMovingRight = false;
    public Vector2 pointA;
    public Vector2 pointB;
    public float speed = 1.0f;
    private Vector2 currentTarget;

    // Start is called before the first frame update
    void Start()
    {
        currentTarget = pointA;
    }


    void Awake()
    {
        // rigidBody = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
        startPositionX = this.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);

        if (new Vector2(transform.position.x, transform.position.y) == currentTarget)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }
    private void MoveRight()
    {
        transform.position = new Vector2(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y);
    }

    private void MoveLeft()
    {
        transform.position = new Vector2(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y);
    }
}
