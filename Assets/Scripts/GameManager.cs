using UnityEngine;
using System.Collections;

public enum GamePhase
{
	Title,
	Opening,
	Play,
	Ending
};

[RequireComponent(typeof(GUIManager))]
public class GameManager : MonoBehaviour
{
	#region statics
	private static GameManager _instance = null;

	public static GameManager Instance
	{
		get
		{
			if(_instance == null)
				Debug.LogError ("I have no idea how this happened.  There is no game manager.");

			return _instance;
		}
	}
	#endregion

	#region fields
	private GamePhase _phase;
	private GUIManager _gui;
	private SimpleTimer _timer;
	private bool _fadeOut;
	private bool _fadeIn;
	private GUITexture _titleFade;
	private SpriteRenderer _houseSpr;
	#endregion

	#region monobehaviour
	private void Awake()
	{
		if(_instance != null)
		{
			Debug.LogError ("A game manager already exists.  Deleting one of them");
			GameObject.DestroyImmediate(gameObject);
		}
		else
		{
			_instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	private void Start()
	{
		_gui = gameObject.GetComponent<GUIManager>();
		_titleFade = ((GameObject)GameObject.Find("TitleFade")).GetComponent<GUITexture>();
		DontDestroyOnLoad(_titleFade);
		_titleFade.pixelInset = new Rect(-Screen.width/2f, -Screen.height/2f, Screen.width, Screen.height);
		_phase = GamePhase.Title;
		GameObject child = (GameObject)GameObject.Find("TitleSprites");
		TitleSpritesBehaviour tsb = child.GetComponent<TitleSpritesBehaviour>();
		Camera.main.backgroundColor = new Color(240/255f, 230/255f, 221/255f);
		tsb.PlaceSprites();
		_timer = new SimpleTimer(15f);
		_timer.StartTimer();
		_fadeOut = false;
		_fadeIn = false;
		_houseSpr = null;
		LastDialogue = false;
	}
	#endregion

	#region methods
	private void Update()
	{
		switch(_phase)
		{
		case GamePhase.Title:
			if(_timer.Expired && !_fadeOut)
			{
				_fadeOut = true;
				_timer.SetTimer(2.5f);
				_timer.StartTimer();
			}
			if(!_timer.Expired && _fadeOut && !_fadeIn)
			{
				fadeOut();
			}
			if(_timer.Expired && _fadeOut && !_fadeIn)
			{
				Debug.Log("Moving to opening");
				_fadeIn = true;
				_fadeOut = false;
				_phase = GamePhase.Opening;
				((GameObject)GameObject.Find("TitleSprites")).SetActive(false);
				Application.LoadLevel("house");
				_timer.SetTimer(2.5f);
				_timer.StartTimer();
			}
			break;
		case GamePhase.Opening:
			if(Application.loadedLevelName.Equals("house") && !_timer.Expired && _fadeIn && !_fadeOut)
			{
				fadeIn();
			}
			if(_timer.Expired && _fadeIn && !_fadeOut)
			{
				_fadeIn = false;
				//show the breakup
				_gui.EnqueueText("We're through.");
				_gui.EnqueueText("I'm leaving");
			}
			if(LastDialogue && _timer.Expired)
			{
				_timer.SetTimer(5.0f);
				_timer.StartTimer();
			}
			if(LastDialogue && !_timer.Expired)
			{
				//shatter the house
				_phase = GamePhase.Play;
				Debug.Log("Playing");
			}
			break;
		case GamePhase.Play:
			//allow play once shattering is done.
			break;
		case GamePhase.Ending:
			break;
		default:
			break;
		}
	}
	#endregion

	#region methods
	private void fadeIn()
	{
		Color c = _titleFade.color;
		c.a = _timer.TimeRemaining / _timer.Length;
		_titleFade.color = c;

	}

	private void fadeOut()
	{
		Color c = _titleFade.color;
		c.a = 1 - _timer.TimeRemaining / _timer.Length;
		_titleFade.color = c;

	}
	#endregion
	#region properties
	public GamePhase Phase { get { return _phase; } }
	public bool LastDialogue { get; set; }
	#endregion
}
