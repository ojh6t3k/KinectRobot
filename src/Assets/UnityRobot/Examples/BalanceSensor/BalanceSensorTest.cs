using UnityEngine;
using System.Collections;
using System;
using UnityRobot;


public class BalanceSensorTest : MonoBehaviour
{
	public RobotProxy robot;
	public BalanceSensor balance;
	public GameObject target;

	public ADCModule pxAxis;
	public ADCModule mxAxis;
	public ADCModule pyAxis;
	public ADCModule myAxis;
	
	private string _statusMessage = "Ready";
	private bool _connecting = false;
	Quaternion _quaternion;
	Vector3 _position;
	
	// Use this for initialization
	void Start ()
	{
		robot.OnConnected += OnConnected;
		robot.OnConnectionFailed += OnConnectionFailed;
		robot.OnDisconnected += OnDisconnected;
		robot.OnSearchCompleted += OnSearchCompleted;

		balance.enabled = false;
		_position = target.transform.position;
		_quaternion = target.transform.rotation;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(balance.enabled == true && target != null)
		{
			target.transform.rotation = _quaternion * Quaternion.AngleAxis(balance.angle.x, Vector3.forward) * Quaternion.AngleAxis(-balance.angle.y, Vector3.right);
			target.transform.position = _position + Vector3.up * balance.height;
		}
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
				balance.enabled = false;
			}
			guiRect.y += (guiRect.height + 5);

			guiRect.width = 500;
			guiRect.height = 250;
			GUILayout.BeginArea(guiRect);
			
			GUILayout.BeginHorizontal();
			balance.sensitivity = GUILayout.HorizontalSlider(balance.sensitivity, 0f, 1f);
			GUILayout.Label(string.Format("Sensitivity: {0:f}", balance.sensitivity));
			GUILayout.EndHorizontal();
			GUILayout.Label(string.Format("Angle: {0:f}, {1:f}", balance.angle.x, balance.angle.y));
			GUILayout.Label(string.Format("Height: {0:f}", balance.height));

			GUILayout.Label(string.Format("px: {0:d}", pxAxis.Value));
			GUILayout.Label(string.Format("mx: {0:d}", mxAxis.Value));
			GUILayout.Label(string.Format("py: {0:d}", pyAxis.Value));
			GUILayout.Label(string.Format("my: {0:d}", myAxis.Value));
			
			GUILayout.EndArea();
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
		balance.enabled = true;
	}
	
	void OnConnectionFailed(object sender, EventArgs e)
	{
		_statusMessage = "Failed to conncet";
		_connecting = false;
		balance.enabled = false;
	}
	
	void OnDisconnected(object sender, EventArgs e)
	{
		_statusMessage = "Disconnected";
		_connecting = false;
		balance.enabled = false;
	}
	
	void OnSearchCompleted(object sender, EventArgs e)
	{
		_statusMessage = "Search completed";
	}
}
