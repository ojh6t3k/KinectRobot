using UnityEngine;
using System.Collections;
using System;
using UnityRobot;


public class ColorSensorTest : MonoBehaviour
{
	public RobotProxy robot;
	public MeshRenderer meshRenderer;
	public ADCModule r_color;
	public ADCModule g_color;
	public ADCModule b_color;

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

			int r = 1024 - r_color.Value;
			guiRect.width = 300;
			GUI.Label(guiRect, string.Format("R color: {0:d}", r));
			guiRect.y += (guiRect.height + 5);

			int g = 1024 - g_color.Value;
			guiRect.width = 300;
			GUI.Label(guiRect, string.Format("G color: {0:d}", g));
			guiRect.y += (guiRect.height + 5);

			int b = 1024 - b_color.Value;
			guiRect.width = 300;
			GUI.Label(guiRect, string.Format("B color: {0:d}", b));
			guiRect.y += (guiRect.height + 5);

			meshRenderer.sharedMaterial.color = new Color(r / 1024f, g / 1024f, b / 1024f);
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
		}
		
		guiRect.width = 300;
		GUI.Label(guiRect, _statusMessage);
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
