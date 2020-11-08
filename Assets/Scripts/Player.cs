using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    //  Config
    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private Animator myAnimator;
    [SerializeField]
    float jumpSeed = 5f;

    //  State
    //bool isAlive = true;
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    public bool detectSwipeOnlyAfterRelease = false;

    public float SWIPE_THRESHOLD = 20f;

    //  Cached Component References
    Rigidbody2D myRigidBody;

    private void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        Run();
        Jump();
        FlipSprite();
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUp = touch.position;
                fingerDown = touch.position;
            }

            //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
                if (!detectSwipeOnlyAfterRelease)
                {
                    fingerDown = touch.position;
                    CheckSwipe();
                }
            }

            ////Detects swipe after finger is released
            //if (touch.phase == TouchPhase.Ended)
            //{
            //    fingerDown = touch.position;
            //    checkSwipe();
            //}
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary)
        {
            Vector2 touchPosition = Input.GetTouch(0).position;
            double halfScreen = Screen.width / 2.0;

            //Check if it is left or right?
            if (touchPosition.x < halfScreen)
            {
                transform.localScale = new Vector2(-1, 1);
                transform.Translate(Vector3.left * 5 * Time.deltaTime);
                myAnimator.SetBool("Running", true);
            }
            else if (touchPosition.x > halfScreen)
            {
                transform.localScale = new Vector2(1, 1);
                transform.Translate(Vector3.right * 5 * Time.deltaTime);
                myAnimator.SetBool("Running", true);
            }

        }
    }

    private void Run()
    {
        float rawHorizontalAxis = Input.GetAxisRaw("Horizontal");
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;

        Vector2 playerVelocity = new Vector2(rawHorizontalAxis * speed, myRigidBody.velocity.y);

        myRigidBody.velocity = playerVelocity;

        myAnimator.SetBool("Running", playerHasHorizontalSpeed);        
    }

    private void Jump()
    {
        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;

        if (Input.GetButtonDown("Jump") && !playerHasVerticalSpeed)
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSeed);
            myRigidBody.velocity += jumpVelocityToAdd;
        }

        myAnimator.SetBool("Jumping", playerHasVerticalSpeed);
    }

    private void ExecuteJump()
    {
        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        if(!playerHasVerticalSpeed)
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSeed);
            myRigidBody.velocity += jumpVelocityToAdd;            
        }
        myAnimator.SetBool("Jumping", playerHasVerticalSpeed);
    }

    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        if(playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1);
        }
    }

    private void CheckSwipe()
    {
        //Check if Vertical swipe
        if (verticalMove() > SWIPE_THRESHOLD && verticalMove() > horizontalValMove())
        {
            //Debug.Log("Vertical");
            if (fingerDown.y - fingerUp.y > 0)//up swipe
            {
                OnSwipeUp();
            }
            else if (fingerDown.y - fingerUp.y < 0)//Down swipe
            {
                OnSwipeDown();
            }
            fingerUp = fingerDown;
        }

        //Check if Horizontal swipe
        else if (horizontalValMove() > SWIPE_THRESHOLD && horizontalValMove() > verticalMove())
        {
            //Debug.Log("Horizontal");
            if (fingerDown.x - fingerUp.x > 0)//Right swipe
            {
                OnSwipeRight();
            }
            else if (fingerDown.x - fingerUp.x < 0)//Left swipe
            {
                OnSwipeLeft();
            }
            fingerUp = fingerDown;
        }

        //No Movement at-all
        else
        {
            Debug.Log("No Swipe!");
        }
    }

    float verticalMove()
    {
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    float horizontalValMove()
    {
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }

    //////////////////////////////////CALLBACK FUNCTIONS/////////////////////////////
    void OnSwipeUp()
    {
        Debug.Log("Swipe UP");
        ExecuteJump();
    }

    void OnSwipeDown()
    {
        Debug.Log("Swipe Down");
    }

    void OnSwipeLeft()
    {
        Debug.Log("Swipe Left");
    }

    void OnSwipeRight()
    {
        Debug.Log("Swipe Right");
    }
}