using UnityEngine;
using System.Collections;
using System;
using UnityRobot;


public class KinectAvatarJoint : MonoBehaviour
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
	
	public Axis linkDirection;
	public Axis panUp;
	public Axis panForward;
	public Axis tiltUp;
	public Axis tiltForward;
	
	public ServoModule panServo;
	public ServoModule tiltServo;
	public bool panFollow = false;
	public bool tiltFollow = false;
	public bool panCCW = false;
	public bool tiltCCW = false;
	
	public bool debug = false;
	public float drawLineLength = 1f;
	
	private Quaternion _sRotation;
	private Quaternion _qParent;
	private Vector3 _panUp;
	private Vector3 _tiltUp;
	private Vector3 _panForward;
	private Vector3 _tiltForward;
	private Vector3 _linkDirection;
	private Vector3 _panProjection;
	private Vector3 _tiltProjection;
	private float _panAngle;
	private float _tiltAngle;
	
	void Awake()
	{
	}
	
	// Use this for initialization
	void Start ()
	{
		_sRotation = transform.localRotation;
		_qParent = Quaternion.identity;
		_panAngle = 0f;
		_tiltAngle = 0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Quaternion qParent = _sRotation;
		Quaternion qEnd = transform.localRotation;
		Vector3 right = Vector3.zero;
		
		_linkDirection = qEnd * AxisToVector(linkDirection);
		
		// compute pan angle
		_panUp = qParent * AxisToVector(panUp);
		_panForward = qParent * AxisToVector(panForward);
		right = qParent * Vector3.Cross(AxisToVector(panUp), AxisToVector(panForward));
		_panProjection = Vector3.Project(_linkDirection, right) + Vector3.Project(_linkDirection, _panForward);
		_panProjection.Normalize();
		
		_panAngle = 0f;
		if(_panProjection != Vector3.zero)
		{
			_panAngle = Vector3.Angle(_panForward, _panProjection);
			float sign = Vector3.Dot(Vector3.Cross(_panForward, _panProjection), _panUp);
			if(sign < 0f)
				_panAngle *= -1f;
		}
		qParent *= Quaternion.AngleAxis(_panAngle, AxisToVector(panUp));
		
		// compute tilt angle
		_tiltUp = qParent * AxisToVector(tiltUp);
		_tiltForward = qParent * AxisToVector(tiltForward);
		right = qParent * Vector3.Cross(AxisToVector(tiltUp), AxisToVector(tiltForward));
		_tiltProjection = Vector3.Project(_linkDirection, right) + Vector3.Project(_linkDirection, _tiltForward);
		_tiltProjection.Normalize();
		
		_tiltAngle = 0f;
		if(_tiltProjection != Vector3.zero)
		{
			_tiltAngle = Vector3.Angle(_tiltForward, _tiltProjection);
			float sign = Vector3.Dot(Vector3.Cross(_tiltForward, _tiltProjection), _tiltUp);
			if(sign < 0f)
				_tiltAngle *= -1f;
		}
		
		if(panFollow == true && panServo != null)
		{
			if(panCCW == false)
				panServo.Angle = _panAngle;
			else
				panServo.Angle = -_panAngle;
		}
		
		if(tiltFollow == true && tiltServo != null)
		{
			if(tiltCCW == false)
				tiltServo.Angle = _tiltAngle;
			else
				tiltServo.Angle = -_tiltAngle;
		}
	}
	
	#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if(debug == false || Application.isPlaying == false)
			return;
		
		Vector3 pos = transform.position;
		Quaternion rot = transform.parent.rotation;
		
		Gizmos.color = Color.magenta;
		Gizmos.DrawRay(pos, rot * (_linkDirection * drawLineLength));
		
		Gizmos.color = Color.green;
		Gizmos.DrawRay(pos, rot * (_panUp * drawLineLength));
		
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(pos, rot * (_panForward * drawLineLength));
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(pos, rot * (_panProjection * drawLineLength));
		
		Gizmos.color = Color.green;
		Gizmos.DrawRay(pos, rot * (_tiltUp * drawLineLength));
		
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(pos, rot * (_tiltForward * drawLineLength));
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(pos, rot * (_tiltProjection * drawLineLength));
		
		Debug.Log(string.Format("Pan:{0:f2}, Tilt:{1:f2}", _panAngle, _tiltAngle));
	}
	#endif
	
	public bool follow
	{
		set
		{
			panFollow = value;
			tiltFollow = value;
		}
	}
	
	Vector3 AxisToVector(Axis axis)
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
}
