using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoville.HOTween;

/// <summary>
///  This class contains all static method used in different part of the project or that may be usefull for you
/// Author : Pondomaniac Games
/// </summary>
public class Util : ScriptableObject
{
	//Getting the picture using the facebook GRAPH api
    public static string GetPictureURL(string facebookID, int? width = null, int? height = null, string type = null)
    {
        string url = string.Format("/{0}/picture", facebookID);
       
        return url;
    }



	//Getting a random friend list
    public static Dictionary<string, string> RandomFriend(List<object> friends)
    {
        var fd = ((Dictionary<string, object>)(friends[Random.Range(0, friends.Count - 1)]));
        var friend = new Dictionary<string, string>();
        friend["id"] = (string)fd["id"];
        friend["first_name"] = (string)fd["first_name"];
        return friend;
    }

	

    
	//Draw the texture picture  
    public static void DrawActualSizeTexture (Vector2 pos, Texture texture, float scale = 1.0f)
    {
        Rect rect = new Rect (pos.x, pos.y, texture.width * scale , texture.height * scale);
        GUI.DrawTexture(rect, texture);
    }
	//Draw a text
    public static void DrawSimpleText (Vector2 pos, GUIStyle style, string text)
    {
        Rect rect = new Rect (pos.x, pos.y, Screen.width, Screen.height);
        GUI.Label (rect, text, style);
    }
	//The animation when you press a button
	public static void ButtonPressAnimation(GameObject go) {
				Sequence mySequence = new Sequence (new SequenceParms ());
				TweenParms parms;
		
		
				parms = new TweenParms ().Prop ("localScale", new Vector3 (0.7f, 0.7f, go.transform.position.z)).Ease (EaseType.EaseInQuad);
				mySequence.Append (HOTween.To (go.transform, 0.12f, parms));
		
		
				parms = new TweenParms ().Prop ("localScale", new Vector3 (1f, 1f, go.transform.position.z)).Ease (EaseType.EaseInQuad);
				mySequence.Append (HOTween.To (go.transform, 0.12f, parms));


				mySequence.Play ();


				mySequence = new Sequence (new SequenceParms ());


	}


}
