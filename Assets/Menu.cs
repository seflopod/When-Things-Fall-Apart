using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	//public GUIStyle style;

	void OnGUI()
	{
		if(GUI.Button(new Rect(0f,Screen.height-60,110,50),"Menu"))
		{
			Application.LoadLevel("title");
		}
	}
}