using UnityEngine;
using System.Collections;

public class AutoScale : MonoBehaviour 
{

	// Start
	void Start () 
	{
		transform.localScale = Vector3.one * ((float)Screen.height / 768f);
	}




}
