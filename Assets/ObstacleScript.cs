using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
	public Camera camera;
	
	public Rigidbody2D obstacleRigidbody;
	
	public LogicScript logic;
	public PlayerScript player;
	
	public GameObject pauseScreen;
	 
	public float fallSpeed = 0.03f;
	public float deadZone = -1;
	public float detectionRange;
	
    // Start is called before the first frame update
    void Start()
    {
		//Get the logic script & the player script
		logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();

		//Obstacle's gravity scaling, set to 0 so it won't accelerate
        obstacleRigidbody = GetComponent<Rigidbody2D>();
        obstacleRigidbody.gravityScale = 0f;

		//This is the camera view
		camera = Camera.main;
		
		//Get the screen height and width
		float screenHeight = Camera.main.orthographicSize * 2f;
		float screenWidth = screenHeight * Camera.main.aspect;
		
		//This part is to get a 2% width of the screen width
		//The obtained width will act as the detection range
		//If two obstacle is too close( based on detection range ), the newer one will get destroy
		float screenPercentage = 0.02f; // desired screen percentage
		float screenSize = Camera.main.orthographicSize * 2f; // get screen size in world units
		float worldToScreenRatio = screenPercentage / screenSize;		
		detectionRange = screenWidth * worldToScreenRatio;

		CheckAndDestroyNearbyObstacles();
    }

    // Update is called once per frame
    void Update()
    {
		//Get pauseSCreen
		//Put it in Update() not Start() is for checking the state of pauseScreen all the time
		pauseScreen = logic.pauseScreen;
		
		scaleSpeed();
    }
	
	//For scaling up the obstacle speed
	//Feel free to change the speed here for adjusting difficulty
	private void scaleSpeed(){
		//If player not dies yet && the screen is not pausing
		if (player.playerLife > 0 && !pauseScreen.activeSelf)
		{
			if (logic.timeScore < 10)
			{
				fallSpeed = 0.03f;
			} else if (logic.timeScore > 10)
			{
				fallSpeed = 0.05f;
			} else if (logic.timeScore > 20)
			{
				fallSpeed = 0.07f;
			} else if (logic.timeScore > 30)
			{
				fallSpeed = 0.09f;
			}
			
			//Get bottom edge of the screen
			obstacleRigidbody.MovePosition(obstacleRigidbody.position + Vector2.down * fallSpeed);
			float bottomEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
			
			//If the obstacle went off screen(vertically), it will auto destroy
			if (transform.position.y < bottomEdge)
			{
			   Destroy(gameObject);
			}
			
		} else { //If game over & screen paused, set all the speed to 0
			obstacleRigidbody.velocity = Vector2.zero;
			fallSpeed = 0.0f;
		}
	}
	
	
	//This is the built-in on-click function, but for collision
	//For the purpose of letting the obstacle won't fly to nowhere after being hit/collide by player
	private void OnCollisionEnter2D(Collision2D collision)
	{
		obstacleRigidbody.velocity = Vector2.zero;
	}
	
	//Check if two obstacle too close, if so, delete the higher obstacle(since newer obstacle spawned later)
	void CheckAndDestroyNearbyObstacles()
	{
		// Define a circle centered at the current obstacle's position with a radius equal to the detection range
		Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);
		
		foreach (Collider2D nearbyCollider in nearbyColliders)
		{
			if (nearbyCollider.CompareTag("Obstacle")) //Check if the object is tag with "Obstacle"
			{
				// Check if the nearby obstacle is higher than the current obstacle
				if (nearbyCollider.transform.position.y < transform.position.y)
				{
					// Destroy the current obstacle
					Destroy(gameObject);
					break;
				}
			}
		}
	}

}
