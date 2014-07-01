package com.robolink.unityrobot;

import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;


public class AndroidPlugInActivity extends UnityPlayerActivity
{   
	private String _debugLog = "";
    private static final String _unityTargetName = "AndroidPlugIn";
	private Bluetooth _bluetooth = null;
	
	// The Handler that gets information back from other class
	public static final int MSG_EXCEPTION = 0;
	public static final int MSG_BT_DISCONNECTED = 1;
    public static final int MSG_BT_CONNECTING = 2;
    public static final int MSG_BT_CONNECTED = 3;
    public static final int MSG_BT_CONNECT_FAIL = 4;
    public static final int MSG_BT_TX_COMPLETED = 5;
    public static final int MSG_BT_RX_ARRIVED = 6;
    public static final int MSG_BT_FOUND_DEVICE = 7;
    public static final int MSG_BT_SEARCH_COMPLETED = 8;
    public static final String MSGKEY_EXCEPTION = "Exception";
    public static final String MSGKEY_BT_DEVICE = "BluetoothName";
    private final Handler mHandler = new Handler()
    {
        @Override
        public void handleMessage(Message msg)
        {
            switch (msg.what)
            {
            case MSG_EXCEPTION:
            	_debugLog = msg.getData().getString(MSGKEY_EXCEPTION);
            	break;
            	
            case MSG_BT_DISCONNECTED:
            	_debugLog = "Bluetooth disconnected";
            	UnityPlayer.UnitySendMessage(_unityTargetName, "OnBluetoothDisconnected", "");
            	break;
             	
            case MSG_BT_CONNECTING:
            	_debugLog = "Bluetooth connecting";
            	UnityPlayer.UnitySendMessage(_unityTargetName, "OnBluetoothConnecting", "");
            	break;
            	
            case MSG_BT_CONNECTED:
            	_debugLog = "Bluetooth connected";
            	UnityPlayer.UnitySendMessage(_unityTargetName, "OnBluetoothConnected", "");
            	break;
            	
            case MSG_BT_CONNECT_FAIL:
            	_debugLog = "Bluetooth failed to connect";
            	UnityPlayer.UnitySendMessage(_unityTargetName, "OnBluetoothConnectFail", "");
            	break;
            	
            case MSG_BT_TX_COMPLETED:
            	_debugLog = "Bluetooth tx completed";
            	UnityPlayer.UnitySendMessage(_unityTargetName, "OnBluetoothTxCompleted", "");
            	break;
            	
            case MSG_BT_RX_ARRIVED:
            	_debugLog = "Bluetooth rx arrived";
            	UnityPlayer.UnitySendMessage(_unityTargetName, "OnBluetoothRxArrived", "");
            	break;
            	
            case MSG_BT_FOUND_DEVICE:
            	UnityPlayer.UnitySendMessage(_unityTargetName, "OnBluetoothFoundDevice", msg.getData().getString(MSGKEY_BT_DEVICE));
            	break;
            	
            case MSG_BT_SEARCH_COMPLETED:
            	_debugLog = "Bluetooth search completed";
            	UnityPlayer.UnitySendMessage(_unityTargetName, "OnBluetoothSearchCompleted", "");
            	break;
            }
        }
    };

    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        
        if (_bluetooth == null)
        	_bluetooth = new Bluetooth(this, mHandler);
        
        if (_bluetooth == null)
        	_debugLog = "Fail to create Bluetooth";
        else
        	_debugLog = "Success to create Bluetooth";        	
    }

    @Override
    public void onDestroy()
    {
        super.onDestroy();

       	_bluetooth.Disconnect();
    }
    
    public String GetDebugLog()
    {
    	return _debugLog;
    }

    public void SearchBluetooth()
    {
    	_debugLog = "Search Bluetooth";
    	_bluetooth.Search();
    }
    
    public boolean IsConnectedBluetooth()
    {
    	return _bluetooth.IsConnected();
    }
    
    public void ConnectBluetooth(String deviceName)
    {
    	_bluetooth.Connect(deviceName);        
    }

    public void DisconnectBluetooth()
    {
    	_bluetooth.Disconnect();
    }
    
    public void ClearBufferBluetooth()
    {
    	_bluetooth.ClearBuffer();
    }
    
    public void WriteBluetooth(byte[] data)
    {
    	_bluetooth.Write(data);
    }
    
    public byte[] ReadBluetooth()
    {
    	return _bluetooth.Read(); 
    }
}
