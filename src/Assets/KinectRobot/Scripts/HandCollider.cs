using UnityEngine;
using System.Collections;

public class HandCollider : MonoBehaviour 
{

	void OnCollisionEnter(Collision collision) 
	{
		if (collision.gameObject.tag == "Gem")
		{
			//Debug.Log(collision.gameObject.name);
		}
	}
}
