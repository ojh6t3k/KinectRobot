using UnityEngine;
using System.Collections;
using System;


namespace UnityRobot
{
	[Serializable]
	public class DistanceSmaple
	{
		public int value;
		public float distance;
	}

	public class DistanceSensor : ADCModule
	{
		public DistanceSmaple[] distanceSamples;

		public float Distance
		{
			get
			{
				float value_a = 0;
				float value_b = 0;
				float dist_a = 0;
				float dist_b = 0;
				for(int i=0; i<distanceSamples.Length; i++)
				{
					if(i == 0 && _value < distanceSamples[i].value)
					{
						value_a = distanceSamples[i].value;
						value_b = distanceSamples[i].value;
						dist_a = distanceSamples[i].distance;
						dist_b = distanceSamples[i].distance;
						break;
					}
					else if(i == distanceSamples.Length - 1 && _value >= distanceSamples[i].value)
					{
						value_a = distanceSamples[i].value;
						value_b = distanceSamples[i].value;
						dist_a = distanceSamples[i].distance;
						dist_b = distanceSamples[i].distance;
						break;
					}
					else
					{
						if(_value >= distanceSamples[i].value && _value < distanceSamples[i+1].value)
						{
							value_a = distanceSamples[i].value;
							value_b = distanceSamples[i+1].value;
							dist_a = distanceSamples[i].distance;
							dist_b = distanceSamples[i+1].distance;
							break;
						}
					}
				}

				float distance = 0;
				if(dist_a == dist_b)
					distance = dist_a;
				else
				{
					float a = (dist_a - dist_b) / (value_a - value_b);
					distance = a * (_value - value_b) + dist_b;
				}

				return distance;
			}
		}
	}
}

