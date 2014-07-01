using UnityEngine;
using System.Collections;
using System;
using UnityRobot;


public class DigitalModuleTest : MonoBehaviour
{
	public RobotProxy robot;
	public DigitalModule digital;

	private string _statusMessage = "Ready";
	private bool _connecting = false;
	private string[] guiGrid = {"OUTPUT", "INPUT", "INPUT_PULLUP"};
	
	// Use this for initialization
	void Start ()
	{
		robot.OnConnected += OnConnected;
		robot.OnConnectionFailed += OnConnectionFailed;
		robot.OnDisconnected += OnDisconnected;
		robot.OnSearchCompleted += OnSearchCompleted;

		digital.OnRisingEdge += OnRisingEdge;
		digital.OnFallingEdge += OnFallingEdge;
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

			guiRect.width = 350;
			int newMode = GUI.SelectionGrid(guiRect, (int)digital.mode, guiGrid, guiGrid.Length);
			if(newMode == (int)DigitalModule.Mode.OUTPUT)
				digital.mode = DigitalModule.Mode.OUTPUT;
			else if(newMode == (int)DigitalModule.Mode.INPUT)
				digital.mode = DigitalModule.Mode.INPUT;
			else if(newMode == (int)DigitalModule.Mode.INPUT_PULLUP)
				digital.mode = DigitalModule.Mode.INPUT_PULLUP;
			guiRect.y += (guiRect.height + 5);

			if(digital.mode == DigitalModule.Mode.OUTPUT)
			{
				bool output = false;
				if(digital.Value > 0)
					output = true;
				output = GUI.Toggle(guiRect, output, "HIGH/LOW");
				if(output == true)
					digital.Value = 1;
				else
					digital.Value = 0;
			}
			else
			{
				GUI.Label(guiRect, string.Format("Value: {0:d}", digital.Value));
			}
			guiRect.y += (guiRect.height + 5);
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

	void OnRisingEdge(object sender, EventArgs e)
	{
		_statusMessage = "Event rising edge";
	}

	void OnFallingEdge(object sender, EventArgs e)
	{
		_statusMessage = "Event falling edge";
	}
}
