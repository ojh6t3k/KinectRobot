using UnityEngine;
using System.Collections;

public class EachGestureJoint : MonoBehaviour 
{
	public bool _bIsContact = false;



	void OnTriggerStay(Collider other) 
	{
		if (tag == other.tag)
			_bIsContact = true;
		else
			_bIsContact = false;
	}

	void OnTriggerExit(Collider other) 
	{
		_bIsContact = false;
	}

}
