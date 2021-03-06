﻿using UnityEngine;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour
{
	#region fields
	public GUISkin skin;
	public string dialogueStyleName = "dialogue";
	public string menuStyleName = "menu";
	public float timeToDisplay = 2.5f;
	public Texture bottomBarBG;

	private GUIStyle _dlogStyle;
	private Queue<string> _dialogueQueue;
	private float _dlogTimer;
	private Rect _btmBarRect;
	private Rect _lineRect;
	private string _line;
	private GameObject _itmGO;
	#endregion

	#region monobehaviour
	private void Start()
	{
		_dlogStyle = skin.FindStyle(dialogueStyleName);
		if(_dlogStyle == null)
		{
			Debug.LogError("Could not find style for dialogue box and/or " +
																	"menu.");
		}

		_dialogueQueue = new Queue<string>();
		_dlogTimer = timeToDisplay;

		_btmBarRect = new Rect(0f,
		                       5 / 6f * Screen.height,
		                       Screen.width,
		                       1 / 6f * Screen.height);

		_line = "";
		_itmGO = new GameObject("ItemDisplay", typeof(SpriteRenderer));
		_itmGO.transform.position = new Vector3(-7.04f, -3.5f, 0f);
		_itmGO.GetComponent<SpriteRenderer>().sortingLayerName = "Midground";
		DontDestroyOnLoad(_itmGO);
	}
	
	// Update is called once per frame
	private void Update()
	{
		_dlogTimer += Time.deltaTime;
		if(_dlogTimer == float.MaxValue - 1f)
		{
			_dlogTimer = timeToDisplay;
		}

		if(_dialogueQueue.Count > 0 && _dlogTimer > timeToDisplay)
		{
			_line = _dialogueQueue.Dequeue();
			if(_dialogueQueue.Count == 0)
			{
				GameManager.Instance.LastDialogue = true;
			}
			_dlogTimer = 0f;
		}

	}

	private void OnGUI()
	{
		if(GUI.skin != skin)
		{
			GUI.skin = skin;
		}

		if(_dlogTimer < timeToDisplay)
		{
			displayLine(_line);
		}

	}
	#endregion

	#region methods
	/// <summary>
	/// Displays the text.
	/// </summary>
	/// <param name="text">Text.</param>
	/// <description>
	/// This will automatically break the passed text into lines, so no \n is
	/// needed.  Assumes that a custom style exists that is named "dialogue".
	/// </description>
	public void EnqueueText(string text)
	{
		string[] words = text.Split(new char[] {' ', '\n'});

		string line = "";
		int wordIdx = 0;
		while(wordIdx < words.Length)
		{
			while(wordIdx < words.Length &&
			      _dlogStyle.CalcSize(
					new GUIContent((line + " " + words[wordIdx]))).x <=
			      										_btmBarRect.width*2/3f)
			{
				line += " " + words[wordIdx++];
			}
			_dialogueQueue.Enqueue(line);
			line = "";
		}
		GameManager.Instance.LastDialogue = false;
	}

	private void displayLine(string line)
	{
		float linePx = _dlogStyle.CalcSize(new GUIContent(line)).x;
		float hMargin = (Screen.width - linePx) / 2;
		float vMargin = Screen.height / 24;
		Rect lineRect = new Rect(hMargin,
		                         _btmBarRect.y + vMargin,
		                         linePx,
		                         _btmBarRect.height - vMargin);
		GUI.DrawTexture(_btmBarRect, bottomBarBG);
		GUI.Label(lineRect, line, _dlogStyle);
	}

	public void DisplayItem(SpriteRenderer itmSpr)
	{
		_itmGO.GetComponent<SpriteRenderer>().sprite = itmSpr.sprite;
		_itmGO.GetComponent<SpriteRenderer>().color = itmSpr.color;
	}

	public void DisplayItem()
	{
		_itmGO.GetComponent<SpriteRenderer>().sprite = null;
	}
	#endregion
}