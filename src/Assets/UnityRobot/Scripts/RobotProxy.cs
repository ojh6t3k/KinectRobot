using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;


namespace UnityRobot
{
	public class RobotProxy : MonoBehaviour
	{
		[HideInInspector]
		public List<string> portNames = new List<string>();
		public string portName;

#if UNITY_ANDROID
		private AndroidPlugIn _androidPlugIn;
		private bool _bluetoothArrived = false;
#else
		public int baudrate = 57600;

		private SerialPort _serialPort;
#endif
		private enum CMD
		{
			Start = 0x80, //128
			Exit = 0x81,  //129
			Update = 0x82, //130
			Action = 0x83, //131
			Ready = 0x84, //132
			Ping = 0x85 //133
		}

		public float timeoutSec = 1f;
		public ModuleProxy[] modules = new ModuleProxy[0];

		public EventHandler OnConnected;
		public EventHandler OnConnectionFailed;
		public EventHandler OnDisconnected;
		public EventHandler OnSearchCompleted;
		public EventHandler OnUpdated;

		private bool _opened = false;
		private bool _connected = false;
		private float _time = 0f;
		private float _timeout = 0;
		private bool _processProtocolTx = false;
		private int _processUpdate = 0;
		private byte _id;
		private byte _numData;
		private List<byte> _rxDataBytes = new List<byte>();

		void Awake()
		{
#if UNITY_ANDROID
			GameObject go = new GameObject();
			go.name = "AndroidPlugIn";
			go.transform.parent = this.transform;
			_androidPlugIn = go.AddComponent<AndroidPlugIn>();
			if(_androidPlugIn != null)
			{
				_androidPlugIn.OnConnected += OnBluetoothConnected;
				_androidPlugIn.OnConnecting += OnBluetoothConnecting;
				_androidPlugIn.OnConnectFail += OnBluetoothFail;
				_androidPlugIn.OnDisconnected += OnBluetoothDisconnected;
				_androidPlugIn.OnSearchCompleted += OnBluetoothSearchCompleted;
				_androidPlugIn.OnFoundDevice += OnBluetoothFoundDevice;
				_androidPlugIn.OnTxCompleted += OnBluetoothTxCompleted;
				_androidPlugIn.OnRxArrived += OnBluetoothRxArrived;
			}
#else
			_serialPort = new SerialPort();
			_serialPort.DtrEnable = true; // win32 hack to try to get DataReceived event to fire
			_serialPort.RtsEnable = true;
			_serialPort.DataBits = 8;
			_serialPort.Parity = Parity.None;
			_serialPort.StopBits = StopBits.One;
			_serialPort.ReadTimeout = 1; // since on windows we *cannot* have a separate read thread
			_serialPort.WriteTimeout = 1000;
#endif
		}

