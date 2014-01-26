using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	public GUIStyle style;
	void OnGUI(){
		if(GUI.Button( new Rect(Screen.width/6,Screen.height/2-120,150,50),"MAIN MENU",style)){
			Application.LoadLevel("title");
		}
		if(GUI.Button( new Rect(Screen.width/6,Screen.height/2-60,150,50),"Credits",style)){
			//Application.LoadLevel(2);
		}
		if(GUI.Button( new Rect(Screen.width/6,Screen.height/2,150,50),"EXIT",style)){
			Application.Quit();
		}
	}
}