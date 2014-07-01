using UnityEngine;
using System.Collections;
using System;


namespace UnityRobot
{
	public class FoundDeviceEventArgs : EventArgs
	{
		public string deviceName = "";
	}

	public class AndroidPlugIn : MonoBehaviour
	{
		public event EventHandler OnConnecting;
		public event EventHandler OnConnected;
		public event EventHandler OnConnectFail;
		public event EventHandler OnDisconnected;
		public event EventHandler OnSearchCompleted;
		public event EventHandler OnTxCompleted;
		public event EventHandler OnRxArrived;
		public event EventHandler<FoundDeviceEventArgs> OnFoundDevice;

	#if UNITY_ANDROID && !UNITY_EDITOR
		private AndroidJavaObject _activity;

		void Awake()
		{
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			_activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
		}

		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
		}

		#region Bluetooth Methods
		public void SearchBluetooth()
		{
			_activity.Call("SearchBluetooth");
		}

		public bool IsConnectedBluetooth()
		{
			return _activity.Call<bool>("IsConnectedBluetooth");
		}

		public void ConnectBluetooth(string deviceName)
		{
			_activity.Call("ConnectBluetooth", deviceName);
		}

		public void DisconnectBluetooth()
		{
			_activity.Call("DisconnectBluetooth");
		}

		public void ClearBufferBluetooth()
		{
			_activity.Call("ClearBufferBluetooth");
		}

		public void WriteBluetooth(byte[] data)
		{
			_activity.Call("WriteBluetooth", data);
		}

		public byte[] ReadBluetooth()
		{
			return _activity.Call<byte[]>("ReadBluetooth");
		}
		#endregion

		#region Bluetooth Events
		private void OnBluetoothDisconnected(string arg)
		{
			if(OnDisconnected != null)
				OnDisconnected(this, null);
		}

		private void OnBluetoothConnecting(string arg)
		{
			if(OnConnecting != null)
				OnConnecting(this, null);
		}

		private void OnBluetoothConnected(string arg)
		{
			if(OnConnected != null)
				OnConnected(this, null);
		}

		private void OnBluetoothConnectFail(string arg)
		{
			if(OnConnectFail != null)
				OnConnectFail(this, null);
		}

		private void OnBluetoothTxCompleted(string arg)
		{
			if(OnTxCompleted != null)
				OnTxCompleted(this, null);
		}

		private void OnBluetoothRxArrived(string arg)
		{
			if(OnRxArrived != null)
				OnRxArrived(this, null);
		}

		private void OnBluetoothFoundDevice(string arg)
		{
			if(OnFoundDevice != null)
			{
				FoundDeviceEventArgs e = new FoundDeviceEventArgs();
				e.deviceName = arg;
				OnFoundDevice(this, e);
			}
		}

		private void OnBluetoothSearchCompleted(string arg)
		{
			if(OnSearchCompleted != null)
				OnSearchCompleted(this, null);
		}
		#endregion
	#else
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
		
		#region Bluetooth Methods
		public void SearchBluetooth()
		{
		}
		
		public bool IsConnectedBluetooth()
		{
			return false;
		}
		
		public void ConnectBluetooth(string deviceName)
		{
		}
		
		public void DisconnectBluetooth()
		{
		}
		
		public void ClearBufferBluetooth()
		{
		}
		
		public void WriteBluetooth(byte[] data)
		{
		}
		
		public byte[] ReadBluetooth()
		{
			return null;
		}
		#endregion
	#endif

		void OnGUI()
		{
		}
	}
}