		// Use this for initialization
		void Start ()
		{
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(_opened == true)
			{
				// Process RX
				byte[] readBytes = Read();
				bool update = false;
				if(readBytes != null)
				{
					for(int i=0; i<readBytes.Length; i++)
					{
						if(_connected == false)
						{
							if(readBytes[i] == (byte)CMD.Ping)
							{
								Write(new byte[] { (byte)CMD.Start, (byte)CMD.Ready });
								foreach(ModuleProxy module in modules)
									module.Reset();
								
								TimeoutReset();
								_connected = true;
								_processProtocolTx = true;
								if(OnConnected != null)
									OnConnected(this, null);
							}
						}
						else
						{
							if(readBytes[i] == (byte)CMD.Ping)
							{
								TimeoutReset();
								_processUpdate = 0;
							}
							else if(readBytes[i] == (byte)CMD.Ready)
							{
								TimeoutReset();
								_processUpdate = 0;
								_processProtocolTx = true;
							}
							else if(readBytes[i] == (byte)CMD.Update)
							{
								TimeoutReset();
								_processUpdate = 1;
							}
							else if(readBytes[i] == (byte)CMD.Action)
							{
								TimeoutReset();
								if(_processUpdate > 0)
								{
									foreach(ModuleProxy module in modules)
										module.Action();

									update = true;
								}
								_processUpdate = 0;
							}
							else if(_processUpdate > 0 && readBytes[i] < 0x80)
							{
								TimeoutReset();
								if(_processUpdate == 1)
								{
									_id = readBytes[i];
									_processUpdate = 2;
								}
								else if(_processUpdate == 2)
								{
									_numData = readBytes[i];
									_processUpdate = 3;
									_rxDataBytes.Clear();
								}
								else if(_processUpdate == 3)
								{
									if(_rxDataBytes.Count < _numData)
										_rxDataBytes.Add(readBytes[i]);
									
									if(_rxDataBytes.Count >= _numData)
									{
										// Decoding 7bit bytes
										byte bit = 1;
										for(int j=0; j<_rxDataBytes.Count; j++)
										{
											if(bit == 1)
												_rxDataBytes[j] = (byte)(_rxDataBytes[j] << bit);
											else if(bit == 8)
											{
												_rxDataBytes[j -1] |= _rxDataBytes[j];
												_rxDataBytes.RemoveAt(j);
												j--;
												bit = 1;
											}
											else
											{
												_rxDataBytes[j - 1] |= (byte)(_rxDataBytes[j] >> (7 - bit + 1));
												if(j == (_rxDataBytes.Count - 1))
													_rxDataBytes.RemoveAt(j);
												else
													_rxDataBytes[j] = (byte)(_rxDataBytes[j] << bit);
											}
											bit++;
										}

										foreach(ModuleProxy module in modules)
										{
											if(module.id == _id)
												module.dataBytes = _rxDataBytes.ToArray();
										}
										_processUpdate = 1;
									}
								}
							}
							else
								_processUpdate = 0;
						}
					}

					if(update == true)
					{
						if(OnUpdated != null)
							OnUpdated(this, null);
						Write(new byte[] { (byte)CMD.Ready });
					}
				}

				// Process TX
				if(_connected == true)
				{
					if(_processProtocolTx == true)
					{
						if(modules.Length > 0)
						{
							List<byte> writeBytes = new List<byte>();
							foreach(ModuleProxy module in modules)
							{
								byte[] dataBytes = module.dataBytes;
								if(dataBytes != null)
								{
									writeBytes.Add((byte)(module.id & 0x7F));
									
									// Encoding 7bit bytes
									List<byte> data7bitBytes = new List<byte>();
									byte bit = 1;
									byte temp = 0;
									for(int i=0; i<dataBytes.Length; i++)
									{
										data7bitBytes.Add((byte)((temp | (dataBytes[i] >> bit)) & 0x7F));
										if(bit == 7)
										{
											data7bitBytes.Add((byte)(dataBytes[i] & 0x7F));
											bit = 1;
											temp = 0;
										}
										else
										{
											temp = (byte)(dataBytes[i] << (7 - bit));
											if(i == (dataBytes.Length - 1))
												data7bitBytes.Add((byte)(temp & 0x7F));
										}
										bit++;
									}

									writeBytes.Add((byte)data7bitBytes.Count); // num bytes
									writeBytes.AddRange(data7bitBytes.ToArray());
								}
							}
							
							if(writeBytes.Count > 0)
							{
								writeBytes.Insert(0, (byte)CMD.Update); // Update
								writeBytes.Add((byte)CMD.Action); // Action
								Write (writeBytes.ToArray());
							}
							else
								Write(new byte[] { (byte)CMD.Update, (byte)CMD.Action });

							_processProtocolTx = false;
						}
					}
				}

				// try reconnection
				if(_time > 0.5f) // per time
				{
					_time = 0;
					if(_connected == false)
						Write(new byte[] { (byte)CMD.Ping });
					else
						Write(new byte[] { (byte)CMD.Ready, (byte)CMD.Update, (byte)CMD.Action });
				}
				else
					_time += Time.deltaTime;

				// Check timeout
				if(_timeout > timeoutSec) // wait until timeout seconds
				{
					if(_connected == false)
					{
						Disconnect();
						if(OnConnectionFailed != null)
							OnConnectionFailed(this, null);
					}
					else
						ErrorDisconnect();
				}
				else
					_timeout += Time.deltaTime;
			}
		}

