﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Player behaviour.
/// </summary>
/// <description>
/// Animation state machine parameters:
/// Triggers:
/// 	walkLeft
/// 	walkRight
/// 	startCrying
/// 	startDancing
/// Bools:
/// 	isCarrying
/// 	isCrying
/// 	isDancing
/// </description>
public class PlayerBehaviour : MonoBehaviour
{
	#region fields
	public float speed = 5f;
	private SpriteRenderer _spr;
	private GameObject _carrying;
	private Animator _anim;
	#endregion

	#region monobehaviour
	// Use this for initialization
	private void Start()
	{
		_spr = gameObject.GetComponent<SpriteRenderer>();
		_anim = gameObject.GetComponent<Animator>();
		_anim.Play("player_idle");
		Score = 0;
	}
	
	// Update is called once per frame
	private void Update()
	{
	
	}
	#endregion

	#region methods
	public void ClearCarry()
	{
		_carrying.transform.parent = null;
		_carrying = null;
		_anim.SetBool("isCarrying", false);
	}

	private void StartedCrying()
	{
		_anim.SetBool("isCrying", true);
	}

	private void StartedDancing()
	{
		_anim.SetBool("isDancing", true);
	}
	#endregion

	#region properities
	public float LeftX {
		get
		{
			return transform.position.x - _spr.sprite.rect.width / 200;
		}
	}

	public float RightX {
		get
		{
			return transform.position.x + _spr.sprite.rect.width / 200;
		}
	}

	public float BottomY {
		get
		{
			return transform.position.y - _spr.sprite.rect.height / 200;
		}
	}

	public float TopY {
		get
		{
			return transform.position.y + _spr.sprite.rect.height / 200;
		}
	}

	public GameObject Carrying {
		get { return _carrying; }
		set
		{
			_carrying = value;
			_carrying.SetActive(true);
			CarriedOrigPos = _carrying.transform.position;
			_carrying.transform.parent = transform;
			_carrying.transform.localPosition = Vector3.zero;
			_anim.SetBool("isCarrying", true);
		}
	}

	public Vector3 CarriedOrigPos { get; private set; }

	public float Score { get; set; }

	public Animator AnimController {
		get { return _anim; }
		set { _anim = value; }
	}
	#endregion
}
