using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;


public class DRCwifi : MonoBehaviour
{
	public int imageWidth = 640;
	public int imageHeight = 480;
	public int imageQuality = 80;
	public string text = "";

	private Socket _socket;
	private string _ipAddress = "192.168.123.10";
	private int _port = 6400;
	private Texture2D _image = null;
	private byte[] _sendBuffer = new byte[28]; // Total 28byte (17 + 11)
	private int _sendCount = 0;
	private byte[] _recvBuffer = new byte[4096];
	private int _recvCount = 0;
	private byte[] _headerBuffer = new byte[33]; // Total 33byte (17 + 16)
	private int _headerBufferCount = 0;
	private byte[] _jpgBuffer = new byte[10];
	private int _jpgBufferCount = 0;
	private int _commProcess = 0;
	private int _totalLength = 0;
	private int _totalCount = 0;
	private int _nFrameCount = 0;
	private float _time = 0;

	void Awake()
	{
		// Create Tx Packet
		for(int i=0; i<_sendBuffer.Length; i++)
			_sendBuffer[i] = 0;
		_sendBuffer[0] = 0x55; // Header(0~2)
		_sendBuffer[1] = 0x33; // Header(0~2)
		_sendBuffer[2] = 0xAA; // Header(0~2)
		_sendBuffer[3] = 28; // TotalLength(3~6)
		_sendBuffer[4] = 0; // TotalLength(3~6)
		_sendBuffer[5] = 0; // TotalLength(3~6)
		_sendBuffer[6] = 0; // TotalLength(3~6)
		_sendBuffer[7] = 1; // CmdType(7~8): TYPE_GET_REALTIME_IMG
		_sendBuffer[8] = 0; // CmdType(7~8): TYPE_GET_REALTIME_IMG
		_sendBuffer[9] = 11; // CmdDataLength(9~12)
		_sendBuffer[10] = 0; // CmdDataLength(9~12)
		_sendBuffer[11] = 0; // CmdDataLength(9~12)
		_sendBuffer[12] = 0; // CmdDataLength(9~12)

		_image = new Texture2D(1, 1);
	}

	// Use this for initialization
	void Start ()
	{
		_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		_socket.ReceiveBufferSize = 10240;
		try
		{
			SocketAsyncEventArgs e = new SocketAsyncEventArgs();
			e.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
			e.UserToken = _socket;			
			e.Completed += new EventHandler<SocketAsyncEventArgs>(ConnectCompleted);
			_socket.ConnectAsync(e);
		}
		catch(Exception e)
		{
			Debug.Log(e);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		text = string.Format("[{0:d}] [{1:d}] [{2:d}] [{3:d}/{4:d}]", _nFrameCount, _socket.Connected, _commProcess, _totalCount, _totalLength);

		if(_socket.Connected == true)
		{
			if(_commProcess != 0)
			{
				if(_time >= 0.5f)
				{
					_time = 0;
					Reset();
				}
				else
					_time += Time.deltaTime;
			}
		}
	}

	public bool Connected
	{
		get
		{
			return _socket.Connected;
		}
	}

	public Texture2D image
	{
		get
		{
			if(_socket.Connected == true)
			{
				if(_commProcess == 0)
				{
					if(_jpgBuffer.Length == _jpgBufferCount)
						_image.LoadImage(_jpgBuffer);

					_sendBuffer[17] = (byte)(imageWidth / 8); // width
					_sendBuffer[18] = (byte)(imageHeight / 8); // height
					_sendBuffer[19] = (byte)imageQuality; // quality
					_sendCount = 0;
					_commProcess = 1;
					_time = 0;
					_socket.BeginSend(_sendBuffer, 0, _sendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCompleted), null);
				}

				return _image;
			}

			return null;
		}
	}

	public void Reset()
	{
		if(_socket.Available > 0)
		{
			_commProcess = 4;
			_totalLength = _socket.Available;
		}
		else
		{
			_commProcess = 0;
			_totalLength = 1;
		}
		_totalCount = 0;
		_nFrameCount = 0;
	}

