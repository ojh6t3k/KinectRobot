using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityRobot;

[Serializable]
public class ToneTrack
{
	public string instrumentName;
	public ToneModule tone;
	public bool mute = false;

	private MidiTrack _midiTrack;
	private int _noteIndex = 0;
	private ToneNote _lastToneNote;

	public ToneTrack(MidiTrack midiTrack)
	{
		_midiTrack = midiTrack;
		if(_midiTrack != null)
			instrumentName = _midiTrack.InstrumentName;
		
		Reset();
	}

	public void Reset()
	{
		_noteIndex = 0;
		_lastToneNote = ToneNote.MUTE;
		if(tone != null)
			tone.Note = ToneNote.MUTE;
	}

	public void Process(int pulse)
	{
		if (_noteIndex < _midiTrack.Notes.Count)
		{
			MidiNote note = _midiTrack.Notes[_noteIndex];
			if(pulse >= note.EndTime)
			{
				_noteIndex++;
				_lastToneNote = ToneNote.MUTE;
			}
			else if (pulse >= note.StartTime)
				_lastToneNote = NoteToTone(note.Number);

			if(tone != null && mute == false)
				tone.Note = _lastToneNote;
		}
	}

	public ToneNote lastToneNote
	{
		get
		{
			return _lastToneNote;
		}
	}

	public static ToneNote NoteToTone(int note)
	{
		ToneNote toneNote = ToneNote.MUTE;

		switch(note)
		{
		case 36:
			toneNote = ToneNote.C3;
			break;
			
		case 37:
			toneNote = ToneNote.CS3;
			break;
			
		case 38:
			toneNote = ToneNote.D3;
			break;
			
		case 39:
			toneNote = ToneNote.DS3;
			break;
			
		case 40:
			toneNote = ToneNote.E3;
			break;
			
		case 41:
			toneNote = ToneNote.F3;
			break;
			
		case 42:
			toneNote = ToneNote.FS3;
			break;
			
		case 43:
			toneNote = ToneNote.G3;
			break;
			
		case 44:
			toneNote = ToneNote.GS3;
			break;
			
		case 45:
			toneNote = ToneNote.A3;
			break;
			
		case 46:
			toneNote = ToneNote.AS3;
			break;
			
		case 47:
			toneNote = ToneNote.B3;
			break;
			
		case 48:
			toneNote = ToneNote.C4;
			break;
			
		case 49:
			toneNote = ToneNote.CS4;
			break;
			
		case 50:
			toneNote = ToneNote.D4;
			break;
			
		case 51:
			toneNote = ToneNote.DS4;
			break;
			
		case 52:
			toneNote = ToneNote.E4;
			break;
			
		case 53:
			toneNote = ToneNote.F4;
			break;
			
		case 54:
			toneNote = ToneNote.FS4;
			break;
			
		case 55:
			toneNote = ToneNote.G4;
			break;
			
		case 56:
			toneNote = ToneNote.GS4;
			break;
			
		case 57:
			toneNote = ToneNote.A4;
			break;
			
		case 58:
			toneNote = ToneNote.AS4;
			break;
			
		case 59:
			toneNote = ToneNote.B4;
			break;
			
		case 60: // Middle C
			toneNote = ToneNote.C5;
			break;
			
		case 61:
			toneNote = ToneNote.CS5;
			break;
			
		case 62:
			toneNote = ToneNote.D5;
			break;
			
		case 63:
			toneNote = ToneNote.DS5;
			break;
			
		case 64:
			toneNote = ToneNote.E5;
			break;
			
		case 65:
			toneNote = ToneNote.F5;
			break;
			
		case 66:
			toneNote = ToneNote.FS5;
			break;
			
		case 67:
			toneNote = ToneNote.G5;
			break;
			
		case 68:
			toneNote = ToneNote.GS5;
			break;
			
		case 69:
			toneNote = ToneNote.A5;
			break;
			
		case 70:
			toneNote = ToneNote.AS5;
			break;
			
		case 71:
			toneNote = ToneNote.B5;
			break;
			
		case 72:
			toneNote = ToneNote.C6;
			break;
			
		case 73:
			toneNote = ToneNote.CS6;
			break;
			
		case 74:
			toneNote = ToneNote.D6;
			break;
			
		case 75:
			toneNote = ToneNote.DS6;
			break;
			
		case 76:
			toneNote = ToneNote.E6;
			break;
			
		case 77:
			toneNote = ToneNote.F6;
			break;
			
		case 78:
			toneNote = ToneNote.FS6;
			break;
			
		case 79:
			toneNote = ToneNote.G6;
			break;
			
		case 80:
			toneNote = ToneNote.GS6;
			break;
			
		case 81:
			toneNote = ToneNote.A6;
			break;
			
		case 82:
			toneNote = ToneNote.AS6;
			break;
			
		case 83:
			toneNote = ToneNote.B6;
			break;		
		}

		return toneNote;
	}
}


