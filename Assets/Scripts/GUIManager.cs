using UnityEngine;
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
	private GUIStyle _menuStyle;
	private Queue<string> _dialogueQueue;
	private float _dlogTimer;
	private Rect _btmBarRect;
	private Rect _lineRect;
	private string _line;
	private GameObject _itmGO;
	private Rect _menuRect;
	#endregion

	#region monobehaviour
	// Use this for initialization
	private void Start ()
	{
		_dlogStyle = skin.FindStyle(dialogueStyleName);
		_menuStyle = skin.FindStyle(menuStyleName);
		if(_dlogStyle == null)
			Debug.LogError("Could not find style for dialogue box and/or menu.");

		_dialogueQueue = new Queue<string>();
		_dlogTimer = timeToDisplay;

		_btmBarRect = new Rect(0f, 5/6f*Screen.height, Screen.width, 1/6f*Screen.height);

		_line = "";
		_itmGO = new GameObject("ItemDisplay", typeof(SpriteRenderer));
		_itmGO.transform.position = new Vector3(-7.04f, -3.5f, 0f);
		_itmGO.GetComponent<SpriteRenderer>().sortingLayerName = "Midground";
		DontDestroyOnLoad(_itmGO);
		ShowMenu = false;
		_menuRect = new Rect(871/1600f*Screen.width, 609/900f*Screen.height, 295/1600f*Screen.width, 200f/900f*Screen.height);
	}
	
	// Update is called once per frame
	private void Update ()
	{
		_dlogTimer+=Time.deltaTime;
		if(_dlogTimer == float.MaxValue - 1f)
			_dlogTimer = timeToDisplay;

		if(_dialogueQueue.Count > 0 && _dlogTimer > timeToDisplay)
		{
			_line = _dialogueQueue.Dequeue();
			if(_dialogueQueue.Count == 0)
				GameManager.Instance.LastDialogue = true;
			_dlogTimer = 0f;
		}
	}

	private void OnGUI()
	{
		if(GUI.skin != skin)
			GUI.skin = skin;

		if(_dlogTimer < timeToDisplay)
			displayLine(_line);

		if(ShowMenu)
			displayMenu();

	}
	#endregion

	#region methods
	/// <summary>
	/// Displays the text.
	/// </summary>
	/// <param name="text">Text.</param>
	/// <description>
	/// This will automatically break the passed text into lines, so no \n is needed.
	/// Assumes that a custom style exists that is named "dialogue".
	/// </description>
	public void EnqueueText(string text)
	{
		string[] words = text.Split(new char[] {' ', '\n'});

		string line = "";
		int wordIdx = 0;
		while(wordIdx < words.Length)
		{
			while(wordIdx < words.Length && _dlogStyle.CalcSize(new GUIContent((line+ " " + words[wordIdx]))).x <= _btmBarRect.width*2/3f)
			{
				line+=" " + words[wordIdx++];
			}
			_dialogueQueue.Enqueue(line);
			line = "";
		}
		GameManager.Instance.LastDialogue = false;
	}

	private void displayLine(string line)
	{
		float linePx = _dlogStyle.CalcSize(new GUIContent(line)).x;
		float hMargin = (Screen.width - linePx)/2;
		float vMargin = Screen.height/24;
		Rect lineRect = new Rect(hMargin, _btmBarRect.y+vMargin, linePx, _btmBarRect.height-vMargin);
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

	private void displayMenu()
	{
		float btnW = Mathf.Min(120.5625f, 0.1875f * Screen.width);
		float btnH = Mathf.Min(16.74f, 11/300f * Screen.height);
		float btnX = 0.54375f * Screen.width;
		bool play = GUI.Button(new Rect(btnX, 2/3f*Screen.height, btnW, btnH), "Play", _menuStyle);
		bool credits = GUI.Button(new Rect(btnX, 2/3f*Screen.height + 1.5f * btnH, btnW, btnH), "Credits", _menuStyle);
		bool quit = GUI.Button(new Rect(btnX, 2/3f*Screen.height + 3f * btnH, btnW, btnH), "Quit", _menuStyle);
		ShowMenu = !(play || credits || quit);
		if(play)
		{
			GameManager.Instance.Phase = GamePhase.FadeToGame;
			GameManager.Instance.Timer.SetTimer(3f);
			GameManager.Instance.Timer.StartTimer();
		}
		if(credits)
		{
			GameManager.Instance.Phase = GamePhase.FadeToCredits;
			GameManager.Instance.Timer.SetTimer(3f);
			GameManager.Instance.Timer.StartTimer();
		}
		if(quit)
			GameManager.Instance.Phase = GamePhase.EndGame;
	}
	#endregion

	#region properties
	public bool ShowMenu { get; set; }
	#endregion
}