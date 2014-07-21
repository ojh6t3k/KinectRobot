using UnityEngine;
using System.Collections;

namespace UnityRobot
{
	public class PulseModule : ModuleProxy
	{
		private ushort _duration;
		
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
			_duration = 0;
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
			Push(_duration);
		}
		
		public int DurationTime
		{
			set
			{
				_duration = (ushort)value;
				canUpdate = true;
			}
		}
	}
}
