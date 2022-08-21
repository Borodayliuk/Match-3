using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Holoville.HOTween;
using UnityEngine.SceneManagement;

/// <summary>
///  This class is the redirect class it should be attached to a button in the levels scene
/// Author : Pondomaniac Games
/// </summary>
public class ButtonRedirect : MonoBehaviour
{

		public string _redirectedScene;	//The name of the scene we want to redirect to 
		public AudioClip MenuSound; //The sound of the menu clicks
		private bool ShouldTransit = false;//A transition flag
	  //Called before init
		void Awake ()
		{
				Time.timeScale = 1; 
				HOTween.Kill ();
	
				
		}

	//This method is called after the init 
		void Start ()
		{
		       		
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (HOTween.GetAllTweens ().Count == 0 && ShouldTransit) {
						if (_redirectedScene != string.Empty) {	
								SceneManager.LoadScene (_redirectedScene);
						}
				}
				if (Input.GetKeyDown (KeyCode.Escape)) {
						Application.Quit ();
				}
				//Detecting if the player clicked on the left mouse button and also if there is no animation playing
				if (Input.GetButtonDown ("Fire1")) {

						//The 3 following lines is to get the clicked GameObject and getting the RaycastHit2D that will help us know the clicked object
						RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
						if (hit.transform != null) {
								if ((hit.transform.gameObject.name == this.name)) {
										GetComponent<AudioSource>().PlayOneShot (MenuSound);
										Util.ButtonPressAnimation (hit.transform.gameObject);
										ShouldTransit = true;
										Time.timeScale = 1;
								}
		
						}
				}
		}

		}