using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerScript : MonoBehaviour
{
	
	public Rigidbody2D playerRigidbody;
	public float moveSpeed = 0.3f;
	
	public int playerLife = 1;
	
	public LogicScript logic;
	
	public GameObject pauseScreen;
	public GameObject gameOverScreen;
	
	Vector3 clickPosition;
	
	Coroutine moveCoroutine;
	
    // Start is called before the first frame update
    void Start()
    {
		//Prevent unwanted physic force applying to player object
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerRigidbody.gravityScale = 0f;

		logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    // Update is called once per frame
    void Update()
    {
		//If user click
		if ((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began || Input.GetMouseButtonDown(0)) && playerLife > 0 && !pauseScreen.activeSelf)
		{
			//If user didn't click button or text
			if (!EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
			{
				
				// Get the position of the mouse click in world space
				clickPosition = Camera.main.ScreenToWorldPoint(Input.touches[0].position); //Input.mousePosition

				// Make sure the z-coordinate is 0
				clickPosition.z = 0f;

				//To cut off/abandon the movement midway
				//So it immediately register the next move
				if (moveCoroutine != null)
				{
					StopCoroutine(moveCoroutine);
				}

				// Rotate the object to face the click position
				//Ps. Minus more 90 degrees so the player face properly
				Vector3 direction = clickPosition - transform.position;
				float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
				transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

				// Start the coroutine to move the object to the click position
				//float distance = Vector3.Distance(transform.position, clickPosition);
				//float duration = distance / moveSpeed;
				moveCoroutine = StartCoroutine(MoveTo(clickPosition, moveSpeed));
			}
		} 
		
		//If game over / game pause, player should stay still
		if (playerLife < 1 || pauseScreen.activeSelf) 
		{
			playerRigidbody.velocity = Vector2.zero;
		}
	
    }
	
    // Coroutine to move the object to a target position over time
    IEnumerator MoveTo(Vector3 targetPosition, float duration)
    {
		
        // Save the starting position of the object
        Vector3 startPosition = transform.position;

        // Initialize the time elapsed to 0
        float timeElapsed = 0f;

        // Loop until the elapsed time reaches the duration
        while (timeElapsed < duration)
        {
			if (!pauseScreen.activeSelf)
			{

				// Increment the elapsed time by the time since the last frame
				timeElapsed += Time.deltaTime;

				// Calculate the t value for lerping between start and target positions
				float t = Mathf.Clamp01(timeElapsed / duration);

				// Move the object towards the target position
				transform.position = Vector3.Lerp(startPosition, targetPosition, t);

			}
			// Wait for the next frame
			yield return null;
			
        }
	
    }
	
	//If player hit obstacle, loses life
	//Usually player only has 1 life, so it loses basically
	private void OnCollisionEnter2D(Collision2D collision)
	{
		playerLife -= 1;
		
		lose();
	}
	
	//Trigger Game Over
	private void lose()
	{
		if (playerLife < 1){
			
			if (moveCoroutine != null)
			{
				StopCoroutine(moveCoroutine);
			}
			
			playerRigidbody.gravityScale = 0f;
			playerRigidbody.velocity = Vector2.zero;
			logic.gameOver();	
		}
	}
}
