package com.robolink.unityrobot;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.UUID;
import java.util.Set;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.content.Context;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;

public class Bluetooth
{
	// Member fields
	private final BluetoothAdapter mAdapter;
	private final Handler mHandler;
	private ConnectThread mConnectThread;
	private ConnectedThread mConnectedThread;
	private boolean _isConnected = false;
	private static final UUID MY_UUID = UUID.fromString("00001101-0000-1000-8000-00805F9B34FB");
	private byte[] _rcvBuffer = new byte[4096];
	private int _numRcvedData = 0;

 	public Bluetooth(Context context, Handler handler)
	{
		mAdapter = BluetoothAdapter.getDefaultAdapter();
		mHandler = handler;
	}
		
	public synchronized void Search()
    {
        try
        {
            Set<BluetoothDevice> pairedDevices = mAdapter.getBondedDevices();
            if (pairedDevices.size() > 0)
            {
                for (BluetoothDevice bd : pairedDevices)
                	SendMessage(AndroidPlugInActivity.MSG_BT_FOUND_DEVICE, AndroidPlugInActivity.MSGKEY_BT_DEVICE, bd.getName());
            }
            
            SendMessage(AndroidPlugInActivity.MSG_BT_SEARCH_COMPLETED);
        }
        catch (Exception e)
        {
        	SendMessage(AndroidPlugInActivity.MSG_EXCEPTION, AndroidPlugInActivity.MSGKEY_EXCEPTION, e.toString());
        }
    }

	public boolean IsConnected()
	{
		return _isConnected;
	}
	
	public synchronized void Connect(String deviceName)
	{
        Set<BluetoothDevice> pairedDevices = mAdapter.getBondedDevices();
        for (BluetoothDevice bd : pairedDevices)
        {
            if (bd.getName().equalsIgnoreCase(deviceName))
            {
        		// Cancel any thread currently running a connection
        		if (mConnectedThread != null)
        		{
        			mConnectedThread.cancel();
        			mConnectedThread = null;
        		}

        		// Start the thread to connect with the given device
        		mConnectThread = new ConnectThread(bd);
        		mConnectThread.start();
        		_isConnected = false;
        		SendMessage(AndroidPlugInActivity.MSG_BT_CONNECTING);
                return;
            }
        }

        SendMessage(AndroidPlugInActivity.MSG_BT_CONNECT_FAIL);
	}
	
	public synchronized void Disconnect()
	{
		_isConnected = false;
		
		if (mConnectThread != null)
		{
			mConnectThread.cancel();
			mConnectThread = null;
		}
		if (mConnectedThread != null)
		{
			mConnectedThread.cancel();
			mConnectedThread = null;
		}		
	}
	
	public void ClearBuffer()
	{
		_numRcvedData = 0;
	}	

	private void SendMessage(int type)
	{
		mHandler.sendMessage(mHandler.obtainMessage(type));	
	}
	
	private void SendMessage(int type, String argKey, String argData)
	{
		Message msg = mHandler.obtainMessage(type);
		Bundle bundle = new Bundle();
		bundle.putString(argKey, argData);
		msg.setData(bundle);
		mHandler.sendMessage(msg);	
	}

	private synchronized void Connected(BluetoothSocket socket,	BluetoothDevice device)
	{
		// Cancel the thread that completed the connection
		if (mConnectThread != null)
		{
			mConnectThread.cancel();
			mConnectThread = null;
		}

		// Cancel any thread currently running a connection
		if (mConnectedThread != null)
		{
			mConnectedThread.cancel();
			mConnectedThread = null;
		}

		// Start the thread to manage the connection and perform transmissions
		mConnectedThread = new ConnectedThread(socket);
		mConnectedThread.start();
		
		ClearBuffer();
		_isConnected = true;
		SendMessage(AndroidPlugInActivity.MSG_BT_CONNECTED);
	}

	public void Write(byte[] data)
	{
		// Create temporary object
		ConnectedThread r;
		// Synchronize a copy of the ConnectedThread
		synchronized (this)
		{
			if (_isConnected == false)
				return;
			r = mConnectedThread;
		}
		// Perform the write unsynchronized
		r.write(data);
	}
	
	public byte[] Read()
	{
		if (_isConnected == false)
			return null;
		
		byte[] data = new byte[_numRcvedData];
		for(int i=0; i<_numRcvedData; i++)
			data[i] = _rcvBuffer[i];
		_numRcvedData = 0;
		return data;
	}
	
	private class ConnectThread extends Thread
	{
		private final BluetoothSocket mmSocket;
		private final BluetoothDevice mmDevice;

		public ConnectThread(BluetoothDevice device)
		{
			mmDevice = device;
			BluetoothSocket tmp = null;

			// Get a BluetoothSocket for a connection with the
			// given BluetoothDevice
			try
			{
				tmp = device.createRfcommSocketToServiceRecord(MY_UUID);
			}
			catch (IOException e)
			{
			}
			mmSocket = tmp;
		}

		public void run()
		{
			setName("ConnectThread");

			// Make a connection to the BluetoothSocket
			try
			{
				mmSocket.connect();
			}
			catch (IOException e)
			{

				SendMessage(AndroidPlugInActivity.MSG_BT_CONNECT_FAIL);

				// Close the socket
				try
				{
					mmSocket.close();
				}
				catch (IOException e2)
				{
				}
				return;
			}

			// Reset the ConnectThread because we're done
			synchronized (Bluetooth.this)
			{
				mConnectThread = null;
			}

			// Start the connected thread
			Connected(mmSocket, mmDevice);
		}

		public void cancel()
		{
			try
			{
				mmSocket.close();
			}
			catch (IOException e)
			{
			}
		}
	}

	private class ConnectedThread extends Thread
	{
		private final BluetoothSocket mmSocket;
		private final InputStream mmInStream;
		private final OutputStream mmOutStream;

		public ConnectedThread(BluetoothSocket socket)
		{
			mmSocket = socket;
			InputStream tmpIn = null;
			OutputStream tmpOut = null;

			// Get the BluetoothSocket input and output streams
			try
			{
				tmpIn = socket.getInputStream();
				tmpOut = socket.getOutputStream();
			}
			catch (IOException e)
			{
			}

			mmInStream = tmpIn;
			mmOutStream = tmpOut;
		}
		
		public void run()
		{
            int bytes;

            // Keep listening to the InputStream while connected
            while (true)
            {
                try
                {
                	bytes = mmInStream.available();
                	if(bytes > 0)
                	{
	                	if(bytes <= (_rcvBuffer.length - _numRcvedData))
	                	{
	                		mmInStream.read(_rcvBuffer, _numRcvedData, bytes);
	                		_numRcvedData += bytes;
	                		SendMessage(AndroidPlugInActivity.MSG_BT_RX_ARRIVED);             	
	                	}
                	}
                }
                catch (IOException e)
                {
                	if(_isConnected == true)
                	{
	                	_isConnected = false;
	                	SendMessage(AndroidPlugInActivity.MSG_BT_DISCONNECTED);
                	}
                    break;
                }
            }
		}

		public void write(byte[] buffer)
		{
			try
			{
				mmOutStream.write(buffer);
				SendMessage(AndroidPlugInActivity.MSG_BT_TX_COMPLETED);
			}
			catch (IOException e)
			{
			}
		}

		public void cancel()
		{
			try
			{
				mmSocket.close();
			}
			catch (IOException e)
			{
			}
		}
	}
}
