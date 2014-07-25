using UnityEngine;
using System.Collections;
using System;
using UnityRobot;


public class AvatarJoint : MonoBehaviour
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

	public Axis linkForward;
	public Axis linkRight;

	public bool follow = false;
	public ServoModule panServo;
	public ServoModule tiltServo;
	public bool panCCW = false;
	public bool tiltCCW = false;

	private Quaternion _baseRotation;
	private Vector3 _linkUp;
	private Vector3 _linkForward;
	private Vector3 _linkRight;
	private Vector3 _linkUp2;
	private Vector3 _linkForward2;
	private Vector3 _linkRight2;
	private Vector3 _panUp;
	private Vector3 _panForward;
	private Vector3 _panRight;
	private Vector3 _panForward2;
	private Vector3 _tiltUp;
	private Vector3 _tiltForward;
	private Vector3 _tiltRight;
	private Vector3 _tiltForward2;
	private Vector3 _yawUp;
	private Vector3 _yawForward;
	private Vector3 _yawRight;
	private Vector3 _yawForward2;

	private float _panAngle = 0f;
	private float _tiltAngle = 0f;
	private float _yawAngle = 0f;


	void Awake()
	{
	}

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
			// compute link transform
			_linkForward = AxisToVector(linkForward);
			_linkRight = AxisToVector(linkRight);
			_linkUp = Vector3.Cross(_linkForward, _linkRight);
			Quaternion diffRotation = Quaternion.Inverse(_baseRotation) * transform.localRotation;
			_linkForward2 = diffRotation * _linkForward;
			_linkRight2 = diffRotation * _linkRight;
			_linkUp2 = diffRotation * _linkUp;

			// compute tilt transform
			_tiltForward = _linkForward;
			_tiltRight = Vector3.Project(_linkForward2, _linkRight) + Vector3.Project(_linkForward2, _linkUp);
			_tiltUp = Vector3.Cross(_tiltForward, _tiltRight);
			_tiltForward2 = _linkForward2;
			_tiltAngle = AngleBetween(_tiltForward, _tiltForward2, _tiltUp);

			// compute yaw transform
			Quaternion yawTransform = Quaternion.FromToRotation(_linkForward, _linkForward2);
			_yawUp = _linkForward2;
			_yawForward = yawTransform * _linkRight;
			_yawRight = Vector3.Cross(_yawUp, _yawForward);
			_yawForward2 = _linkRight2;
			_yawAngle = AngleBetween(_yawForward, _yawForward2, _yawUp);

			// compute pan transform
			_panUp = _linkForward;
			_panForward = _linkRight;
			_panRight = Vector3.Cross(_panUp, _panForward);
			if(_tiltAngle == 0f)
			{
				_panForward2 = _yawForward2;
				_panAngle = _yawAngle;
			}
			else
			{
				_panForward2 = Vector3.Project(_linkForward2, _panForward) + Vector3.Project(_linkForward2, _panRight);
				float angle = AngleBetween(_panForward, _panForward2, _panUp);
				if(Mathf.Abs(_panAngle - angle) > 150f)
					_panAngle = AngleBetween(_panForward, -_panForward2, _panUp);
				else
					_panAngle = angle;

				_yawAngle -= _panAngle;
				yawTransform = Quaternion.AngleAxis(-_yawAngle, _linkForward2);
				_linkRight2 = yawTransform * _linkRight2;
				_linkUp2 = yawTransform * _linkUp2;

				_panForward2 = Quaternion.AngleAxis(-90f, _panUp) * _linkUp2;
				_panAngle = AngleBetween(_panForward, _panForward2, _panUp);
			}

			float sign = Vector3.Dot(_linkUp2, _tiltUp);
			if(sign < 0f)
			{
				_tiltUp *= -1f;
				_tiltRight *= -1f;
				_tiltAngle *= -1f;
			}

			// apply servo angle
			if(follow == true)
			{
				if(panServo != null)
				{
					if(panCCW == true)
						panServo.Angle = -_panAngle;
					else
						panServo.Angle = _panAngle;
				}

				if(tiltServo != null)
				{
					if(tiltCCW == true)
						tiltServo.Angle = -_tiltAngle;
					else
						tiltServo.Angle = _tiltAngle;
				}
			}
		}
	}

#if UNITY_EDITOR
	public bool debugLink = false;
	public bool debugPan = false;
	public bool debugTilt = false;

	void OnDrawGizmos()
	{
		if(Application.isPlaying == false)
			return;

		if(follow == true)
		{
			Vector3 pos = transform.position;
			Quaternion rot = transform.parent.rotation * _baseRotation;

			if(debugLink == true)
			{
				Gizmos.color = Color.magenta;
				Gizmos.DrawRay(pos, rot * (_linkForward2 * 100f));
				
				Gizmos.color = Color.cyan;
				Gizmos.DrawRay(pos, rot * (_linkUp2 * 100f));
			}

			if(debugPan == true)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawRay(pos, rot * (_panUp * 100f));
				
				Gizmos.color = Color.blue;
				Gizmos.DrawRay(pos, rot * (_panForward * 100f));

				Gizmos.color = Color.red;
				Gizmos.DrawRay(pos, rot * (_panRight * 100f));

				Gizmos.color = Color.yellow;
				Gizmos.DrawRay(pos, rot * (_panForward2 * 100f));
			}

			if(debugTilt == true)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawRay(pos, rot * (_tiltUp * 100f));
				
				Gizmos.color = Color.blue;
				Gizmos.DrawRay(pos, rot * (_tiltForward * 100f));

				Gizmos.color = Color.red;
				Gizmos.DrawRay(pos, rot * (_tiltRight * 100f));

				Gizmos.color = Color.yellow;
				Gizmos.DrawRay(pos, rot * (_tiltForward2 * 100f));
			}

			if(debugPan == true || debugTilt == true)
			{
				string message = "";
				if(debugPan == true)
					message += string.Format("Pan:{0:f2} ", _panAngle);
				if(debugTilt == true)
					message += string.Format("Tilt:{0:f2} ", _tiltAngle);
				Debug.Log(message);
			}
		}
	}
#endif

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
