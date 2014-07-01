using UnityEngine;
using System.Collections;
using System;

namespace UnityRobot
{
	public class ForceFeedback : ModuleProxy
	{
		protected byte _forceLimit;
		protected short _forceFeedback;
		
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
			_forceLimit = 100;
			_forceFeedback = 0;
			canUpdate = true;
		}
		
		public override void Action ()
		{
		}
		
		public override void OnPop ()
		{
			Pop(ref _forceFeedback);
		}
		
		public override void OnPush ()
		{
			Push(_forceLimit);
		}
		
		public int forceLimit
		{
			get
			{
				return (int)_forceLimit;
			}
			set
			{
				byte newValue = (byte)Mathf.Clamp(value, 0, 100);
				if(_forceLimit != newValue)
				{
					_forceLimit = newValue;
					canUpdate = true;
				}
			}
		}
		
		public int forceFeedback
		{
			get
			{
				return (int)_forceFeedback;
			}
		}
	}
}
