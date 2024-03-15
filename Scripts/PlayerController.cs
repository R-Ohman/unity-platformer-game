using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;




public class PlayerController : MonoBehaviour
{
    [Header("Movement parameters")]
    [Range(0.01f, 20.0f)] [SerializeField] private float moveSpeed = 0.1f;
    
    [Range(0.5f, 20.0f)] [SerializeField] private float jumpForce = 6.0f;
    [Space(10)]
    private Rigidbody2D rigidBody;
    public LayerMask groundLayer;
    const float rayLength = 1.6f;
    private Animator animator;
    private bool isWalking = false;
    private bool isFacingRight = true;
    private int score = 0;
    private bool enteredLadder = false;
    public int lives = 3;
    private Vector2 startPosition;
    private int keysNumber = 0;

    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioClip cherrySound;
    [SerializeField] private AudioClip killSound;
    [SerializeField] private AudioClip deathSound;
    private AudioSource source;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = this.transform.position;
        source = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("other.CompareTag(\"Ladder\")" + other.CompareTag("Ladder"));
        if (other.CompareTag("Ladder"))
        {
            Debug.Log("ladder");
            enteredLadder = true;

            if (!IsGrounded())
            {
                animator.SetBool("isOnLadder", true);
            }
        }
        if (other.CompareTag("Key"))
        {
            if (coinSound != null)
            {
                //Debug.Log("SOUND");
                source.PlayOneShot(coinSound, AudioListener.volume);
            }
            GameManager.instance.AddKeys(keysNumber);
            keysNumber++;
            Debug.Log("Keys: " + keysNumber);
            other.gameObject.SetActive(false);
        }
        if (other.CompareTag("FallLevel"))
        {
            if (deathSound != null)
            {
                //Debug.Log("SOUND");
                source.PlayOneShot(deathSound, AudioListener.volume);
            }
            Death();
        }
        if (other.CompareTag("Heart"))
        {
            GameManager.instance.AddHeart(lives);
            lives++;
            
            Debug.Log("Lives: " + lives);
            other.gameObject.SetActive(false);
        }
        if (other.CompareTag("Bonus"))
        {
            if (cherrySound != null)
            {
                //Debug.Log("SOUND");
                source.PlayOneShot(cherrySound, AudioListener.volume);
            }
            GameManager.instance.AddPoints(1);
            score++;
            Debug.Log("Score: " + score);
            other.gameObject.SetActive(false);
        }
        if (other.CompareTag("Moving Platform"))
        {
            Debug.Log("Collision with moving platform");
            transform.SetParent(other.transform);
        }
        if (other.CompareTag("Finish level"))
        {
            int keysLeftOnMap = GameObject.FindGameObjectsWithTag("Key").Length;
            if (keysLeftOnMap == 0)
            {
                score = score + lives * 100;
                GameManager.instance.AddPoints(lives * 100);
                GameManager.instance.LevelCompleted();
                //EditorUtility.DisplayDialog("Congratulations!", "You have finished the level!", "OK");
                //EditorApplication.isPlaying = false;
            }
            else
            {
                Debug.Log("You need collect more gems (" + keysLeftOnMap + ")");
            }
        }
        if (other.CompareTag("Enemy"))
        {
            if (transform.position.y > other.gameObject.transform.position.y)
            {
                if (killSound != null)
                {
                    //Debug.Log("SOUND");
                    source.PlayOneShot(killSound, AudioListener.volume);
                }
                score++;
                GameManager.instance.UpdateEnemiesUI();
                Debug.Log("Killed an enemy");
            }
            else
            {
                if (deathSound != null)
                {
                    //Debug.Log("SOUND");
                    source.PlayOneShot(deathSound, AudioListener.volume);
                }
                Death();
            }
        }
    }
    private void Death()
    {
        this.transform.position = startPosition;
        lives--;
        GameManager.instance.RemoveHeart(lives);
        if (lives == 0)
        {
            EditorUtility.DisplayDialog("Game over!", "Your score: " + score, "OK");
            EditorApplication.isPlaying = false;
        }
        else
        {
            Debug.Log("You are dead. Lives left: " + lives);
        }
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    // Update is called once per frame
    void Update()
    {
        isWalking = false;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            if (!isFacingRight)
            {
                Flip();
            }
            isWalking = true;
            transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            if (isFacingRight)
            {
                Flip();
            }
            isWalking = true;
            transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (enteredLadder)
            {
                Debug.Log("ladder");
                animator.SetBool("isOnLadder", true);
            }
        }

        Debug.DrawRay(transform.position, rayLength * Vector3.down, Color.white, 1, false);
        animator.SetBool("isGrounded", IsGrounded());
        animator.SetBool("isWalking", isWalking);
    }
    
    bool IsGrounded()
    {
        float player_width = 0.8f;
        Vector3 left_player_position = new Vector3(this.transform.position.x - player_width / 2, this.transform.position.y, this.transform.position.z);
        Vector3 right_player_position = new Vector3(this.transform.position.x + player_width / 2, this.transform.position.y, this.transform.position.z);

        return Physics2D.Raycast(left_player_position, Vector2.down, rayLength, groundLayer.value) ||
                Physics2D.Raycast(right_player_position, Vector2.down, rayLength, groundLayer.value);
    }
    void Jump()
    {
        if (IsGrounded())
        {
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            Debug.Log("jump");
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("other.CompareTag(\"Ladder\")" + other.CompareTag("Ladder"));
        if (other.CompareTag("Ladder"))
        {
            animator.SetBool("isOnLadder", false);
            enteredLadder = false;
        }
        if (other.CompareTag("Moving Platform"))
        {
            Debug.Log("Exit from moving platform");
            transform.SetParent(null);
        }
    }
}
