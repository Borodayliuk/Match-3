using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Holoville.HOTween;
using UnityEngine.SceneManagement;

/// <summary>
///  This class is the MainMenu class that contain the logic of the menu
/// Author : Pondomaniac Games
/// </summary>
public class MainMenu : MonoBehaviour {
	public GameObject _Logo;//The animated logos 
	public GameObject _PlayButton;//The play button
	public GameObject _BestScore;//The bestscore text
	
	
    // Use this for initialization
	public string _NextScene;//The  nextScene to navigate to 
	public AudioClip MenuSound;//The  menu sound when the user clicka buton
	public EaseType AnimationTypeOfPanels;//The animation effect used on the panel
	public float AnimationDurationOfPanels;//The animation duration time



	
	
	

		//This method is called after the init 
		void Start()
		{

			
			AnimateLogo();

			(_BestScore.GetComponent(typeof(TextMesh)) as TextMesh).text = "" + PlayerPrefs.GetInt("HighScore");
			

		}




		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
			//Detecting if the player clicked on the left mouse button and also if there is no animation playing
			if (Input.GetButtonDown("Fire1"))
			{

				//The 3 following lines is to get the clicked GameObject and getting the RaycastHit2D that will help us know the clicked object
				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if (hit.transform != null)
				{
					if ((hit.transform.gameObject.name == _PlayButton.name)) { GetComponent<AudioSource>().PlayOneShot(MenuSound);  SceneManager.LoadScene(_NextScene, LoadSceneMode.Single);
				 }
					

				}
			}
		}


		// Animation of the logo 
		void AnimateLogo()
		{
			Sequence mySequence = new Sequence();
			TweenParms parms;

			Color oldColor = _Logo.GetComponent<Renderer>().material.color;
			parms = new TweenParms().Prop("color", new Color(oldColor.r, oldColor.b, oldColor.g, 0.4f)).Ease(EaseType.EaseInQuart);

			parms = new TweenParms().Prop("localScale", new Vector3(1.1f, 1.1f, -2)).Ease(EaseType.EaseOutElastic);
			mySequence.Append(HOTween.To(_Logo.transform, 6f, parms));


			parms = new TweenParms().Prop("localScale", new Vector3(0.9f, 0.9f, -2)).Ease(EaseType.EaseOutElastic);
			mySequence.Append(HOTween.To(_Logo.transform, 5f, parms));

			mySequence.Play();
		}

	}
