using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityRobot
{
	public class PanTiltController : ModuleProxy
	{
		private short _panAngle;
		private short _tiltAngle;
		
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
			_panAngle = 0;
			_tiltAngle = 0;
		}
		
		public override void Action ()
		{
		}
		
		public override void OnPop ()
		{
		}
		
		public override void OnPush ()
		{
			Push(_panAngle);
			Push(_tiltAngle);
		}
		
		public void ControlPoint(float distance, Vector2 offset)
		{
			short pan = (short)(-Mathf.Atan(offset.x / distance) * 180f / Mathf.PI * 10f);
			short tilt = (short)(Mathf.Atan(offset.y / distance) * 180f / Mathf.PI * 10f);

			if(_panAngle != pan || _tiltAngle != tilt)
			{
				_panAngle = pan;
				_tiltAngle = tilt;
				canUpdate = true;
			}
		}
	}
}
