using UnityEngine;
using System.Collections;

public class ShardsBehaviour : MonoBehaviour
{
	private Rigidbody2D[] _shards;
	private bool _affectShards;

	private void Start()
	{
		_affectShards = true;
		_shards = gameObject.GetComponentsInChildren<Rigidbody2D>();
	}

	private void FixedUpdate ()
	{
		if(_affectShards)
		{
			for(int i=0;i<_shards.Length;++i)
			{
				_shards[i].AddForce(Random.Range(-50f, 50f) * Vector2.right);
				//_shards[i].AddForce(Random.Range(-10f, 0f) * Vector2.up);
				_shards[i].AddTorque(Random.Range(-300f, 300f));
			}
			_affectShards = false;
		}
	}
}
