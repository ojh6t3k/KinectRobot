using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityRobot;


public class MusicPlayerTest : MonoBehaviour
{
	public RobotProxy robot;
	public ToneModule tone;

	private string _statusMessage = "Ready";
	private bool _connecting = false;

	private MidiFile _midiFile;
	private bool _bIsPlay = false;	
	private List<MidiNote> _lstMidiNote = new List<MidiNote>();	
	private float _fMusicTime = 0f;
	private int _nMusicTime = 0;
	private int _nCurNoteCounter = 0;

	
	// Use this for initialization
	void Start ()
	{
		robot.OnConnected += OnConnected;
		robot.OnConnectionFailed += OnConnectionFailed;
		robot.OnDisconnected += OnDisconnected;
		robot.OnSearchCompleted += OnSearchCompleted;

		_midiFile = new MidiFile("D:/GitHub/UnityRobot/unity3d/Assets/UnityRobot/Examples/MusicPlayer/snowfalls.mid");
		Debug.Log(_midiFile);
		_lstMidiNote = _midiFile.Tracks[0].Notes;

	}
	
	// Update is called once per frame
	void Update ()
	{
		if(_bIsPlay == true)
		{
			_fMusicTime = _fMusicTime + Time.deltaTime;
			_nMusicTime = (Int32)(_fMusicTime * 384f);

			if (_lstMidiNote[_nCurNoteCounter].StartTime > _nMusicTime)
				return;
			
			Debug.Log( _lstMidiNote[_nCurNoteCounter].Number );
			switch(_lstMidiNote[_nCurNoteCounter].Number)
			{
			case 60:
				tone.Note = ToneNote.C5;
				break;
			
			case 61:
				tone.Note = ToneNote.CS5;
				break;

			case 62:
				tone.Note = ToneNote.D5;
				break;

			case 63:
				tone.Note = ToneNote.DS5;
				break;

			case 64:
				tone.Note = ToneNote.E5;
				break;

			case 65:
				tone.Note = ToneNote.F5;
				break;

			case 66:
				tone.Note = ToneNote.FS5;
				break;

			case 67:
				tone.Note = ToneNote.G5;
				break;

			case 68:
				tone.Note = ToneNote.GS5;
				break;

			case 69:
				tone.Note = ToneNote.A5;
				break;
			
			case 70:
				tone.Note = ToneNote.AS5;
				break;

			case 71:
				tone.Note = ToneNote.B5;
				break;

			case 72:
				tone.Note = ToneNote.C6;
				break;

			case 73:
				tone.Note = ToneNote.CS6;
				break;

			case 74:
				tone.Note = ToneNote.D6;
				break;

			case 75:
				tone.Note = ToneNote.DS6;
				break;

			default:
				tone.Note = ToneNote.MUTE;
				break;
			}
			
			if (_lstMidiNote.Count-1 == _nCurNoteCounter)
			{
				_bIsPlay = false;
				tone.Note = ToneNote.MUTE;
			}
			
			if (_lstMidiNote.Count-1 > _nCurNoteCounter)
				_nCurNoteCounter ++;
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
			}
			guiRect.y += (guiRect.height + 5);

			GUI.enabled = !_bIsPlay;
			if(GUI.Button(guiRect, "Play") == true)
			{
				_bIsPlay = true;
				_fMusicTime = 0f;
				_nCurNoteCounter = 0;
			}
			guiRect.x += (guiRect.width + 5);

			GUI.enabled = _bIsPlay;
			if(GUI.Button(guiRect, "Stop") == true)
			{
				_bIsPlay = false;
				tone.Note = ToneNote.MUTE;
			}
			guiRect.x = 10;
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
}
