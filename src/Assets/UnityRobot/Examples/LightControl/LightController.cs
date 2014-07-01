using UnityEngine;
using System.Collections;
using System;


namespace UnityRobot
{
	public class LightController : MonoBehaviour
	{
		public ADCModule dial;
		public DigitalModule button;

		public int maxADCValue;
		public float minIntensity;
		public float maxIntensity;
		public bool risingEdgeTurnOn = true;

		public EventHandler OnTurnOn;
		public EventHandler OnTurnOff;

		private float _intensity;
		private bool _turn = false;

		// Use this for initialization
		void Start ()
		{
			if(risingEdgeTurnOn == true)
			{
				button.OnRisingEdge += OnTurnToggle;
			}
			else
			{
				button.OnFallingEdge += OnTurnToggle;
			}

			_intensity = maxIntensity;
			light.intensity = 0f;
			_turn = false;
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(_turn == true)
			{
				_intensity = ((float)dial.Value / (float)maxADCValue) * (maxIntensity - minIntensity) + minIntensity;
				light.intensity = _intensity;
			}
		}

		void OnTurnToggle(object sender, EventArgs e)
		{
			if(_turn == true)
			{
				_turn = false;
				light.intensity = 0f;

				if(OnTurnOff != null)
					OnTurnOff(this, null);
			}
			else
			{
				light.intensity = _intensity;
				_turn = true;
				
				if(OnTurnOn != null)
					OnTurnOn(this, null);
			}
		}

		public bool IsTurnOn
		{
			get
			{
				return _turn;
			}
		}

		public float Intensity
		{
			get
			{
				return _intensity;
			}
		}
	}
}
