//Peter
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
	public float titleToMenuDelay = 3f;
	public Sprite heapSprite;
	//public Sprite afterBreakupGlass;
	//public Sprite emptyHouse;
	private GamePhase _phase;
	private GUIManager _gui;
	private SimpleTimer _timer;
	private GUITexture _titleFade;
	private SpriteRenderer _houseSpr;
	private PlayerBehaviour _player;
	private Transform[] _stairs;
	private bool _procdInput;
	private Queue<GameObject> _items;
	private GameObject _shatteredHouse;
	private AudioPlay _audio;
	private bool _startedLeaving;
	private bool _shattered;
	private List<GameObject> _stackableObjects = new List<GameObject>();
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
		_houseSpr = null;
		LastDialogue = false;
		_player = null;
		_stairs = new Transform[0];
		_procdInput = false;
		_items = new Queue<GameObject>();
		_audio = GameObject.FindGameObjectWithTag("audio_mgr").GetComponent<AudioPlay>();
		DontDestroyOnLoad(_audio);
		_phase = GamePhase.SetupTitle;
		_startedLeaving = false;
		_shattered = false;
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
				if(!Application.isLoadingLevel && Application.loadedLevelName != "breakup")
					Application.LoadLevel("breakup");
				if(Application.loadedLevelName == "breakup" && !Application.isLoadingLevel)
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
			if(!_timer.Expired && !_startedLeaving)
				fadeIn ();
			else
			{
				if(!_startedLeaving)
				{

					//show the breakup
					_gui.EnqueueText("We're through.");
					_gui.EnqueueText("I'm leaving.");
					_timer.SetTimer(15f); //length of whole breakup, animation + dlog
					_timer.StartTimer();
					GameObject.FindGameObjectWithTag("significant_other").GetComponent<SOBehaviour>().LeaveInTime(5f);
					_startedLeaving = true;
				}
				else if(_timer.Expired)
				{
					_phase = GamePhase.ShatterHouse;
					Application.LoadLevel("shatter");
					_timer.SetTimer(1f);
					_timer.StartTimer();
				}
			}
			break;
		case GamePhase.ShatterHouse:
			if(!_shattered && _timer.Expired && !Application.isLoadingLevel && Application.loadedLevelName == "shatter")
			{
				GameObject[] items = GameObject.FindGameObjectsWithTag("item");
				for(int i=0;i<items.Length;++i)
					items[i].SetActive(false);

				//animate shattered house using rigidbodies
				_timer.SetTimer(3f);
				_timer.StartTimer();
				_shattered = true;
			}
			if(_shattered && _timer.Expired)
			{
				_phase = GamePhase.PlaySetup;
				Application.LoadLevel("house");
			}
			break;
		case GamePhase.PlaySetup:
			if(!Application.isLoadingLevel && Application.loadedLevelName == "house")
			{
				_houseSpr = GameObject.FindGameObjectWithTag("houseBG").GetComponent<SpriteRenderer>();
				if(_player == null)
					_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
				if(_stairs.Length == 0)
					_stairs = GameObject.FindGameObjectWithTag("stairs").GetComponentsInChildren<Transform>();
				if(_items.Count == 0)
				{
					GameObject[] items = GameObject.FindGameObjectsWithTag("item");
					for(int i=0;i<items.Length;++i)
						items[i].SetActive(false);
					
					for(int i=items.Length - 1; i > 0; --i)
					{
						int j = UnityEngine.Random.Range(0, i+1);
						GameObject tmp = items[j];
						items[j] = items[i];
						items[i] = tmp;
					}
					_items = new Queue<GameObject>(items);
				}

				if(_timer.Expired)
					_phase = GamePhase.Play;
			}
			break;
		case GamePhase.Play:
			if(!_procdInput)
				_procdInput = ProcessInput();
			else
				_procdInput = false;
			break;
		case GamePhase.PlayEnd:
			//John Burgar says, "No nested ifs motherfucker!"
			if(!Application.isLoadingLevel && (Application.loadedLevelName != "end_dance" || Application.loadedLevelName != "end_cry"))
			{
				_stackableObjects.Clear();
				
				if(PlayerScore < 16)
					Application.LoadLevel("end_dance");
				else
					Application.LoadLevel("end_cry");
			}
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
		{
			_player.transform.Translate(dt * -_player.speed, 0f, 0f);
			_player.AnimController.SetTrigger("walkLeft");
		}
		if(hAxis > 0 && _player.RightX + dt * _player.speed < _houseSpr.bounds.extents.x)
		{
			_player.transform.Translate(dt * _player.speed, 0f, 0f);
			_player.AnimController.SetTrigger("walkRight");
		}

		if(up || down)
		{
			for(int i=0;i<_stairs.Length;++i)
			{
				Vector3 pos = _stairs[i].position;
				if((_player.TopY >= pos.y && _player.BottomY <= pos.y) &&
				   (_player.LeftX <= pos.x+2f && _player.RightX >= pos.x-2f))
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

				if(_items.Count == 5)
					heapSpr.sprite = heapSprite;
				//_player.Carrying.SetActive(true);
				//_gui.DisplayItem(_player.Carrying.GetComponent<SpriteRenderer>());
				//_player.Carrying.SetActive(false);

			}
			else if(_player.Carrying != null && _player.transform.position.y > -3f)
			{
				int points = 2;
				Vector3 oPos = _player.CarriedOrigPos;
				Vector3 pPos = _player.transform.position;
				if((oPos.x > 0f) ^ (pPos.x > 0f)) //I can't type the explanation.  Just make a truth table if you're confused
					points--;
				if((oPos.y > 0f) ^ (pPos.y > 0f))
					points--;

				_player.Score+=points;
				_audio.VaryMusic = (_items.Count <= 15); //start to change music after awhile

				_player.Carrying.SetActive(true);

				if(_stackableObjects == null)
				{
					_stackableObjects.Clear();
					_stackableObjects.AddRange(FindGameObjectsWithLayer(9));
				}

				if(_stackableObjects != null && _stackableObjects.Count > 0)
				{
					foreach(GameObject go in _stackableObjects)
					{
						var sprRend = go.GetComponent<SpriteRenderer>();
						var goPos = go.transform.position;
						if((_player.LeftX >= goPos.x-sprRend.bounds.extents.x && _player.RightX <= goPos.x+sprRend.bounds.extents.x) || 
						   (_player.BottomY >= goPos.x-sprRend.bounds.extents.x && _player.TopY <= goPos.x+sprRend.bounds.extents.x))
						{
							if(_player.Carrying.layer == 8 && go == _player.Carrying)
								_player.Carrying.transform.position = go.transform.FindChild("stackPosition").transform.position; //add offset based on which story you are in.
							else
								_player.Carrying.transform.position = _player.transform.position - _player.GetComponent<SpriteRenderer>().bounds.extents + 0.1f * Vector3.forward;
						}
						else
						{
							_player.Carrying.transform.position = _player.transform.position - _player.GetComponent<SpriteRenderer>().bounds.extents + 0.1f * Vector3.forward;
						}
					}
				}
				else
					_player.Carrying.transform.position = _player.transform.position - _player.GetComponent<SpriteRenderer>().bounds.extents + 0.1f * Vector3.forward;
				
				_player.ClearCarry();
				_gui.DisplayItem();
				if(_items.Count == 0)
					_phase = GamePhase.PlayEnd;
			}

		}

		return (up || down || placePickup);

	}
	
	///Finds GameObjects by layer. Similar to GameObject.FindGameObjectsWithTag.
	/// Used to find objects in a specific layer.
	/// 
	/// EXAMPLE: Add objects in the "ObjectCanBePlacedOn" layer to a list and
	/// make a check to see if you are colliding with it so you can stack an
	/// object in the "PlaceOnOtherObject" layer on top of the "ObjectCanBePlacedOn" object.
	private GameObject[] FindGameObjectsWithLayer(int layer)
	{
		GameObject[] goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
		List<GameObject> goList = new List<GameObject>();
		for (int i = 0; i < goArray.Length; i++) {
			if (goArray[i].layer == layer) {
				goList.Add(goArray[i]);
			}
		}
		if (goList.Count == 0) {
			return null;
		}
		return goList.ToArray();
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
