using UnityEngine;
using System.Collections;
using System;

namespace UnityRobot
{
	public class BalanceSensor : MonoBehaviour
	{
		public float sensitivity = 0.5f;
		public float scaleAngle = 0.1f;
		public float scaleHeight = 1f;
		public ADCModule pxAxis;
		public ADCModule mxAxis;
		public ADCModule pyAxis;
		public ADCModule myAxis;

		ArrayList _buffer = new ArrayList();
		float _sensitivity;
		ushort _pxValue;
		ushort _mxValue;
		ushort _pyValue;
		ushort _myValue;
		Vector2 _angle = new Vector2();
		float _height;

		void Awake()
		{
		}
		
		// Use this for initialization
		void Start ()
		{
			_sensitivity = sensitivity;
		}
		
		// Update is called once per frame
		void Update ()
		{
			ushort newPXValue = pxAxis.Value;
			ushort newMXValue = mxAxis.Value;
			ushort newPYValue = pyAxis.Value;
			ushort newMYValue = myAxis.Value;

			if(_pxValue != newPXValue || _mxValue != newMXValue || _pyValue != newPYValue || _myValue != newMYValue)
			{
				_pxValue = newPXValue;
				_mxValue = newMXValue;
				_pyValue = newPYValue;
				_myValue = newMYValue;

				if(_sensitivity != sensitivity)
				{
					_sensitivity = sensitivity;
					_buffer.Clear();
				}

				Vector4 filteredValue = new Vector4(_pxValue, _mxValue, _pyValue, _myValue);
				_sensitivity = Mathf.Clamp(_sensitivity, 0f, 1f);
				int n = (int)((1f - _sensitivity) * 100);
				if(n > 0)
				{
					if(_buffer.Count >= n && n > 0)
						_buffer.RemoveAt(0);
					_buffer.Add(new Vector4(_pxValue, _mxValue, _pyValue, _myValue));

					if(_buffer.Count > 0)
					{
						filteredValue = Vector4.zero;
						for(int i=0; i<_buffer.Count; i++)
							filteredValue += (Vector4)_buffer[i];
						
						filteredValue /= (float)_buffer.Count;
					}
				}

				_angle.x = filteredValue.x - filteredValue.y;
				_angle.y = filteredValue.z - filteredValue.w;
				_height = (filteredValue.x + filteredValue.y + filteredValue.z + filteredValue.w) / 4f;

				_angle *= scaleAngle;
				_height *= scaleHeight;
			}
		}

		public Vector2 angle
		{
			get
			{
				return _angle;
			}
		}
		
		public float height
		{
			get
			{
				return _height;
			}
		}
	}
}
