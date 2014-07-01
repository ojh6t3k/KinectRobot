using UnityEngine;
using System.Collections;
using System;
using UnityRobot;

public class WheelControlTest : MonoBehaviour
{
	public RobotProxy robot;
	public WheelController wheelController;
	
	private string _statusMessage = "Ready";
	private bool _connecting = false;
	private Vector2 _speed;
	
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

			guiRect.width = 200;
			_speed.x = GUI.HorizontalSlider(guiRect, _speed.x, -1, 1);
			guiRect.x += (guiRect.width + 5);
			guiRect.width = 30;
			guiRect.height = 200;
			_speed.y = GUI.VerticalSlider(guiRect, _speed.y, 1, -1);
			guiRect.y += (guiRect.height + 5);
			guiRect.x = 10;
			guiRect.height = 25;

			guiRect.width = 300;
			GUI.Label(guiRect, string.Format("X:{0:f} Y:{1:f}", _speed.x, _speed.y));
			guiRect.y += (guiRect.height + 5);

			guiRect.width = 100;
			if(GUI.Button(guiRect, "Stop") == true)
			{
				_speed.x = 0;
				_speed.y = 0;
			}
			guiRect.y += (guiRect.height + 5);

			wheelController.ControlRect(_speed);
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
		_speed = new Vector2(0, 0);
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
