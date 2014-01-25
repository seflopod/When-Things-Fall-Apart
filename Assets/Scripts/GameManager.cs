using UnityEngine;
using System.Collections.Generic;

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
	private PlayerBehaviour _player;
	private Transform[] _stairs;
	private bool _procdInput;
	private Queue<GameObject> _items;
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
		_player = null;
		_stairs = new Transform[0];
		_procdInput = false;
		_items = new Queue<GameObject>();
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
				_houseSpr = GameObject.FindGameObjectWithTag("houseBG").GetComponent<SpriteRenderer>();
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
				//Debug.Log("Collecting items");
				GameObject[] items = GameObject.FindGameObjectsWithTag("item");
				for(int i=0;i<items.Length;++i)
					items[i].SetActive(false);
				//Debug.Log("Shuffling items");
				for(int i=items.Length - 1;i >= 1; ++i)
				{
					int j = Random.Range(0, i+1);
					GameObject tmp = items[j];
					items[j] = items[i];
					items[i] = items[j];
				}
				_items = new Queue<GameObject>(items);
				//Debug.Log(_items.Count);
				//reserved for shatter the house
				_phase = GamePhase.Play;
				Debug.Log("Playing");
			}
			break;
		case GamePhase.Play:
			//allow play once shattering is done.
			if(_timer.Expired)
			{
				if(_player == null)
					_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
				if(_stairs.Length == 0)
					_stairs = GameObject.FindGameObjectWithTag("stairs").GetComponentsInChildren<Transform>();
				if(!_procdInput)
					_procdInput = ProcessInput();
				else
					_procdInput = false;
			}
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

	private bool ProcessInput()
	{
		float dt = Time.deltaTime;
		float hAxis = Input.GetAxis("Horizontal");
		bool down = Input.GetKeyDown(KeyCode.DownArrow);
		bool up = Input.GetKeyDown(KeyCode.UpArrow);
		bool placePickup = Input.GetButtonDown("Fire1");
		if(hAxis < 0 && _player.LeftX - dt * _player.speed > -_houseSpr.bounds.extents.x)
			_player.transform.Translate(dt * -_player.speed, 0f, 0f);
		if(hAxis > 0 && _player.RightX + dt * _player.speed < _houseSpr.bounds.extents.x)
			_player.transform.Translate(dt * _player.speed, 0f, 0f);

		if(up || down)
		{
			for(int i=0;i<_stairs.Length;++i)
			{
				Vector3 pos = _stairs[i].position;
				if((_player.TopY >= pos.y && _player.BottomY <= pos.y) &&
				   (_player.LeftX <= pos.x && _player.RightX >= pos.x))
				{
					if(up && _stairs[i].gameObject.CompareTag("basement"))
					{
						_player.transform.position = GameObject.FindGameObjectWithTag("firstFloor").transform.position;
						break;
					}
					else if(up && _stairs[i].gameObject.CompareTag("firstFloor"))
					{
						_player.transform.position = GameObject.FindGameObjectWithTag("secondFloor").transform.position;
						break;
					}
					else if(down && _stairs[i].gameObject.CompareTag("firstFloor"))
					{
						_player.transform.position = GameObject.FindGameObjectWithTag("basement").transform.position;
						break;
					}
					else if(down && _stairs[i].gameObject.CompareTag("secondFloor"))
					{
						_player.transform.position = GameObject.FindGameObjectWithTag("firstFloor").transform.position;
						break;
					}
				}
			}
		}

		if(placePickup)
		{
			//is this a valid pickup?
			GameObject heap = GameObject.FindGameObjectWithTag("objectHeap");
			Vector3 pos = heap.transform.position;
			SpriteRenderer heapSpr = heap.GetComponent<SpriteRenderer>();
			Debug.Log("Checking for pickup...");
			Debug.Log(_player.TopY + " " + (pos.y+heapSpr.bounds.extents.y));
			Debug.Log(_player.BottomY + " " + (pos.y-heapSpr.bounds.extents.y));
			Debug.Log(_player.LeftX + " " + (pos.x-heapSpr.bounds.extents.x));
			Debug.Log(_player.RightX + " " + (pos.x+heapSpr.bounds.extents.x));
			if((_player.TopY <= pos.y+heapSpr.bounds.extents.y && _player.BottomY >= pos.y-heapSpr.bounds.extents.y) &&
			   (_player.LeftX >= pos.x-heapSpr.bounds.extents.x && _player.RightX <= pos.x+heapSpr.bounds.extents.x) &&
			   _items.Count > 0 && _player.Carrying != null)
			{
				Debug.Log("Picking up item");
				_player.Carrying = _items.Dequeue();
				Debug.Log(_player.Carrying.name);
			}
			else if(_player.Carrying != null && _player.transform.position.y > 598f)
			{
				_player.Score+=(_player.transform.position - _player.CarriedOrigPos).sqrMagnitude;
				_player.Carrying.SetActive(true);
				_player.Carrying.transform.position = _player.transform.position;
				_player.ClearCarry();
				if(_items.Count == 0)
					Debug.Log ("Game Over? " + "Score: " + Mathf.Round(_player.Score).ToString());
			}

		}

		return (up || down || placePickup);

	}
	#endregion
	
	#region properties
	public GamePhase Phase { get { return _phase; } }
	public bool LastDialogue { get; set; }
	#endregion
}
