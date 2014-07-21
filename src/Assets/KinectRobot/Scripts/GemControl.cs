using UnityEngine;
using System.Collections;
using UnityRobot;

public class GemControl : MonoBehaviour 
{
	public AutoRotate  _scrX;
	public AutoRotate  _scrY;
	public AutoRotate  _scrZ;

	public PulseModule pulseModule;
	public int durationTime; // msec

	// Use this for initialization
	void Start () 
	{
		_scrX._Speed = Random.Range(-100f, 100f);
		_scrY._Speed = Random.Range(-100f, 100f);
		_scrZ._Speed = Random.Range(-100f, 100f);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnCollisionEnter(Collision collision) 
	{
		if (collision.gameObject.tag == "Hand")
		{
			pulseModule.DurationTime = durationTime;
		}
	}
}
