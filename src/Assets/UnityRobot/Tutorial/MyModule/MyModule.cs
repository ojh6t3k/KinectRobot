using UnityEngine;
using System.Collections;
using UnityRobot;

public class MyModule : ModuleProxy
{
	public byte input_byte;
	public ushort input_ushort;
	public short input_short;

	public byte output_byte;
	public ushort output_ushort;
	public short output_short;
	
	void Awake()
	{
		Reset();
	}
	
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
		input_byte = 0;
		input_ushort = 0;
		input_short = 0;
	}
	
	public override void Action ()
	{
	}
	
	public override void OnPop ()
	{
		Pop(ref output_byte);
		Pop(ref output_ushort);
		Pop(ref output_short);

		canUpdate = true;
	}
	
	public override void OnPush ()
	{
		Push(input_byte);
		Push(input_ushort);
		Push(input_short);
	}
}
