using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityRobot
{
	public class WheelController : ModuleProxy
	{
		private short _leftSpeed;
		private short _rightSpeed;
		
		void Awake()
		{
			Reset();
		}
		
		// Use this for initialization
		void Start ()
		{
			
		}
		
		// Update is called once per frame
		void Update ()
		{
			
		}
		
		public override void Reset ()
		{
			_leftSpeed = 0;
			_rightSpeed = 0;
		}
		
		public override void Action ()
		{
		}
		
		public override void OnPop ()
		{
		}
		
		public override void OnPush ()
		{
			Push(_rightSpeed);
			Push(_leftSpeed);
		}
		
		public void ControlRect(Vector2 vector)
		{
			float left = vector.x + vector.y;
			float right = vector.x - vector.y;

			ControlDirect(left, right);
		}

		public void ControlCircle(Vector2 vector)
		{
			float left = 0;
			float right = 0;
			float angle = Vector2.Angle(new Vector2(0, 1f), vector.normalized);
			if(angle > 90)
				angle = 180 - angle;
			if(angle >= 0f && angle <= 60f)
			{
				if(vector.y > 0)
					left = vector.magnitude;
				else if(vector.y < 0)
					left = -vector.magnitude;
				
				right = -left;
				
				if(vector.x > 0)
					right *= (1.0f - vector.x);
				else if(vector.x < 0)
					left *= (1.0f + vector.x);
			}
			else
			{
				left = vector.x;
				right = vector.x;
			}
			
			ControlDirect(left, right);
		}
		
		public void ControlDirect(float left, float right)
		{
			left = Mathf.Clamp(left * 100f, -100f, 100f);
			short speed = (short)left;
			if(_leftSpeed != speed)
			{
				_leftSpeed = speed;
				canUpdate = true;
			}
			
			right = Mathf.Clamp(right * 100f, -100f, 100f);
			speed = (short)right;
			if(_rightSpeed != speed)
			{
				_rightSpeed = speed;
				canUpdate = true;
			}
		}
	}
}
