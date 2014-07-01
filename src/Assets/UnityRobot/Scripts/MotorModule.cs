using UnityEngine;
using System.Collections;

namespace UnityRobot
{
	public class MotorModule : ModuleProxy
	{
		private short _speed;
		
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
			_speed = 0;
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
			Push (_speed);
		}
		
		public int speed
		{
			get
			{
				return (int)_speed;
			}
			set
			{
				short newValue = (short)Mathf.Clamp(value, -100, 100);
				if(_speed != newValue)
				{
					_speed = newValue;
					canUpdate = true;
				}
			}
		}
	}
}