		private void TimeoutReset()
		{
			_time = 0;
			_timeout = 0;
		}
		
		public bool Connected
		{
			get
			{
				return _connected;
			}
		}

		public void Connect()
		{
			_processUpdate = 0;
			_connected = false;
			_opened = false;

#if UNITY_ANDROID
			_bluetoothArrived = false;
			_androidPlugIn.ConnectBluetooth(portName);
#else
			try
			{
				_serialPort.PortName = portName;
				_serialPort.BaudRate = baudrate;
				_serialPort.Open();
				if(_serialPort.IsOpen == true)
				{
					_opened = true;
					TimeoutReset();
					Write(new byte[] { (byte)CMD.Ping });
				}
				else
				{
					if(OnConnectionFailed != null)
						OnConnectionFailed(this, null);
				}
			}
			catch(Exception)
			{
				if(OnConnectionFailed != null)
					OnConnectionFailed(this, null);
			}
#endif
		}

		private void ErrorDisconnect()
		{
			_connected = false;
			_opened = false;
			
#if UNITY_ANDROID
			_androidPlugIn.DisconnectBluetooth();
#else
			try
			{
				_serialPort.Close();
			}
			catch(Exception)
			{
			}
#endif
			if(OnDisconnected != null)
				OnDisconnected(this, null);
		}

		public void Disconnect()
		{
			if(_connected == true)
				Write(new byte[] { (byte)CMD.Exit });

			_connected = false;
			_opened = false;

#if UNITY_ANDROID
			_androidPlugIn.DisconnectBluetooth();
#else
			try
			{
				_serialPort.Close();
			}
			catch(Exception)
			{
			}
#endif
		}

		public void PortSearch()
		{
#if UNITY_ANDROID
			portNames.Clear();
			_androidPlugIn.SearchBluetooth();
#else
			portNames.Clear();
			portNames.AddRange(SerialPort.GetPortNames());
			if(OnSearchCompleted != null)
				OnSearchCompleted(this, null);
#endif
		}

		private byte[] Read()
		{
			List<byte> bytes = new List<byte>();
#if UNITY_ANDROID
			if(_bluetoothArrived == false)
				return null;

			bytes.AddRange(_androidPlugIn.ReadBluetooth());
			_bluetoothArrived = false;
#else
			while(true)
			{			
				try
				{
					bytes.Add((byte)_serialPort.ReadByte());
				}
				catch(TimeoutException)
				{
					break;
				}
				catch(Exception)
				{
					ErrorDisconnect();
					return null;
				}
			}
#endif
			if(bytes.Count == 0)
				return null;
			else
				return bytes.ToArray();
		}
		
		private void Write(byte[] bytes)
		{
			if(bytes == null)
				return;
			if(bytes.Length == 0)
				return;

#if UNITY_ANDROID
			_androidPlugIn.WriteBluetooth(bytes);
#else
			try
			{
				_serialPort.Write(bytes, 0, bytes.Length);
			}
			catch(Exception)
			{
				ErrorDisconnect();
			}
#endif
		}

#if UNITY_ANDROID
		private void OnBluetoothConnecting(object sender, EventArgs e)
		{
		}

		private void OnBluetoothConnected(object sender, EventArgs e)
		{
			_opened = true;
			TimeoutReset();
			Write(new byte[] { (byte)CMD.Ping });
		}

		private void OnBluetoothFail(object sender, EventArgs e)
		{
			if(OnConnectionFailed != null)
				OnConnectionFailed(this, null);
		}

		private void OnBluetoothDisconnected(object sender, EventArgs e)
		{
			ErrorDisconnect();
		}

		private void OnBluetoothSearchCompleted(object sender, EventArgs e)
		{
			if(OnSearchCompleted != null)
				OnSearchCompleted(this, null);
		}

		private void OnBluetoothTxCompleted(object sender, EventArgs e)
		{
		}

		private void OnBluetoothRxArrived(object sender, EventArgs e)
		{
			_bluetoothArrived = true;
		}

		private void OnBluetoothFoundDevice(object sender, FoundDeviceEventArgs e)
		{
			portNames.Add(e.deviceName);
		}
#endif
	}
}
