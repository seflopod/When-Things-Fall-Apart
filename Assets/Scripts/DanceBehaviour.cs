using UnityEngine;
using System.Collections;

public class DanceBehaviour : MonoBehaviour
{
	private Animator _anim;
	
	private void Start()
	{
		_anim = gameObject.GetComponent<Animator>();
		//_anim.Play("player_idle");
	}
	
	private void StartedDancing()
	{
		_anim.SetBool("isDancing", true);
	}
}
