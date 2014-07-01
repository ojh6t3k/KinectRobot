using UnityEngine;
using System.Collections;
using System;

namespace UnityRobot
{
	public class ADCModule : ModuleProxy
	{
		public int threshold = 0;

		public EventHandler OnEnter;
		public EventHandler OnExit;

		protected bool _enter;
		protected ushort _value;

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
			_enter = false;
			_value = 0;
		}
		
		public override void Action ()
		{
			if(threshold > 0)
			{
				if(_value < threshold)
				{
					if(_enter == true)
					{
						_enter = false;
						if(OnExit != null)
							OnExit(this, null);
					}
				}
				else
				{
					if(_enter == false)
					{
						_enter = true;
						if(OnEnter != null)
							OnEnter(this, null);
					}
				}
			}
		}

		public override void OnPop ()
		{
			Pop(ref _value);
		}
		
		public override void OnPush ()
		{
		}

		public ushort Value
		{
			get
			{
				return _value;
			}
		}

		public bool isEnter
		{
			get
			{
				return _enter;
			}
		}
	}
}
