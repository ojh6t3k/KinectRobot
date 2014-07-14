using UnityEngine;
using System.Collections;

public class GemControl : MonoBehaviour 
{
	public AutoRotate  _scrX;
	public AutoRotate  _scrY;
	public AutoRotate  _scrZ;


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
}
