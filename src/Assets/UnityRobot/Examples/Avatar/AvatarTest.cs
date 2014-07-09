using UnityEngine;
using System.Collections;
using System;
using UnityRobot;


public class AvatarTest : MonoBehaviour
{
	public RobotProxy robot;
	public Animation animation;
	public AvatarJoint[] joints;

	private string _statusMessage = "Ready";
	private bool _connecting = false;
	private Vector2 _scrollPos;

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
				foreach(AvatarJoint joint in joints)
					joint.follow = false;
			}
			guiRect.y += (guiRect.height + 5);
			
			guiRect.width = 100;
			guiRect.height = 250;
			GUILayout.BeginArea(guiRect);
			GUILayout.BeginScrollView(_scrollPos);

			GUI.enabled = !animation.isPlaying;
			foreach(AnimationState clip in animation)
			{
				if(GUILayout.Button(clip.name) == true)
				{
					animation.Play(clip.name);				
				}
			}
			GUI.enabled = true;

			GUILayout.EndScrollView();			
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

		foreach(AvatarJoint joint in joints)
			joint.follow = true;
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

		foreach(AvatarJoint joint in joints)
			joint.follow = false;
	}
	
	void OnSearchCompleted(object sender, EventArgs e)
	{
		_statusMessage = "Search completed";
	}
}
