using UnityEngine;
using System.Collections;

namespace UnityRobot
{
	public class PWMModule : ModuleProxy
	{
		private byte _value;
		
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
			_value = 0;
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
			Push (_value);
		}
		
		public int Value
		{
			get
			{
				return (int)_value;
			}
			set
			{
				byte newValue = (byte)value;
				if(_value != newValue)
				{
					_value = newValue;
					canUpdate = true;
				}
			}
		}
	}
}
