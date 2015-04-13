using UnityEngine;
using System.Collections;

public class SOBehaviour : MonoBehaviour
{
	private Animator _anim;
	private SimpleTimer _timer;
	private bool _timerSet;
	private bool _leaving;

	private void Start()
	{
		_anim = gameObject.GetComponent<Animator>();
		_timer = new SimpleTimer();
		_timerSet = false;
		_leaving = false;
		_anim.SetBool("gone", false);
	}

	private void Update()
	{
		if(_timerSet && _timer.Expired && !_leaving)
		{
			_anim.SetTrigger("leave");
			_leaving = true;
			_anim.SetBool("gone", true);

			//the significant other makes the player cry
			GameObject.FindGameObjectWithTag("Player").
												GetComponent<PlayerBehaviour>().
													AnimController.
													SetTrigger("startCrying");
		}
	}

	private void LeaveForGood()
	{
		transform.position = new Vector3(-3f, 2.1f, 0f);
		Camera.main.transform.position = -10 * Vector3.forward;
		Camera.main.orthographicSize = 4.5f;
		Destroy(gameObject);
	}

	public void LeaveInTime(float t)
	{
		_timer.SetTimer(t);
		_timer.StartTimer();
		_timerSet = true;
	}

	public Animator AnimController {
		get { return _anim; }
		set { _anim = value; }
	}
}
