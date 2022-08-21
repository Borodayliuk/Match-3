using UnityEngine;
using System.Collections;
using System;
using Holoville.HOTween;
using UnityEngine.SceneManagement;

/// <summary>
///  This class is the main entry point of the game it should be attached to a gameobject and be instanciate in the scene
/// Author : Pondomaniac Games
/// </summary>
public class MenuManager : MonoBehaviour
{
    


		public GameObject _Tutorial;//Tutorial GameObject
		public int _scoreIncrement;//The amount of point to increment each time we score
		float progress = 0;//The progress bar progress 
		public float _timerCoef  ;//The progress bar speed 
	    public float _timerCountDownInMinutes;//The progress bar speed 
        public GameObject _Time;//The timer
		public GameObject _PauseButton;//The pause button we use in the scene
		public GameObject _ReloadButton;//The reload button we use in the scene
		public GameObject _PlayButton;//The play button we use in the scene
		public GameObject _MenuButton;//The menu button we use in the scene
		public GameObject _PausedBackground;//The pause background
		float timing = 0;//The local timer
		public GameObject _TimeIsUp;//The object indicating if the time is up
		public GameObject _CurrentScoreBoard;//The current score in the scene
	    public GameObject _CurrentLevelBoard;//The current level in the scene
		public GameObject _ScoreBoardPanel;//The score board when the game has ended or when the time is up
		public GameObject _FaceBookButton;//The facebook button to share the score with friends
		public GameObject _ScoreTextValue;//Level reached
		public GameObject _LevelTextValue;//Level reached text value
	    public GameObject _TimerTextValue;//Level reached text value
	    public GameObject _CountDown;//The CountDown object that is not used directly that we use to replicate the style when displaying countdown
		public GameObject _TestScore;//The testscore button used to test the score and level increment 
		public GameObject _TestLevelEnd;//The testscore button used to test the rnd of the level
		public AudioClip PowerSound;//The sound heared when we level up
		public AudioClip MenuSound;//The sound heared when we click on a button
		int   level = 0;
	    public AudioClip LevelUpSound;
		public AudioClip TimeUpSound;
		public AudioClip BestScoreSound;
		public AudioClip EndSound;
		public AudioClip CountDownSound;
	private Vector3 center;

		// Use this for initialization
	void Start ()
		{
	
		       progress = (float)(timing * _timerCoef);
				//_Timer new Rect(pos.x, pos.y, size.x, size.y), progressBarEmpty);
				
				StartCoroutine (Init ());
		}

		// Update is called once per frame
		void Update ()
		{
				if (Input.GetKeyDown (KeyCode.Escape)) {
						Application.Quit ();
				}

	
	
				//Detecting if the player clicked on the left mouse button and also if there is no animation playing
				if (Input.GetButtonDown ("Fire1")) {

						//The 3 following lines is to get the clicked GameObject and getting the RaycastHit2D that will help us know the clicked object
						RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);

						if (hit.transform != null) {

										Debug.Log(hit.transform.gameObject.name);
										if (hit.transform.gameObject.name == _MenuButton.name) {
										GetComponent<AudioSource>().PlayOneShot (MenuSound);
										hit.transform.localScale = new Vector3 (1.1f, 1.1f, 0);
					SceneManager.LoadScene("MainMenu");
								}
								if (hit.transform.gameObject.name == _FaceBookButton.name) {
										hit.transform.localScale = new Vector3 (1.5f, 1.5f, 0);
										onBragClicked ();
								}
								if (hit.transform.gameObject.name == _ReloadButton.name) {
										GetComponent<AudioSource>().PlayOneShot (MenuSound);
										Time.timeScale = 1;
					                    GameStateManager.IsGamePaused = false;
					                    HOTween.Play ();
										hit.transform.localScale = new Vector3 (1.1f, 1.1f, -1);
										GameStateManager.initValues();
										Application.LoadLevel (Application.loadedLevelName);
								}
								if (hit.transform.gameObject.name == _PauseButton.name && !GameStateManager.IsGamePaused && !GameStateManager.IsGameOver && !GameStateManager.IsGameOver && HOTween.GetTweenersByTarget (_PlayButton.transform, false).Count == 0 && HOTween.GetTweenersByTarget (_MenuButton.transform, false).Count == 0) {
										GetComponent<AudioSource>().PlayOneShot (MenuSound);
					                	GameStateManager.IsGamePaused = true;
					                	HOTween.Pause();
					                 	StartCoroutine (ShowMenu ());
										hit.transform.localScale = new Vector3 (4f, 4f, 1f);
								} else if ((hit.transform.gameObject.name == _PauseButton.name || hit.transform.gameObject.name == _PlayButton.name) && !GameStateManager.IsGameOver && !GameStateManager.IsGameOver && GameStateManager.IsGamePaused  && HOTween.GetTweenersByTarget (_PlayButton.transform, false).Count == 0 && HOTween.GetTweenersByTarget (_MenuButton.transform, false).Count == 0) {
										GetComponent<AudioSource>().PlayOneShot (MenuSound);
										StartCoroutine (HideMenu ());
			                     		if (hit.transform.gameObject.name == _PlayButton.name) hit.transform.localScale = new Vector3(2f, 1f, 1f);
					                    else     hit.transform.localScale = new Vector3(3.5f, 3.4f, 1f);
			                 	} 
				
						}
				}
				if (GameStateManager.IsGamePaused)
						return;

