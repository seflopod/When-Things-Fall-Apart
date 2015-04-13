using UnityEngine;
using System.Collections;

public class CryBehaviour : MonoBehaviour
{
	private Animator _anim;

	private void Start()
	{
		_anim = gameObject.GetComponent<Animator>();
		//_anim.Play("player_idle");
	}

	private void StartedCrying()
	{
		_anim.SetBool("isCrying", true);
	}
}
