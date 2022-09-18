using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float touchSpeed = 2f, levelheight = 0;
    [SerializeField] float speed = 15f, originalSpeed = 15f;
    [SerializeField] float reduceSpeed = 2f;
    [SerializeField] float jumpSpeed = 8f;
    [SerializeField] float holdDistance = 0.12f;
    [SerializeField] Animator animator;
    [SerializeField] string run = "RunA_front@loop", stand = "StandA_idleA";
    [SerializeField] GameObject step, hold, holdStack;
    ScannerMovement scanner;
    GameManager gameManager;

    private float horizontalInput;

    private Rigidbody playerRb;
    public float distance = 1f;
    private bool canJump = true, isOnPod = false, isAlive = true;
    private int plankCounter = 0;

    Vector3 holdPosition = new Vector3(0, 1.2f, 0.3f);

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        scanner = GameObject.Find("Scanner").GetComponent<ScannerMovement>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void Update()
    {
        //Debug.Log(plankCounter);
        if (isAlive && gameManager.isGameActive)
        {
            animator.Play(run);
            MovePlayer();

            if (IsGrounded() && !isOnPod)
            {
                canJump = true;
                speed = originalSpeed;
            }
            if (!IsGrounded() && canJump && plankCounter == 0)
            {
                PerformJump(jumpSpeed, reduceSpeed);
            }
            else if (!IsGrounded() && !IsJumping() && plankCounter > 0)
            {
                SpawnPlankOnWater();
            }
        }
        else
        {
            animator.Play(stand);
        }
    }

    void SpawnPlankOnWater()
    {
        Vector3 stepPosition = transform.position;
        stepPosition.y = -1;

        Instantiate(step, stepPosition, transform.rotation);
        DecreaseHold();
    }

    void MovePlayer()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        MoveByTouch();
        MoveByKeyboard();
    }

    private void MoveByKeyboard()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        Vector3 movementDirection = new Vector3(0, horizontalInput, 0);
        movementDirection.Normalize();

        transform.Rotate(movementDirection);
    }

    private void MoveByTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 movementDirection = new Vector3(0, touch.deltaPosition.x, 0);
                movementDirection.Normalize();

                transform.Rotate(movementDirection*touchSpeed);
            }
        }
    }

    private void PerformJump(float jumpSpeed, float reduceSpeed)
    {
        speed /= reduceSpeed;
        playerRb.velocity = Vector3.up * jumpSpeed;
        canJump = false;
    }

    private bool IsJumping()
    {
        if (transform.position.y <= levelheight)
            return false;
        return true;
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, distance);
    }    

    private void OnCollisionEnter(Collision collision)
    {
        string s = collision.gameObject.tag;
        if(s == "Pod")
        {
            isOnPod = true;
            PerformJump(jumpSpeed * 2, 0.5f);
        }
        else
        {
            isOnPod = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Plank"))
        {
            plankCounter++;

            IncreaseHold();
            
            Destroy(other.gameObject);
        }
        
        if(other.gameObject.CompareTag("Water"))
        {
            isAlive = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if(other.gameObject.CompareTag("Finish"))
        {
            scanner.isActive = true;
        }
    }

    private void IncreaseHold()
    {
        Vector3 tempPosition = holdStack.transform.position;
        int childCount = holdStack.transform.childCount;
        if (childCount > 0)
        {
            tempPosition = holdStack.transform.GetChild(childCount - 1).position;
            tempPosition.y += holdDistance;
            var child = Instantiate(hold, tempPosition, transform.rotation);
            child.transform.parent = holdStack.transform;
        }
        else
        {
            var child = Instantiate(hold, tempPosition, transform.rotation);
            child.transform.parent = holdStack.transform;
        }
        
    }

    private void DecreaseHold()
    {
        plankCounter--;
        if (holdStack.transform.childCount>0)
            GameObject.Destroy(holdStack.transform.GetChild(holdStack.transform.childCount - 1).gameObject);
    }
}
