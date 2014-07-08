using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour 
{
	public float	_Speed = 500f;


	// Start ----------------------------------------------------------
	void Start () 
	{

	}



	void Update () 
	{
		transform.Rotate(0f, Time.deltaTime * _Speed, 0f);
	}
}