public class MidiPlayer : MonoBehaviour
{
	public TextAsset midi;
	public ToneModule tone;
	public ToneTrack[] tracks;

	private MidiFile _midiFile;
	private bool _bIsPlay = false;
	private float _pulseTime = 0f;
	private float _totalTime = 0f;
	private float _time = 0f;
	private ToneNote _singleToneNote;

	void Awake()
	{
		Load();
	}

	// Use this for initialization
	void Start()
	{	
	}
	
	// Update is called once per frame
	void Update()
	{
		if(_bIsPlay == true)
		{
			if(_time < _totalTime)
			{
				int pulse = (int)(_time / _pulseTime);
				_time += Time.deltaTime;

				foreach(ToneTrack track in tracks)
					track.Process(pulse);

				if(tone != null)
				{
					if(tracks.Length == 1)
					{
						_singleToneNote = tracks[0].lastToneNote;
					}
					else
					{
						ToneNote finalToneNote = ToneNote.MUTE;
						foreach(ToneTrack track in tracks)
						{
							ToneNote eachToneNote = track.lastToneNote;
							if(track.tone == null && track.mute == false)
							{
								if(finalToneNote != eachToneNote)
								{
									if(finalToneNote == ToneNote.MUTE)
										finalToneNote = eachToneNote;
									else if(eachToneNote != ToneNote.MUTE)
										finalToneNote = eachToneNote;
								}
							}
						}

						if(_singleToneNote != finalToneNote)
							_singleToneNote = finalToneNote;
					}
					tone.Note = _singleToneNote;
				}
			}
			else
			{
				_bIsPlay = false;
				foreach(ToneTrack track in tracks)
					track.Reset();

				if(tone != null)
					tone.Note = ToneNote.MUTE;
			}
		}
	}

	public void Load()
	{
		if(midi == null)
			return;

		_midiFile = new MidiFile(midi.bytes);
		tracks = new ToneTrack[_midiFile.Tracks.Count];

		for(int i=0; i<_midiFile.Tracks.Count; i++)
			tracks[i] = new ToneTrack(_midiFile.Tracks[i]);

		_bIsPlay = false;
		_pulseTime = (float)_midiFile.Time.Tempo / (float)_midiFile.Time.Quarter; // microsec
		_pulseTime /= 1000000f; //sec
		_totalTime = _midiFile.TotalPulses * _pulseTime;
		_time = 0f;
	}

	public bool isPlaying
	{
		get
		{
			return _bIsPlay;
		}
	}

	public float currentTime
	{
		get
		{
			return _time;
		}
	}

	public float totalTime
	{
		get
		{
			return _totalTime;
		}
	}

	public void Play()
	{
		if(tracks.Length == 0)
			return;

		_bIsPlay = true;
		_time = 0f;
		_singleToneNote = ToneNote.MUTE;

		foreach(ToneTrack track in tracks)
			track.Reset();
	}

	public void Pause()
	{
		_bIsPlay = false;
	}

	public void Resume()
	{
		_bIsPlay = true;
	}

	public void Stop()
	{
		_time = _totalTime;
	}
}
