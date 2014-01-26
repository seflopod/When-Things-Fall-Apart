using UnityEngine;

/// <summary>
/// Simple timer.
/// </summary>
/// <description>
/// <para>This class is used in place of constantly recreating variables for timing.  This should
/// not be confused with the .NET class Timer, which is used to execute methods after a certain
/// amount of time has passed.
/// </para>
/// </description>
public class SimpleTimer
{
	#region fields
	private float _startTime;
	private float _timerLength;
	private bool _stopped;
	#endregion

	#region ctors
	/// <summary>
	/// Initializes a new instance of the <see cref="SimpleTimer"/> class with a length of 0.
	/// </summary>
	public SimpleTimer()
	{
		_timerLength = 0.0f;
		_stopped = true;
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="SimpleTimer"/> class.
	/// </summary>
	/// <param name='length'>
	/// Timer length.
	/// </param>
	public SimpleTimer(float length)
	{
		SetTimer(length);
		_stopped = true;
	}
	#endregion

	#region methods
	/// <summary>
	/// Sets the timer to the passed length.
	/// </summary>
	/// <param name='length'>
	/// Length.
	/// </param>
	public void SetTimer(float length)
	{
		_timerLength = length;
	}
	
	/// <summary>
	/// Starts the timer.
	/// </summary>
	public void StartTimer()
	{
		_startTime = Time.time;
		_stopped = false;
	}

	public void StopTimer()
	{
		_stopped = true;
	}
	#endregion

	#region properties
	/// <summary>
	/// Gets the time remaining.
	/// </summary>
	/// <value>
	/// The time remaining.
	/// </value>
	public float TimeRemaining
	{
		get { return _timerLength - Time.time + _startTime; }
	}
	
	/// <summary>
	/// Gets the timer length.
	/// </summary>
	/// <value>
	/// The timer length.
	/// </value>
	public float Length
	{
		get { return _timerLength; }
	}
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="SimpleTimer"/> is expired.
	/// </summary>
	/// <value>
	/// <c>true</c> if expired; otherwise, <c>false</c>.
	/// </value>
	public bool Expired
	{
		get { return (Time.time - _startTime >= _timerLength); }
	}
	#endregion
}