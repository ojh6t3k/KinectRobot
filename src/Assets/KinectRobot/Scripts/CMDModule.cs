using UnityEngine;
using System.Collections;
using UnityRobot;

public class CMDModule : ModuleProxy
{
	private byte _value;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public override void Reset ()
	{
		_value = 0;
	}
	
	public override void Action ()
	{
	}
	
	public override void OnPop ()
	{
		Pop(ref _value);
	}
	
	public override void OnPush ()
	{
		Push(_value);
	}
	
	public byte Value
	{
		get
		{
			return _value;
		}
		set
		{
			if(_value != value)
			{
				_value = value;
				canUpdate = true;
			}
		}
	}
}
