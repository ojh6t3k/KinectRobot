using UnityEngine;
using System.Collections;
using UnityRobot;

public class SimpleJoint : MonoBehaviour
{
	public enum Axis
	{
		pX,
		mX,
		pY,
		mY,
		pZ,
		mZ
	}
	
	public Axis up;
	public Axis forward;
	public bool follow = false;
	public ServoModule servo;
	public bool CCW = false;
	public float offset = 0f;

	private Quaternion _baseRotation;
	private Vector3 _up;
	private Vector3 _forward;
	private Vector3 _forward2;
	private float _angle;

	// Use this for initialization
	void Start ()
	{
		_baseRotation = transform.localRotation;	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(follow == true)
		{
			_forward = AxisToVector(forward);
			_up = AxisToVector(up);
			Quaternion diffRotation = Quaternion.Inverse(_baseRotation) * transform.localRotation;
			_forward2 = diffRotation * _forward;
			_angle = AngleBetween(_forward, _forward2, _up);

			if(servo != null)
			{
				if(CCW == true)
					servo.Angle = -(_angle + offset);
				else
					servo.Angle = (_angle + offset);
			}
		}	
	}

	public Vector3 AxisToVector(Axis axis)
	{
		Vector3 vec = Vector3.zero;
		
		switch(axis)
		{
		case Axis.pX:
			vec = Vector3.right;
			break;
			
		case Axis.mX:
			vec = -Vector3.right;
			break;
			
		case Axis.pY:
			vec = Vector3.up;
			break;
			
		case Axis.mY:
			vec = -Vector3.up;
			break;
			
		case Axis.pZ:
			vec = Vector3.forward;
			break;
			
		case Axis.mZ:
			vec = -Vector3.forward;
			break;
		}
		
		return vec;
	}
	
	public float AngleBetween(Vector3 from, Vector3 to, Vector3 up)
	{
		float angle = Vector3.Angle(from, to);
		float sign = Vector3.Dot(Vector3.Cross(from, to), up);
		if(sign < 0f)
			angle *= -1f;
		
		return angle;
	}
}
