using UnityEngine;
using System.Collections;

namespace UnityRobot
{
	public class ServoModule : ModuleProxy
	{
		public float defaultAngle;
		public float minAngle;
		public float maxAngle;

		private short _angle;

		void Awake()
		{
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
			_angle = (short)(defaultAngle * 10f);
			canUpdate = true;
		}
		
		public override void Action ()
		{
		}
		
		public override void OnPop ()
		{
		}
		
		public override void OnPush ()
		{
			Push (_angle);
		}
		
		public float Angle
		{
			get
			{
				return (float)_angle / 10f;
			}
			set
			{
				short newAngle = (short)(Mathf.Clamp(value, minAngle, maxAngle) * 10f);
				if(_angle != newAngle)
				{
					_angle = newAngle;
					canUpdate = true;
				}
			}
		}
	}
}
