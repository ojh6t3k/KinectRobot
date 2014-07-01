using UnityEngine;
using System.Collections;
using System;
using UnityRobot;

public class MyModuleTest : MonoBehaviour
{
	public RobotProxy robot;
	public MyModule myModule;
	
	private string _statusMessage = "Ready";
	private bool _connecting = false;
	
	// Use this for initialization
	void Start ()
	{
		robot.OnConnected += OnConnected;
		robot.OnConnectionFailed += OnConnectionFailed;
		robot.OnDisconnected += OnDisconnected;
		robot.OnSearchCompleted += OnSearchCompleted;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	void OnGUI()
	{
		Rect guiRect = new Rect(10, 10, 0, 25);
		
		if(robot.Connected == true)
		{
			guiRect.width = 100;
			if(GUI.Button(guiRect, "Disconnect") == true)
			{
				_statusMessage = "Disconnected";
				robot.Disconnect();
			}
			guiRect.y += (guiRect.height + 5);

			guiRect.width = 300;
			guiRect.height = 200;
			GUILayout.BeginArea(guiRect);

			int value;
			GUILayout.BeginHorizontal();
			GUILayout.Label("In byte : ");
			value = (int)myModule.input_byte;
			value = (int)GUILayout.HorizontalSlider(value, 0, 10);
			myModule.input_byte = (byte)value;
			GUILayout.Label(myModule.input_byte.ToString());
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("In ushort : ");
			value = (int)myModule.input_ushort;
			value = (int)GUILayout.HorizontalSlider(value, 0, 10);
			myModule.input_ushort = (ushort)value;
			GUILayout.Label(myModule.input_ushort.ToString());
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("In short : ");
			value = (int)myModule.input_short;
			value = (int)GUILayout.HorizontalSlider(value, -10, 10);
			myModule.input_short = (short)value;
			GUILayout.Label(myModule.input_short.ToString());
			GUILayout.EndHorizontal();

			GUILayout.Label(string.Format("Out byte: {0:d}", myModule.output_byte.ToString()));
			GUILayout.Label(string.Format("Out ushort: {0:d}", myModule.output_ushort.ToString()));
			GUILayout.Label(string.Format("Out short: {0:d}", myModule.output_short.ToString()));
			GUILayout.EndArea();
		}
		else
		{
			if(_connecting == false)
			{
				guiRect.width = 100;
				if(GUI.Button(guiRect, "Search") == true)
				{
					_statusMessage = "Searching...";
					robot.PortSearch();
				}
				guiRect.y += (guiRect.height + 5);
				
				if(robot.portNames.Count > 0)
				{
					guiRect.width = 100;
					for(int i=0; i<robot.portNames.Count; i++)
					{
						if(GUI.Button(guiRect, robot.portNames[i]) == true)
						{
							_statusMessage = "Connecting...";
							_connecting = true;
							robot.portName = robot.portNames[i];
							robot.Connect();
						}
						guiRect.x += (100 + 5);
					}
					guiRect.x = 10;
					guiRect.y += (guiRect.height + 5);
				}
			}

			guiRect.width = 300;
			GUI.Label(guiRect, _statusMessage);
		}
	}
	
	void OnConnected(object sender, EventArgs e)
	{
		_statusMessage = "Success to conncet";
		_connecting = false;
	}
	
	void OnConnectionFailed(object sender, EventArgs e)
	{
		_statusMessage = "Failed to conncet";
		_connecting = false;
	}
	
	void OnDisconnected(object sender, EventArgs e)
	{
		_statusMessage = "Disconnected";
		_connecting = false;
	}
	
	void OnSearchCompleted(object sender, EventArgs e)
	{
		_statusMessage = "Search completed";
	}
}
