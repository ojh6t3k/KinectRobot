using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace UnityRobot
{
	public class ModuleProxy : MonoBehaviour
	{
		public int id;

		private List<byte> _dataBytes = new List<byte>();
		private const int _maxNumBytes = 116;
		protected bool canUpdate = false;

		public byte[] dataBytes
		{
			get
			{
				if(canUpdate == false)
					return null;

				_dataBytes.Clear();
				OnPush();

				if(_dataBytes.Count == 0)
					return null;
				else
				{
					canUpdate = false;
					return _dataBytes.ToArray();
				}
			}
			set
			{
				_dataBytes.Clear();
				_dataBytes.AddRange(value);
				OnPop();
			}
		}

		public virtual void Reset()
		{
		}

		public virtual void Action()
		{
		}

		public virtual void OnPush()
		{
		}

		public virtual void OnPop()
		{
		}

		protected bool Push(byte value)
		{
			if((_maxNumBytes - _dataBytes.Count) < 1)
				return false;

			_dataBytes.Add(value);
			return true;
		}

		protected bool Push(ushort value)
		{
			if((_maxNumBytes - _dataBytes.Count) < 2)
				return false;
			
			_dataBytes.Add((byte)(value & 0xFF));
			_dataBytes.Add((byte)((value >> 8) & 0xFF));
			return true;
		}

		protected bool Push(short value)
		{
			ushort binary = 0;
			if(value < 0)
			{
				value *= -1;
				binary = (ushort)value;
				binary |= (ushort)0x8000;
			}
			else
				binary = (ushort)value;
			
			return Push(binary);
		}

		protected bool Push(byte[] value)
		{
			if((_maxNumBytes - _dataBytes.Count) < value.Length)
				return false;

			_dataBytes.AddRange(value);
			return true;
		}

		protected bool Pop(ref byte value)
		{
			if(_dataBytes.Count < 1)
				return false;
			value = _dataBytes[0];
			_dataBytes.RemoveAt(0);
			return true;
		}
		
		protected bool Pop(ref ushort value)
		{
			if(_dataBytes.Count < 2)
				return false;

			value = (ushort)(((_dataBytes[1] << 8) & 0xFF00) | (_dataBytes[0] & 0xFF));
			_dataBytes.RemoveRange(0, 2);
			return true;
		}
		
		protected bool Pop(ref short value)
		{
			ushort binary = 0;
			if(Pop(ref binary) == false)
				return false;

			value = (short)(binary & 0x7FFF);
			if((binary & 0x8000) == 0x8000)
				value *= -1;
			return true;
		}
		
		protected bool Pop(ref byte[] value, int count)
		{
			if(_dataBytes.Count < count)
				return false;

			value = _dataBytes.GetRange(0, count).ToArray();
			_dataBytes.RemoveRange(0, count);
			return true;
		}
	}
}