using UnityEngine;
using System.Collections;
using UnityRobot;


public class AvatarJoint : MonoBehaviour
{
	public ServoModule servo;

	public enum Axis
	{
		X,
		Y,
		Z
	}
	
	public bool follow = false;
	public Axis followAxis = Axis.X;
	public bool opposition = false;
	public Color gizmoColor = Color.cyan;


	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(follow == true && servo != null)
		{
			Vector3 refAxis = Vector3.zero;
			Vector3 rotAxis = Vector3.zero;
			switch(followAxis)
			{
			case Axis.X:
				refAxis = Vector3.forward;
				rotAxis = Vector3.right;
				break;
				
			case Axis.Y:
				refAxis = Vector3.right;
				rotAxis = Vector3.up;
				break;
				
			case Axis.Z:
				refAxis = Vector3.up;
				rotAxis = Vector3.forward;
				break;
			}

			Vector3 axis = transform.localRotation * refAxis;
			axis.Normalize();

			float angle = Vector3.Angle(axis, refAxis);
			float sign = Vector3.Dot(Vector3.Cross(axis, refAxis), rotAxis);
			if(sign < 0f)
				angle *= -1f;

			if(opposition == true)
				angle *= -1f;
			
			servo.Angle = angle;
		}	
	}

	void OnDrawGizmosSelected()
	{
		if(servo != null)
		{
			Gizmos.color = gizmoColor;
			Vector3 dir = Vector3.zero;
			switch(followAxis)
			{
			case Axis.X:
				dir = transform.right;
				break;
				
			case Axis.Y:
				dir = transform.up;
				break;
				
			case Axis.Z:
				dir = transform.forward;
				break;
			}
			if(opposition == true)
				dir *= -1f;
			
			Gizmos.DrawRay(transform.position, dir * 10f);
		}
	}
}