				if (!GameStateManager.IsGamePaused) {
						timing += 0.001f;
						progress = (float)(timing * _timerCoef);
						//_Time.transform.localScale = new Vector3 (Mathf.Clamp01 (progress), _Time.transform.localScale.y, 0);
	                  //  (_TimerTextValue.GetComponent(typeof(TextMesh)) as TextMesh).text = progress.ToString();

		}
              if (GameStateManager.IsTimesUp) {
			GameStateManager.IsGameOver = true;
			            GameStateManager.IsGamePaused = true;
		            	AnimateBigSmall(_CountDown, new Vector3(center.x + 3f, 10f, -5), "Time is up!", true);
                        StartCoroutine(ShowBoardScore ());

				} else if (GameStateManager.IsGameOver)
		{
			GameStateManager.IsGameOver = true;
			GameStateManager.IsGamePaused = true;
			AnimateBigSmall(_CountDown, new Vector3(center.x + 3f, 10f, -5), "Game Over!",true) ;
			StartCoroutine(ShowBoardScore());

		}
				//Update the score
				(_ScoreTextValue.GetComponent (typeof(TextMesh))as TextMesh).text = GameStateManager.Score.ToString ();
				UpdateLevel ();
			/*	if (PlayerPrefs.GetInt ("HighScore") < _scoreTotal && !_BestScoreReached) {
						_BestScoreReached = true;
				}
				if (PlayerPrefs.GetInt ("HighLevel") < level && !_BestLevelReached) {
						_BestLevelReached = true;
				}*/
				


	

		}
	//Update the Level
		void UpdateLevel ()
		{ 
			            level =(int) (0.1 * Math.Sqrt(GameStateManager.Score)) +1;
						Debug.Log ("level " + level);
						Debug.Log ("GameStateManager.Score " + GameStateManager.Score);
						(_LevelTextValue.GetComponent (typeof(TextMesh))as TextMesh).text = level.ToString ();
						GameStateManager.Level  = level;

		}
	//Show the bestscore board
		IEnumerator ShowBoardScore ()
		{
				GetComponent<AudioSource>().Stop ();
				GetComponent<AudioSource>().PlayOneShot (TimeUpSound);
				yield return new WaitForSeconds (0.5f);
				GetComponent<AudioSource>().PlayOneShot (EndSound);
				(_CurrentScoreBoard.GetComponent (typeof(TextMesh))as TextMesh).text = "" + GameStateManager.Score;
				(_CurrentLevelBoard.GetComponent (typeof(TextMesh))as TextMesh).text = "" + GameStateManager.Level;
				SetScore (GameStateManager.Score);
				yield return new WaitForSeconds (1);
				TweenParms parms = new TweenParms ().Prop ("position", new Vector3 (_ScoreBoardPanel.transform.position.x, 7f, _ScoreBoardPanel.transform.position.z)).Ease (EaseType.EaseOutQuart);
				HOTween.To (_ScoreBoardPanel.transform, 0.5f, parms);
				//_MenuButton.transform.position = new Vector3 (4, _MenuButton.transform.position.y, _MenuButton.transform.position.z);
				parms = new TweenParms ().Prop ("position", new Vector3 (_MenuButton.transform.position.x, 1.5f, -8)).Ease (EaseType.EaseOutQuart);
				HOTween.To (_MenuButton.transform, 0.7f, parms).WaitForCompletion ();
				parms = new TweenParms ().Prop ("position", new Vector3 (_ReloadButton.transform.position.x, 2.5f, -8)).Ease (EaseType.EaseOutQuart);
				HOTween.To (_ReloadButton.transform, 0.9f, parms).WaitForCompletion ();
		}
	//Update the pause menu
		IEnumerator ShowMenu ()
		{
		GameStateManager.IsGamePaused = true;
		HOTween.Pause ();
				GetComponent<AudioSource>().Pause ();

				TweenParms parms = new TweenParms ().Prop ("position", new Vector3 (_PausedBackground.transform.position.x, 10f, -4)).Ease (EaseType.EaseOutQuart);
				HOTween.To (_PausedBackground.transform, 0.2f, parms).WaitForCompletion ();

				parms = new TweenParms ().Prop ("position", new Vector3 (_PlayButton.transform.position.x, 10f, -5)).Ease (EaseType.EaseOutQuart);
				HOTween.To (_PlayButton.transform, 0.4f, parms).WaitForCompletion ();

				parms = new TweenParms ().Prop ("position", new Vector3 (_ReloadButton.transform.position.x, 9f, -5)).Ease (EaseType.EaseOutQuart);
				HOTween.To (_ReloadButton.transform, 0.5f, parms).WaitForCompletion ();

				parms = new TweenParms ().Prop ("position", new Vector3 (_MenuButton.transform.position.x, 8f, -5)).Ease (EaseType.EaseOutQuart);
				yield return StartCoroutine (HOTween.To (_MenuButton.transform, 0.6f, parms).WaitForCompletion ());


				Time.timeScale = 0;


		}
	//Hide the pause menu
		IEnumerator HideMenu ()
		{
				Time.timeScale = 1;
		GameStateManager.IsGamePaused = false;
		HOTween.Play ();

				TweenParms parms = new TweenParms ().Prop ("position", new Vector3 (_PausedBackground.transform.position.x, 30, -4)).Ease (EaseType.EaseOutQuart);
				HOTween.To (_PausedBackground.transform, 0.6f, parms).WaitForCompletion ();

				parms = new TweenParms ().Prop ("position", new Vector3 (_PlayButton.transform.position.x, 30, -5)).Ease (EaseType.EaseOutQuart);
				HOTween.To (_PlayButton.transform, 0.4f, parms).WaitForCompletion ();
				GetComponent<AudioSource>().Play ();

				parms = new TweenParms ().Prop ("position", new Vector3 (_ReloadButton.transform.position.x, 30, -5)).Ease (EaseType.EaseOutQuart);
				HOTween.To (_ReloadButton.transform, 0.5f, parms).WaitForCompletion ();


				parms = new TweenParms ().Prop ("position", new Vector3 (_MenuButton.transform.position.x, 30, -5)).Ease (EaseType.EaseOutQuart);
				yield return StartCoroutine (HOTween.To (_MenuButton.transform, 0.2f, parms).WaitForCompletion ());

		}


	//Where facebook button is clicked
		private void onBragClicked ()
		{
				
		}

	
	
		void OnLoggedIn ()
		{                                                                                          
				                                                                         
				onBragClicked ();
		
		}
	
		private void SetInit ()
		{                                                                                            
			                                                                
				enabled = true; // "enabled" is a property inherited from MonoBehaviour                  
			                                                                                  
		}
	
		private void OnHideUnity (bool isGameShown)
		{                                                                                            
				if (!isGameShown) {                                                                                        
						// pause the game - we will need to hide                                             
						Time.timeScale = 0;                                                                  
				} else {                                                                                        
						// start the game back up - we're getting focus again                                
						Time.timeScale = 1;                                                                  
				}                                                                                        
		}

	//Setting the score in the player preferences
		void SetScore (int _scoreTotal)
		{
				PlayerPrefs.SetInt ("LastScore", _scoreTotal);
				if (PlayerPrefs.GetInt ("HighScore") < _scoreTotal) {

						PlayerPrefs.SetInt ("HighScore", _scoreTotal);
						GetComponent<AudioSource>().PlayOneShot (BestScoreSound);
				}
				if (PlayerPrefs.GetInt ("HighLevel") < _scoreTotal) {
						//PlayerPrefs.SetInt ("OldHighLevel",PlayerPrefs.GetInt ("HighLevel"));
						PlayerPrefs.SetInt ("HighLevel", int.Parse ((_LevelTextValue.GetComponent (typeof(TextMesh))as TextMesh).text));

				}
		}

	//Show a message in the screen 
			IEnumerator Init ()
		{
		GameStateManager.isCountingDown = true;
		GameStateManager.IsGamePaused = true;
				 center = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
				//PlayerPrefs.SetInt ("Tutorial", 0);
			//	if (PlayerPrefs.GetInt ("Tutorial") != 1) {
						var isOkay = false;
                                        TweenParms parms = new TweenParms ().Prop ("localPosition", new Vector3 (5, -10, -10)).Ease (EaseType.EaseOutQuart);
										HOTween.To (_Tutorial.transform, 1f, parms);

						while (isOkay==false) { 
								if (Input.GetButtonDown ("Fire1")) {
										isOkay = true; 
										 parms = new TweenParms ().Prop ("localPosition", new Vector3 (5, 8, -10)).Ease (EaseType.EaseOutQuart);
										HOTween.To (_Tutorial.transform, 1f, parms);
										PlayerPrefs.SetInt ("Tutorial", 1);
			     	
								}
								yield return 0;	
						}
		
			/*	} else {
						_Tutorial.transform.localPosition = new Vector3 (100, 0, -10);

				}*/
	//Count down from 3,2,1 Go!
				AnimateBigSmall (_CountDown, new Vector3 (center.x+ 3f, 10f, -5), "3");
				GetComponent<AudioSource>().PlayOneShot (CountDownSound);
				yield return new WaitForSeconds (0.7f);
				AnimateBigSmall (_CountDown, new Vector3 (center.x+ 3f, 10f, -5), "2");
				GetComponent<AudioSource>().PlayOneShot (CountDownSound);
				yield return new WaitForSeconds (0.7f);
				AnimateBigSmall (_CountDown, new Vector3 (center.x+ 3f, 10f, -5), "1");
				GetComponent<AudioSource>().PlayOneShot (CountDownSound);
				yield return new WaitForSeconds (0.7f);
				AnimateBigSmall (_CountDown, new Vector3 (center.x+3f, 10f, -5), "Go!");
				GetComponent<AudioSource>().PlayOneShot (CountDownSound);
				yield return new WaitForSeconds (0.1f);
Debug.Log ("test  ");
			
		GameStateManager.isCountingDown = false;
		GameStateManager.IsGamePaused = false;
		GameStateManager.IsGameOver = false;
		}

	//Gameobject animation from big to small when showing a message
		void AnimateBigSmall (GameObject go, Vector3 position, string s, bool StayOnScreen = false )
		{ 
				var destroyingParticle = GameObject.Instantiate (go as GameObject, position, transform.rotation) as GameObject;
				(destroyingParticle.GetComponent (typeof(TextMesh))as TextMesh).text = s;

		TweenParms parms2 = new TweenParms ().Prop ("fontSize", 200).Ease (EaseType.EaseOutQuart);
				HOTween.To ((destroyingParticle.GetComponent (typeof(TextMesh))as TextMesh), 0.5f, parms2);
		if (!StayOnScreen)
		{
			Color oldColor2 = destroyingParticle.GetComponent<Renderer>().material.color;
			 parms2 = new TweenParms().Prop("color", new Color(oldColor2.r, oldColor2.b, oldColor2.g, 0f)).Ease(EaseType.EaseOutQuart);
			HOTween.To((destroyingParticle.GetComponent(typeof(TextMesh)) as TextMesh), 0.7f, parms2);
			Destroy(destroyingParticle, 4);
		}
		
		}

	//Update the reached level in the player preferences
		void UpdateReachedLevel ()
		{
				
		
			
		}
}

