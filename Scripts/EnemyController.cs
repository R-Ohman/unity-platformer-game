using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class EnemyController : MonoBehaviour
{

    [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 0.1f;
    private bool isFacingRight = false;
    private Animator animator;
    private float startPositionX;
    public float moveRange = 1.0f;
    private bool isMovingRight = false;

    void Awake()
    {
        // rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPositionX = this.transform.position.x;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovingRight)
        {
            if (this.transform.position.x <= startPositionX + moveRange)
            {
                MoveRight();
            }
            else if (moveRange < 0 && this.transform.position.x <= startPositionX)
            {
                MoveRight();
            }
            else
            {
                Flip();
                MoveLeft();
            }
        }
        else
        {
            if (this.transform.position.x >= startPositionX)
            {
                MoveLeft();
            }
            else if (moveRange < 0 && this.transform.position.x >= startPositionX + moveRange)
            {
                MoveLeft();
            }
            else
            {
                Flip();
                MoveRight();
            }
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        isMovingRight = !isMovingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void MoveRight()
    {
        transform.position = new Vector2(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y);
    }

    private void MoveLeft()
    {
        transform.position = new Vector2(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.transform.position.y > transform.position.y)
            {
                animator.SetBool("isDead", true);
                StartCoroutine(KillOnAnimationEnd());
            }

        }
    }

    IEnumerator KillOnAnimationEnd()
    {
        yield return new WaitForSeconds(1.0f); // time of the death animation
        gameObject.SetActive(false);
    }
}
