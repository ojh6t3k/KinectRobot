using UnityEngine;
using System.Collections;


public enum EAxis
{
	X,
	Y,
	Z,
}


public class AutoRotate : MonoBehaviour 
{
	public EAxis Axis = EAxis.Y;
	public float	_Speed = 500f;


	// Start ----------------------------------------------------------
	void Start () 
	{

	}



	void Update () 
	{
		switch(Axis)
		{
		case EAxis.X:
			transform.Rotate(Time.deltaTime * _Speed, 0f, 0f);
			break;

		case EAxis.Y:
			transform.Rotate(0f, Time.deltaTime * _Speed, 0f);
			break;

		case EAxis.Z:
			transform.Rotate(0f, 0f, Time.deltaTime * _Speed);
			break;
		}
	}
}