	private void ConnectCompleted(object sender, SocketAsyncEventArgs e)		
	{
		if(_socket.Connected == true)
			Debug.Log("DRC WiFi connected!");
		else
			Debug.Log("DRC WiFi can not connect!");

		Reset();
		_socket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, new AsyncCallback(RecieveCompleted), null);
	}

	private void SendCompleted(IAsyncResult ar)
	{
		try			
		{
			int count = _socket.EndSend(ar);
			if(count > 0)
			{
				if(_commProcess == 1)
				{
					_sendCount += count;
					if( _sendCount == _sendBuffer.Length )				
					{
						_commProcess = 2;
						_totalCount = 0;
						_headerBufferCount = 0;
					}
				}

				if(_sendCount < _sendBuffer.Length)
					_socket.BeginSend(_sendBuffer, _sendCount, _sendBuffer.Length - _sendCount, SocketFlags.None, new AsyncCallback(SendCompleted), null);
			}
		}		
		catch( Exception e )			
		{			
			Debug.Log(e);
		}
	}

	private void RecieveCompleted(IAsyncResult ar)
	{
		_recvCount = _socket.EndReceive(ar);
		if(_recvCount > 0)
		{
			int index = 0;
			_totalCount += _recvCount;
			if(_commProcess == 2)
			{
				for(int i=index; i<_recvCount; i++)
				{
					if(_headerBufferCount == _headerBuffer.Length)
						break;
					else
						_headerBuffer[_headerBufferCount++] = _recvBuffer[index++];
				}

				if(_headerBufferCount == _headerBuffer.Length)
				{
					if(_headerBuffer[0] == 0x55 && _headerBuffer[1] == 0x33 && _headerBuffer[2] == 0xAA)
					{
						//Debug.Log(string.Format("Header: {0:x} {1:x} {2:x}", _headerBuffer[0], _headerBuffer[1], _headerBuffer[2]));
						_totalLength = (int)(_headerBuffer[3] + (_headerBuffer[4] << 8) + (_headerBuffer[5]<< 16) + (_headerBuffer[6] << 24));
						//Debug.Log(string.Format("Total Length: {0:d}", totalLength));
						int ackType = (int)(_headerBuffer[7] + (_headerBuffer[8] << 8));
						//Debug.Log(string.Format("Ack Type: {0:d}", ackType));
						int ackLength = (int)(_headerBuffer[9] + (_headerBuffer[10] << 8) + (_headerBuffer[11]<< 16) + (_headerBuffer[12] << 24));
						//Debug.Log(string.Format("Ack Length: {0:d}", ackLength));
						int rxLength = (int)(_headerBuffer[13] + (_headerBuffer[14] << 8) + (_headerBuffer[15] << 16) + (_headerBuffer[16] << 24));
						//Debug.Log(string.Format("Rx Length: {0:d}", rxLength));
						int width = (int)(_headerBuffer[17] * 8);
						int height = (int)(_headerBuffer[18] * 8);
						//Debug.Log(string.Format("Width:{0:d} Height:{1:d}", width, height));
						int offset = (int)(_headerBuffer[19] + (_headerBuffer[20] << 8) + (_headerBuffer[21]<< 16) + (_headerBuffer[22] << 24));
						//Debug.Log(string.Format("Offset: {0:d}", offset));
						int size = (int)(_headerBuffer[23] + (_headerBuffer[24] << 8) + (_headerBuffer[25]<< 16) + (_headerBuffer[26] << 24));
						//Debug.Log(string.Format("Size: {0:d}", size));
						int jpgLength = (int)(_headerBuffer[27] + (_headerBuffer[28] << 8) + (_headerBuffer[29]<< 16) + (_headerBuffer[30] << 24));
						//Debug.Log(string.Format("Jpg Length: {0:d}", jpgLength));
						int version = _headerBuffer[31];
						int record = _headerBuffer[32];

						if(_jpgBuffer.Length != jpgLength)
							_jpgBuffer = new byte[jpgLength];
							
						_commProcess = 3;
						_jpgBufferCount = 0;
					}
					else
					{
						_commProcess = 4;
						_totalLength = _socket.Available;
						_totalCount = 0;
					}
				}
			}

			if(_commProcess == 3)
			{
				for(int i=index; i<_recvCount; i++)
				{
					if(_jpgBufferCount == _jpgBuffer.Length)
						break;
					else
						_jpgBuffer[_jpgBufferCount++] = _recvBuffer[index++];
				}

				if( _jpgBufferCount == _jpgBuffer.Length )
				{
					_commProcess = 4;
				}
			}

			if(_commProcess == 4)
			{
				if(_totalCount >= _totalLength)
				{
					if(_socket.Available > 0)
					{
						_commProcess = 4;
						_totalLength = _socket.Available;
					}
					else
					{
						_commProcess = 0;
						_totalLength = 1;
						_nFrameCount++;
					}
					_totalCount = 0;
				}
			}
		}

		_socket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, new AsyncCallback(RecieveCompleted), null); 
	}
}
