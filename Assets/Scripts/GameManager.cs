using UnityEngine;
using System.Collections.Generic;

public enum GamePhase
{
	Title,
	Opening,
	Play,
	Ending,
	SetupTitle,
	ShowTitle,
	ShowMenu,
	FadeOutTitle,
	ShowOpeningDlog,
	ShowWalkout,
	ShatterHouse,
	PlaySetup,
	PlayEnd,
	ShowHappyEnd,
	ShowSadEnd,
	ShowCredits,
	EndGame,
	FadeToGame,
	FadeToCredits
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
	public float titleToMenuDelay = 7f;

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

	private TitleSpritesBehaviour _tsb;
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
		DontDestroyOnLoad(_gui);
		_titleFade = ((GameObject)GameObject.Find("TitleFade")).GetComponent<GUITexture>();
		DontDestroyOnLoad(_titleFade);
		_titleFade.pixelInset = new Rect(-Screen.width/2f, -Screen.height/2f, Screen.width, Screen.height);
		GameObject child = (GameObject)GameObject.Find("TitleSprites");
		_tsb = child.GetComponent<TitleSpritesBehaviour>();
		_timer = new SimpleTimer();
		//Camera.main.backgroundColor = new Color(240/255f, 230/255f, 221/255f);
		//_tsb.PlaceSprites();
		//_timer = new SimpleTimer(15f);
		//_timer.StartTimer();
		_fadeOut = false;
		_fadeIn = false;
		_houseSpr = null;
		LastDialogue = false;
		_player = null;
		_stairs = new Transform[0];
		_procdInput = false;
		_items = new Queue<GameObject>();

		_phase = GamePhase.SetupTitle;
	}
	#endregion

	#region methods
	private void Update()
	{
		switch(_phase)
		{
		case GamePhase.SetupTitle:
			if(Application.loadedLevelName != "title" && !Application.isLoadingLevel)
				Application.LoadLevel("title");
			else
			{
				Camera.main.backgroundColor = new Color(240/255f, 230/255f, 221/255f);
				_tsb.PlaceSprites();
				_timer.SetTimer(titleToMenuDelay);
				_timer.StartTimer();
				_phase = GamePhase.ShowTitle;
			}
			break;
		case GamePhase.ShowTitle:
			if(_timer.Expired)
			{
				_phase = GamePhase.ShowMenu;
				_tsb.DropSprites();
			}
			break;
		case GamePhase.ShowMenu:
			if(!_gui.ShowMenu)
				_gui.ShowMenu = true;
			_timer.SetTimer(100f);
			_timer.StartTimer();
			break;
		case GamePhase.FadeToGame:
			if(!_timer.Expired)
				fadeOut();
			else
			{
				if(!Application.isLoadingLevel && Application.loadedLevelName != "house")
					Application.LoadLevel("house");
				if(Application.loadedLevelName == "house" && !Application.isLoadingLevel)
				{
					_timer.SetTimer(3f);
					_timer.StartTimer();
					_tsb.DestroySprites();
					_phase = GamePhase.ShowOpeningDlog;
				}
			}
			break;
		case GamePhase.FadeToCredits:
			break;
		case GamePhase.ShowOpeningDlog:
			if(!_timer.Expired)
				fadeIn ();
			else
			{
				_houseSpr = GameObject.FindGameObjectWithTag("houseBG").GetComponent<SpriteRenderer>();
				//show the breakup
				_gui.EnqueueText("We're through.");
				_gui.EnqueueText("I'm leaving");
				_timer.SetTimer(2.5f); //length of whole breakup, animation + dlog
				_timer.StartTimer();
				_phase = GamePhase.ShatterHouse;
			}
			break;
		case GamePhase.ShatterHouse:
			if(_timer.Expired)
			{
				//animate shattered house
				_timer.SetTimer(2.5f); //length of animation
				_timer.StartTimer();
				_phase = GamePhase.PlaySetup;
			}
			break;
		case GamePhase.PlaySetup:
			if(_timer.Expired)
			{
				if(_player == null)
					_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
				if(_stairs.Length == 0)
					_stairs = GameObject.FindGameObjectWithTag("stairs").GetComponentsInChildren<Transform>();
				if(_items.Count == 0)
				{
					//Debug.Log("Collecting items");
					GameObject[] items = GameObject.FindGameObjectsWithTag("item");
					for(int i=0;i<items.Length;++i)
						items[i].SetActive(false);
					
					//Debug.Log("Shuffling items");
					for(int i=items.Length - 1; i > 0; --i)
					{
						int j = UnityEngine.Random.Range(0, i+1);
						GameObject tmp = items[j];
						items[j] = items[i];
						items[i] = tmp;
					}
					_items = new Queue<GameObject>(items);
					_phase = GamePhase.Play;
				}
			}
			break;
		case GamePhase.Play:
			if(!_procdInput)
				_procdInput = ProcessInput();
			else
				_procdInput = false;
			break;
		case GamePhase.PlayEnd:
			Debug.Log ("Game Over? " + "Score: " + Mathf.Round(PlayerScore).ToString());
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
			if((_player.TopY <= pos.y+heapSpr.bounds.extents.y && _player.BottomY >= pos.y-heapSpr.bounds.extents.y) &&
			   (_player.LeftX >= pos.x-heapSpr.bounds.extents.x && _player.RightX <= pos.x+heapSpr.bounds.extents.x) &&
			   _items.Count > 0 && _player.Carrying == null)
			{
				_player.Carrying = _items.Dequeue();
				_player.Carrying.SetActive(true);
				_gui.DisplayItem(_player.Carrying.GetComponent<SpriteRenderer>());
				_player.Carrying.SetActive(false);

			}
			else if(_player.Carrying != null && _player.transform.position.y > -3f)
			{
				float pX = _player.transform.position.x;
				float pY = _player.transform.position.y;
				float oX = _player.CarriedOrigPos.x;
				float oY = _player.CarriedOrigPos.y;
				int points = 0;
				float maxY = (oY < (1.73f-1.22f)/2) ? -1.22f : 1.73f;
				float maxX = (oX < 0) ? -7.7f : 7.7f;
				float maxSqrMag = ((new Vector3(maxX, maxY, 0f)) - _player.CarriedOrigPos).sqrMagnitude;
				float sqrMag = (_player.transform.position - _player.CarriedOrigPos).sqrMagnitude;

				Debug.Log (maxSqrMag + " "+ sqrMag);
				if(sqrMag < 16f)
					points = 1;
				else
				{
					if(sqrMag >= 16f)
						points++;
					if(sqrMag >= 64f)
						points++;
					if(sqrMag >=144f)
						points++;
				}

				_player.Score+=points;
				//Debug.Log (sqrMag + " is worth " + points + ".  Score is now " + PlayerScore);

				_player.Carrying.SetActive(true);
				_player.Carrying.transform.position = _player.transform.position;
				_player.ClearCarry();
				_gui.DisplayItem();
				if(_items.Count == 0)
					_phase = GamePhase.PlayEnd;
			}

		}

		return (up || down || placePickup);

	}
	#endregion
	
	#region properties
	public GamePhase Phase
	{
		get { return _phase; }
		set { _phase = value; }
	}

	public bool LastDialogue { get; set; }

	public SpriteRenderer CurrentItem
	{
		get
		{
			if(_player == null || _player.Carrying == null)
				return null;
			return _player.Carrying.GetComponent<SpriteRenderer>();
		}
	}

	public SimpleTimer Timer
	{
		get { return _timer; }
		set { _timer = value; }
	}

	public float PlayerScore { get { return (_player != null) ? _player.Score : 0f; } }
	#endregion
}
