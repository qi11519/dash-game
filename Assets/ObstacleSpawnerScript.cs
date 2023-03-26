using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawnerScript : MonoBehaviour
{
	
	public GameObject obstacle;
	public GameObject pauseScreen;
	
	//public float spawnRate; //= 0.5;
	public float offsetRate1 = 2f;
	public float offsetRate2 = 2.5f;
	private float timer = 0;
	
	public Camera camera;
	
	public PlayerScript player;
	public LogicScript logic;
	
    // Start is called before the first frame update
    void Start()
    {
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
		logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
	
		//Get camera view of the game(based on phone)
		camera = Camera.main;
		
		//Spawn obstacle
        spawnObstacle();
    }

    // Update is called once per frame
    void Update()
    {
		controlSpawn();
    }
	
	//Spawn obstacle
	void spawnObstacle()
	{
		//Basically here is getting width of the game, 
		//then spawn the obstacle within the range of the width
		float halfHeight = camera.orthographicSize;
		float halfWidth = camera.aspect * halfHeight;
		
		float leftPoint = transform.position.x - halfWidth;
		float rightPoint = transform.position.x + halfWidth;
		
		//Instantiate = spawn command
		Instantiate(obstacle, new Vector3(Random.Range(leftPoint,rightPoint), transform.position.y, 0), transform.rotation);
	}
	
	//For controlling the setting of spawn object
	void controlSpawn(){
		
		//For scaling up the obstacle spawn rate, smaller rate = more obstacle 
		//Feel free to change the speed here for adjusting difficulty
		if (logic.timeScore > 10)
		{
			offsetRate1 = 0.2f;
			offsetRate2 = 0.4f;
		} else if (logic.timeScore > 20)
		{
			offsetRate1 = 0.1f;
			offsetRate2 = 0.2f;
		} /*else if (logic.timeScore > 10)
		{
			offsetRate1 = 0.1f;
			offsetRate2 = 0.2f;
		}*/
			
		//Add up a second spawn 
		if (player.playerLife > 0 && !pauseScreen.activeSelf)
		{
			determineSpawn();
			
			int spawn2 = Random.Range(1,7);
			
			//This one is for extra spawn obstacle, for increasing the difficulty, Optional
			if (logic.timeScore > 30 && spawn2 < 4){
				determineSpawn();
			}
		}
	}
	
	//Random spawnrate then spawn obstacle
	void determineSpawn()
	{
		//Random a number in between spawn rate offset
		float spawnRate = Random.Range(offsetRate1, offsetRate2);
		
		if (timer < spawnRate){ //Making sure only spawn on the calculated timer
				timer += Time.deltaTime;
		} 
		else 
		{	//Spawn obstacle on the reached timer
			spawnObstacle();
			timer = 0;
		}
	}
}
