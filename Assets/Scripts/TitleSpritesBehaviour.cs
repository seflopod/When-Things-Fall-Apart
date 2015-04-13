using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleSpritesBehaviour : MonoBehaviour
{
	delegate void EndOfFadeCallback();
	#region fields
	public TitleSpriteData[] titleSprites = new TitleSpriteData[22];
	public float delay = 3f;
	public float timeToFade = 3f;
	private GameObject[] _children;
	private SimpleTimer _timer = new SimpleTimer();
	private GameObject _titleBtns;
	private GameObject _credits;
	private Image _fadeImg;
	#endregion

	#region monobehaviour
	private void Start()
	{
		ResetChildren();
		PlaceSprites();
		_timer.SetTimer(delay);
		_timer.StartTimer();
		_titleBtns = GameObject.FindGameObjectWithTag("titleButtons");
		Button[] buttons = _titleBtns.transform.
											GetComponentsInChildren<Button>();
		for(int i=0; i<buttons.Length; ++i)
		{
			Button btn = buttons[i];
			switch(btn.gameObject.name)
			{
			case "PlayBtn":
				btn.onClick.AddListener(() => {
					StartCoroutine(fadeOut(timeToFade,
					                       GameManager.Instance.StartPlay));
					_titleBtns.SetActive(false);
				});
				break;
			case "CreditsBtn":
				btn.onClick.AddListener(() => {
					_titleBtns.SetActive(false);
					_credits.SetActive(true);
					_credits.transform.SetAsLastSibling();
				});
				break;
			case "QuitBtn":
				btn.onClick.AddListener(() => {
					GameManager.Instance.Phase = GamePhase.EndGame;
				});
				break;
			default:
				break;
			}
		}
		_titleBtns.SetActive(false);
		_credits = GameObject.FindGameObjectWithTag("creditsPanel");
		_credits.transform.SetAsFirstSibling();
		_credits.transform.GetComponentInChildren<Button>().onClick.
															AddListener(() => {
			DestroySprites();
			ResetChildren();
			PlaceSprites();
			_credits.transform.SetAsFirstSibling();
			_credits.SetActive(false);
			_titleBtns.SetActive(true);
			DropSprites();
		});
		_credits.SetActive(false);
	}

	private void Update()
	{
		if(_timer.Expired && GameManager.Instance.Phase != GamePhase.ShowMenu)
		{
			GameManager.Instance.Phase = GamePhase.ShowMenu;
			_titleBtns.SetActive(true);
			DropSprites();
		}
	}
	#endregion

	#region methods
	public void ResetChildren()
	{
		_children = new GameObject[titleSprites.Length];
		for(int i=0; i<_children.Length; ++i)
		{
			_children[i] = new GameObject();
		}
	}

	public void PlaceSprites()
	{
		for(int i=0; i<titleSprites.Length; ++i)
		{
			TitleSpriteData spr = titleSprites[i];
			_children[i].transform.parent = transform;
			SpriteRenderer sr = _children[i].AddComponent<SpriteRenderer>();
			sr.sprite = spr.sprite;
			sr.sortingLayerName = "Midground";
			sr.material.color = new Color(240 / 255f, 230 / 255f, 221 / 255f);
			Vector3 pos = new Vector3(spr.screenPosition.x / 100f - 8,
			                          spr.screenPosition.y / 100f - 4.5f,
			                          0f);
			_children[i].transform.position = pos;
			(_children[i].AddComponent<Rigidbody2D>()).isKinematic = true;
			BoxCollider2D bc = (_children[i].AddComponent<BoxCollider2D>());
			bc.center = Vector2.zero;

			//100 is here assuming the sprite was imported on a 100 scale
			//this is the default and we have no reason to change it so I feel
			//safe in that assumption.
			bc.size = new Vector2(spr.screenPosition.width / 100,
			                      spr.screenPosition.height / 100);
		}
	}

	public void DropSprites()
	{
		for(int i=0; i<_children.Length; ++i)
		{
			//_children[i].rigidbody2D.gravityScale = 0.33f;
			_children[i].rigidbody2D.gravityScale = 0.0f;
			_children[i].rigidbody2D.isKinematic = false;
		}

		_children[0].rigidbody2D.AddForce(new Vector2(50, -50));
		_children[0].rigidbody2D.AddTorque(30f);
		_children[11].rigidbody2D.AddForce(new Vector2(-50, -50));
		_children[11].rigidbody2D.AddTorque(30f);
		_children[12].rigidbody2D.AddForce(new Vector2(50, 50));
		_children[12].rigidbody2D.AddTorque(-30f);
		_children[_children.Length - 1].rigidbody2D.AddForce(new Vector2(-50,
		                                                                 50));
		_children[_children.Length - 1].rigidbody2D.AddTorque(-30f);
	}

	public void DestroySprites()
	{
		for(int i=0; i<_children.Length; ++i)
			GameObject.Destroy(_children[i]);
	}

	private IEnumerator fadeOut(float timeToFadeOut, EndOfFadeCallback callback)
	{
		Image fadeImg = GameObject.FindGameObjectWithTag("fadeImage").
														GetComponent<Image>();
		fadeImg.transform.SetAsLastSibling();
		float startTime = Time.timeSinceLevelLoad;
		float dt = 0;
		Color c = fadeImg.color;
		while(dt <= timeToFade)
		{
			c.a = Mathf.Lerp(0, 1, dt / timeToFadeOut);
			fadeImg.color = c;
			dt = Time.timeSinceLevelLoad - startTime;
			yield return new WaitForEndOfFrame();
		}
		if(callback != null)
		{
			callback();
		}
	}
	#endregion
}
