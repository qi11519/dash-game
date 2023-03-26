using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
	//public int playerScore;
	//public Text scoreText;
	public float timeScore = 0;
	public Text timeText;
	public GameObject gameOverScreen;
	public GameObject pauseScreen;
	public PlayerScript player;
	public Button pauseButton;
	
    // Start is called before the first frame update
    void Start()
    {
		//Get player script
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();

    }

    // Update is called once per frame
    void Update()
    {
		//If game over & game paused
		if( player.playerLife > 0 && !pauseScreen.activeSelf)
		{
			StartCoroutine(TimeIEn());
		}
		
		if (pauseScreen.activeSelf) //If game is paused
		{
			pauseButton.interactable = false;
			//Click once to unpause
			if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began ||Input.GetMouseButtonDown(0))
			{
				pauseButton.interactable = true;
				pauseScreen.SetActive(false);
			}
		}
    }
	
	//Setting time as the game score, 1 sec = 1 score
	IEnumerator TimeIEn()
	{
		// Set the text of the timeText object to the current time score, formatted as a string with 0 decimal places
		timeText.text = timeScore.ToString("0"); 
		yield return new WaitForSeconds(0); // Wait for one frame (this is done to allow the text to update before updating the time score)
		timeScore += Time.deltaTime; // Add the time that has passed since the last frame to the time score
	}
	
	//Restart Game
	public void restartGame()
	{
		//player.playerLife = 1;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		
	}
	
	//Game Over
	public void gameOver()
	{
		gameOverScreen.SetActive(true);
	}
	
	//Game Pause
	public void pauseGame()
	{
		if (player.playerLife > 0)
		{
			pauseScreen.SetActive(true);
		}
	}
}
