using UnityEngine;
using System.Collections;

public class TitleSpritesBehaviour : MonoBehaviour
{
	#region fields
	public TitleSpriteData[] titleSprites = new TitleSpriteData[22];
	public float delay = 3f;

	private GameObject[] _children;
	private float _timer;
	private bool _useTimer;
	#endregion

	#region monobehaviour
	private void Start()
	{
		_children = new GameObject[titleSprites.Length];
		for(int i=0;i<_children.Length;++i)
			_children[i] = new GameObject();
		_timer = 0f;
		_useTimer = false;
	}

	private void Update()
	{
		if(_useTimer)
		{
			_timer+=Time.deltaTime;
			if(_timer > delay)
			{
				DropSprites();
				_useTimer = false;
			}
		}
	}
	#endregion

	#region methods
	public void PlaceSprites()
	{
		float zPos = Mathf.Abs(Camera.main.transform.position.z);
		for(int i=0;i<titleSprites.Length;++i)
		{
			TitleSpriteData spr = titleSprites[i];
			//GameObject go = new GameObject();
			_children[i].transform.parent = transform;
			SpriteRenderer sr = _children[i].AddComponent<SpriteRenderer>();
			sr.sprite = spr.sprite;
			sr.sortingLayerName = "Midground";
			sr.material.color = new Color(240/255f, 230/255f, 221/255f);
			Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * spr.screenPosition.x/1600f, Screen.height * spr.screenPosition.y/900f, zPos));
			_children[i].transform.position = pos;
			(_children[i].AddComponent<Rigidbody2D>()).isKinematic = true;
			BoxCollider2D bc = (_children[i].AddComponent<BoxCollider2D>());
			bc.center = Vector2.zero;

			//100 is here assuming the sprite was imported on a 100 scale
			//this is the default and we have no reason to change it so I feel
			//safe in that assumption.
			bc.size = new Vector2(spr.screenPosition.width/100, spr.screenPosition.height/100);
			//_children[i] = go;
		}
		_useTimer = true;
	}

	public void DropSprites()
	{
		for(int i=0;i<_children.Length;++i)
		{
			_children[i].rigidbody2D.gravityScale = 0f;
			_children[i].rigidbody2D.isKinematic = false;
		}

		_children[0].rigidbody2D.AddForce(new Vector2(50,-50));
		_children[0].rigidbody2D.AddTorque(30f);
		_children[11].rigidbody2D.AddForce(new Vector2(-50,-50));
		_children[11].rigidbody2D.AddTorque(30f);
		_children[12].rigidbody2D.AddForce(new Vector2(50,50));
		_children[12].rigidbody2D.AddTorque(-30f);
		_children[_children.Length-1].rigidbody2D.AddForce(new Vector2(-50,50));
		_children[_children.Length-1].rigidbody2D.AddTorque(-30f);
	}

	public void DestroySprites()
	{
		for(int i=0;i<_children.Length;++i)
			GameObject.Destroy(_children[i]);
	}
	#endregion
}
