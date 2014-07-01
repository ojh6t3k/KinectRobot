using UnityEngine;
using System.Collections;
using System;

namespace UnityRobot
{
	public class DigitalModule : ModuleProxy
	{
		public enum Mode
		{
			OUTPUT = 0,
			INPUT = 1,
			INPUT_PULLUP = 2
		}

		public Mode mode = Mode.OUTPUT;

		public EventHandler OnRisingEdge;
		public EventHandler OnFallingEdge;

		private Mode _mode;
		private byte _preValue;
		private byte _value;

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
			if(_mode != mode)
			{
				_mode = mode;
				canUpdate = true;
			}
		}

		public override void Reset ()
		{
			_mode = mode;
			if(mode == Mode.OUTPUT)
			{
				_value = 0;
			}
			else
			{
				_preValue = _value;
			}
			canUpdate = true;
		}
		
		public override void Action ()
		{
			if(mode != Mode.OUTPUT)
			{
				if(_preValue != _value)
				{
					_value = _preValue;
					if(_preValue > 0)
					{
						if(OnRisingEdge != null)
							OnRisingEdge(this, null);
					}
					else
					{
						if(OnFallingEdge != null)
							OnFallingEdge(this, null);
					}
				}
			}
		}

		public override void OnPop ()
		{
			Pop(ref _preValue);
		}
		
		public override void OnPush ()
		{
			Push((byte)mode);
			Push(_value);
		}

		public byte Value
		{
			get
			{
				return _value;
			}
			set
			{
				if(mode == Mode.OUTPUT)
				{
					if(_value != value)
					{
						_value = value;
						canUpdate = true;
					}
				}
			}
		}
	}
}
