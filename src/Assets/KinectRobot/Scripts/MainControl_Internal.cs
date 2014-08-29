using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityRobot;








public class MainControl_Internal : MonoBehaviour 
{
	public RobotProxy		_RobotProxy;
	public KinectManager	_KinectManager;
	
	bool IsConnect_Robot = false;
	bool IsConnect_Kinect = false;
	
	public bool _bIsDemoPlay = false;

	EMode	_eCurMode = EMode.TITLE;
	string	_statusMessage = "Ready";


	public GameObject		_goBGM;
	public GameObject		_goSnd_Click;
	public GameObject		_goSnd_Success;

	// NGUI ========================
	public GameObject		_goUI_Title;
	public GameObject		_goPnl_Title;
	public GameObject		_goPnl_Option;
	public GameObject		_goPnl_Deco;
	
	public GameObject		_goLbl_Standby;
	public UILabel			_ULbl_Standby;
	public UILabel			_ULbl_StandbyTime;
	
	public GameObject		_goUI_Waiting;
	public GameObject		_goObj_Robot;
	
	
	public UIPopupList		_UIPortList;
	public GameObject		_goBtnConnect;
	public GameObject		_goBtnStart;
	public UILabel			_UILblRobotMessage;
	public UILabel			_UILblKinectMessage;

	public GameObject		_goBtnDemo;
	public GameObject		_goAncCenter;
	public GameObject		_goGestureModeBtn;
	public GameObject		_goAvatarModeBtn;
	
	public GameObject		_goCursor;
	public GameObject		_goWaitingSet;
	public GameObject		_goGestureBtnSet;
	public GameObject[]		_goGestureBtn = new GameObject[10];
	public GameObject		_goLinePerson;
	public GameObject		_goRobot;
	public UILabel			_UILblMessage;
	public UILabel			_UILblTimer;
	String					_strModeMessage = "";
	
	public UILabel			_UILblRepeat_Title;
	public UILabel			_UILblRepeat_Waiting;
	
	public UILabel			_UILblDuration;
	// NGUI ========================
	
	// OPTION & NGUI ========================
	public int		_nOption_StartTime = 1;
	public int		_nOption_Repeat = 1;
	public int		_nOption_Interval = 1;
	int				_nOption_Interval_Include = 1; // 옵션에서 보여질 회차간격-----
	public int		_nOption_Duration = 1;
	public int		_nOption_PlayTime = 10;
	public int		_nOption_Button = 10;
	public int		_nOption_Gesture = 1;
	public int		_nOption_Avatar = 1;
	
	
	public UILabel	_Ulbl_StartTime;
	public UILabel	_Ulbl_Repeat;
	public UILabel	_Ulbl_Interval;
	public UILabel	_Ulbl_Duration;
	public UILabel	_Ulbl_PlayTime;
	public UILabel	_Ulbl_Button;
	public GameObject	_goGestureOn;
	public GameObject	_goGestureOff;
	public GameObject	_goAvatarOn;
	public GameObject	_goAvatarOff;

	public UILabel	_Ulbl_EndTime;
	// OPTION & NGUI ========================
	
	
	int _nCurRepeat = 0; // 현재반복횟수--
	int _nCurDuration = 0;	// 회당 제한시간---
	int _nCurInterval = 0;	// 다음공연까지의 남은시간-
	
	int _nAvatarTimer = 0;
	
	
	public Animation	_Animation;
	
	SimpleJoint[] _SimpleJoints;
	
	
	
	int _nGestureNum = -2;
	
	public List<GameObject> GestureList = new List<GameObject>();
	public List<GameObject> GuideList = new List<GameObject>();
	
	public GameObject		_goSuccessGesture;
	
	public EachGestureJoint		_scr_Arm_L;
	public EachGestureJoint		_scr_Hand_L;
	public EachGestureJoint		_scr_Arm_R;
	public EachGestureJoint		_scr_Hand_R;
	
	
	
	
	
	public List<CTimeTable> _lstTimeTable;
	
	
	
	
	
	
	// Start ------------------------------------------------------------------------------
	void Start ()
	{
		_RobotProxy.OnConnected += OnConnected;
		_RobotProxy.OnConnectionFailed += OnConnectionFailed;
		_RobotProxy.OnDisconnected += OnDisconnected;
		_RobotProxy.OnSearchCompleted += OnSearchCompleted;
		
		_goBtnStart.SetActive(false);
		//		_goBtnGesture.SetActive(false);
		//		_goBtnAvatar.SetActive(false);
		_goRobot.transform.position = new Vector3(0f, 200f, 0f);
		
		_RobotProxy.PortSearch();
		CheckKinect();
		
		GoTitle();
		
		_SimpleJoints = FindObjectsOfType(typeof(SimpleJoint)) as SimpleJoint[];
		
		LoadOptions();
	}
	
	
	
	
	void LoadOptions()
	{
		if (PlayerPrefs.GetInt("PP_StartTime") != 0 )
		{
			_nOption_StartTime = PlayerPrefs.GetInt("PP_StartTime");
			_Ulbl_StartTime.text = _nOption_StartTime.ToString();
		}
		if (PlayerPrefs.GetInt("PP_Repeat") != 0 )
		{
			_nOption_Repeat = PlayerPrefs.GetInt("PP_Repeat");
			_Ulbl_Repeat.text = _nOption_Repeat.ToString();
			Show_RepeatCounter();
		}
		if (PlayerPrefs.GetInt("PP_Duration") != 0 )
		{
			_nOption_Duration = PlayerPrefs.GetInt("PP_Duration");
			_Ulbl_Duration.text = _nOption_Duration.ToString();
		}
		if (PlayerPrefs.GetInt("PP_Interval") != 0 )
		{
			_nOption_Interval = PlayerPrefs.GetInt("PP_Interval");
			_nOption_Interval_Include = _nOption_Interval + _nOption_Duration;
			_Ulbl_Interval.text = _nOption_Interval_Include.ToString();
		}
		if (PlayerPrefs.GetInt("PP_PlayTime") != 0 )
		{
			_nOption_PlayTime = PlayerPrefs.GetInt("PP_PlayTime");
			_Ulbl_PlayTime.text = _nOption_PlayTime.ToString();
		}
		if (PlayerPrefs.GetInt("PP_Button") != 0 )
		{
			_nOption_Button = PlayerPrefs.GetInt("PP_Button");
			_Ulbl_Button.text = _nOption_Button.ToString();
		}
		
		
		_nOption_Gesture = PlayerPrefs.GetInt("PP_GestureMode");
		Change_Tollge_Gesture(_nOption_Gesture);
		
		_nOption_Avatar = PlayerPrefs.GetInt("PP_AvatarMode");
		Change_Tollge_Avatar(_nOption_Avatar);
		
		Set_TimeTable();
	}
	
	
	
	public void SaveOptions()
	{
		PlayerPrefs.SetInt("PP_StartTime",_nOption_StartTime);
		PlayerPrefs.SetInt("PP_Repeat",_nOption_Repeat);
		PlayerPrefs.SetInt("PP_Interval",_nOption_Interval);
		PlayerPrefs.SetInt("PP_Duration",_nOption_Duration);
		PlayerPrefs.SetInt("PP_PlayTime",_nOption_PlayTime);
		PlayerPrefs.SetInt("PP_Button",_nOption_Button);
		PlayerPrefs.SetInt("PP_GestureMode",_nOption_Gesture);
		PlayerPrefs.SetInt("PP_AvatarMode",_nOption_Avatar);
		
		Set_TimeTable();
	}
	
	



	public void Set_TimeTable()
	{
		_lstTimeTable = new List<CTimeTable>();
		int _nTime = _nOption_StartTime * 3600;
		
		
		for (int i = 1; i <= _nOption_Repeat; i++)
		{
			CTimeTable timetable = new CTimeTable();
			timetable._nCount = i;
			timetable._nStartPlay = _nTime;
			timetable._nEndPlay = _nTime + (_nOption_Duration * 60);
			_nTime = timetable._nEndPlay;
			timetable._nEndBreak = _nTime + (_nOption_Interval * 60);
			_nTime = timetable._nEndBreak;
			_lstTimeTable.Add(timetable);
			
			Debug.Log("Count: " + timetable._nCount.ToString() +
			          "   Start: " + timetable._nStartPlay.ToString() + 
			          "   End: " + timetable._nEndPlay.ToString() +
			          "   EndBreak: " + timetable._nEndBreak.ToString() );
		}
		
		Debug.Log("TableCount " + _lstTimeTable.Count.ToString());


		int nH = (int)Mathf.Floor(_nTime / 3600);
		int nM = (int)Mathf.Floor((_nTime - (nH * 3600)) / 60);
		
		_Ulbl_EndTime.text = "예상종료시각 " + nH.ToString() +  ":" + string.Format("{0:D2}", nM);
	}






	
	public void Search_Ports()
	{
		_goBtnConnect.SetActive(false);		// NGUI
		_RobotProxy.PortSearch();
	}
	
	
	public void Connect_Port()
	{
		_RobotProxy.portName = _UIPortList.value;
		_RobotProxy.Connect();
		_UILblRobotMessage.text = "연결 시도중..."; // NGUI
		_goBtnConnect.SetActive(false);	// NGUI
	}
	
	
	void CheckKinect()
	{
		if (_KinectManager.IsInitialized())
		{
			IsConnect_Kinect = true;
			_UILblKinectMessage.text = "연결됨"; // NGUI
		}
		
		Invoke("CheckKinect", 1f);
	}
	
	
	
	void CheckStartButton()
	{
		if ((IsConnect_Robot) && (IsConnect_Kinect))
			_goBtnStart.SetActive(true);
		else
			_goBtnStart.SetActive(false);
	}
	
	
	
	public void AppExit()
	{
		Application.Quit();
	}
	
	
	
	bool AddRepeatCounter()
	{
		bool bValue = true;
		
		if (_nCurRepeat < _nOption_Repeat)
			_nCurRepeat = _nCurRepeat + 1;
		else
			bValue = false;
		
		return bValue;
	}
	
	
	
	void Show_RepeatCounter()
	{
		_UILblRepeat_Title.text = "회차 " + _nCurRepeat.ToString() + "/" + _nOption_Repeat.ToString();
		_UILblRepeat_Waiting.text = "회차 " + _nCurRepeat.ToString() + "/" + _nOption_Repeat.ToString();
	}
	
	
	
	
	
	
	public void GoBack()
	{
		if (_eCurMode == EMode.WAITING)
			GoTitle();
		else if ( (_eCurMode == EMode.GESTURE) || (_eCurMode == EMode.AVATAR) )
			GoWaiting();
	}
	
	
	
	public void GoTitle()
	{
		_eCurMode = EMode.TITLE;
		_goUI_Title.SetActive(true);
		_goPnl_Title.SetActive(true);
		_goPnl_Deco.SetActive(true);
		_goLbl_Standby.SetActive(false);
		_goPnl_Option.SetActive(false);
		_goUI_Waiting.SetActive(false);
		_goObj_Robot.SetActive(false);
		_goRobot.transform.position = new Vector3(0f, 200f, 0f);
		Show_RepeatCounter();
		_KinectManager.DisplayUserMap = false;

		_goBGM.audio.Stop();

		CancelInvoke("Show_StanbyTime");
		CancelInvoke("Show_ImtervalTime");
		CancelInvoke("DurationCountDown");
		CancelInvoke("AvatarCountDown");
		CancelInvoke("CheckGesture");
		CancelInvoke("CheckPlayer");
	}
	
	
	public void GoOption()
	{
		_eCurMode = EMode.OPTION;
		_goUI_Title.SetActive(true);
		_goPnl_Title.SetActive(false);
		_goPnl_Deco.SetActive(false);
		_goPnl_Option.SetActive(true);
		_goUI_Waiting.SetActive(false);
		_goObj_Robot.SetActive(false);
		_goRobot.transform.position = new Vector3(0f, 200f, 0f);
		_KinectManager.DisplayUserMap = false;
	}
	
	
	
	
	public void GoQuickStart()
	{
		int _nCurTime;
		_nCurTime = (DateTime.Now.Hour * 3600) + (DateTime.Now.Minute * 60) + (DateTime.Now.Second);
		
		foreach(CTimeTable table in _lstTimeTable)
		{
			if (table._nStartPlay > _nCurTime)
			{
				_nCurRepeat = 1;
				break;
			}
			if ( (table._nStartPlay <= _nCurTime) && (table._nEndPlay > _nCurTime) )
			{
				_nCurRepeat = table._nCount;
				break;
			}
			else if ( (table._nEndPlay <= _nCurTime) && (table._nEndBreak > _nCurTime) )
			{
				_nCurRepeat = table._nCount;
				break;
			}
		}

		_nCurDuration = _nOption_Duration * 60; // 회당 제한 시간-
		DurationCountDown();
		GoWaiting();
		PlayBGM();
	}
	
	
	
	public void GoStart()
	{
		int _nCurTime;
		_nCurTime = (DateTime.Now.Hour * 3600) + (DateTime.Now.Minute * 60) + (DateTime.Now.Second);
		
		
		bool _bReturn = false;
		
		
		foreach(CTimeTable table in _lstTimeTable)
		{
			if (table._nStartPlay > _nCurTime)
			{
				GoStandby(true); // 시작시간을 기다림-
				_bReturn = true;
				break;
			}
			
			if ( (table._nStartPlay <= _nCurTime) && (table._nEndPlay > _nCurTime) )
			{
				_nCurRepeat = table._nCount;
				Show_RepeatCounter();
				_nCurDuration = table._nEndPlay - _nCurTime;
				DurationCountDown();
				GoWaiting();
				PlayBGM();
				_bReturn = true;
				break;
			}
			else if ( (table._nEndPlay <= _nCurTime) && (table._nEndBreak > _nCurTime) )
			{
				_nCurRepeat = table._nCount;
				Show_RepeatCounter();
				_nCurDuration = -1;
				_nCurInterval = table._nEndBreak - _nCurTime;
				_eCurMode = EMode.WAITING;
				DurationCountDown();
				_bReturn = true;
				break;
			}
		}
		
		if (_bReturn)
			return;
		
		_nCurRepeat = _nOption_Repeat;
		Show_RepeatCounter();
		_nCurDuration = -1;
		_nCurInterval = -1;
		_eCurMode = EMode.WAITING;
		DurationCountDown();
	}
	
	
	
	
	public void GoStandby(bool p_IsFirstStart)
	{
		_eCurMode = EMode.STANDBY;
		_goUI_Title.SetActive(true);
		_goPnl_Title.SetActive(false);
		_goPnl_Deco.SetActive(true);
		_goLbl_Standby.SetActive(true);
		_goPnl_Option.SetActive(false);
		_goUI_Waiting.SetActive(false);
		_goObj_Robot.SetActive(false);
		_goRobot.transform.position = new Vector3(0f, 200f, 0f);
		if (p_IsFirstStart) // 처음 시작에 의한 것인가?---
		{
			CancelInvoke("Show_StanbyTime");
			Invoke("Show_StanbyTime", 1f);
		}
		else // 공연을 끝내고 다음공연을 기다림--
		{
			int _nCurTime;
			_nCurTime = (DateTime.Now.Hour * 3600) + (DateTime.Now.Minute * 60) + (DateTime.Now.Second);
			
			_nCurInterval = (_lstTimeTable[_nCurRepeat-1]._nEndBreak - _nCurTime);
			CancelInvoke("Show_ImtervalTime");
			CancelInvoke("AvatarCountDown");
			Invoke("Show_ImtervalTime", 1f);
		}
		CancelInvoke("DurationCountDown");
		CancelInvoke("CheckPlayer");
		_goBGM.audio.Stop();
		_KinectManager.DisplayUserMap = false;
	}
	
	
	// 시작시간을 기다림--------------------------------------------------------
	void Show_StanbyTime()
	{
		int _nCurTime;
		_nCurTime = (DateTime.Now.Hour * 3600) + (DateTime.Now.Minute * 60) + (DateTime.Now.Second);
		
		if (_lstTimeTable[0]._nStartPlay <= _nCurTime)
		{
			if (AddRepeatCounter())
			{
				_nCurDuration = _nOption_Duration * 60; // 회당 제한 시간-
				DurationCountDown();
				GoWaiting();
				PlayBGM();
			}
			return;
		}
		
		if (_nCurRepeat >= _nOption_Repeat)
		{
			_ULbl_Standby.text = "";
			_ULbl_StandbyTime.text = "체험 종료";
			return;
		}
		
		int nTime = _nOption_StartTime - DateTime.Now.Hour - 1;
		string strTime = string.Format("{0:D2}", nTime);
		
		int nMinute = 60 - DateTime.Now.Minute - 1;
		string strMinute = string.Format("{0:D2}", nMinute);
		
		int nSecond = 60 - DateTime.Now.Second - 1;
		string strSecond = string.Format("{0:D2}", nSecond);
		
		_ULbl_StandbyTime.text = strTime + "." + strMinute + "." + strSecond;
		
		Invoke("Show_StanbyTime", 1f);
	}
	
	
	// 다음공연을 기다림-----------------------------------------------------------
	void Show_ImtervalTime()
	{
		if (_nCurRepeat >= _nOption_Repeat)
		{
			_ULbl_Standby.text = "";
			_ULbl_StandbyTime.text = "체험 종료";
			return;
		}

		if (_nCurInterval <= -1)
		{
			if (AddRepeatCounter())
			{
				_nCurDuration = _nOption_Duration * 60; // 회당 제한 시간-
				DurationCountDown();
				GoWaiting();
				PlayBGM();
			}
			return;
		}

		
		
		int nTime = (int)Mathf.Floor(_nCurInterval / 360);
		string strTime = string.Format("{0:D2}", nTime);
		
		int nMinute = (int)Mathf.Floor((_nCurInterval - (nTime * 360)) / 60);
		string strMinute = string.Format("{0:D2}", nMinute);
		
		int nSecond = _nCurInterval - (nTime * 360) - (nMinute * 60);
		string strSecond = string.Format("{0:D2}", nSecond);
		
		_ULbl_StandbyTime.text = strTime + "." + strMinute + "." + strSecond;
		
		_nCurInterval --;
		
		Invoke("Show_ImtervalTime", 1f);
	}
	
	
	
	
	
	public void PlayBGM()
	{
		_goBGM.audio.Play();
	}
	
	
	
	
	
	public void GoWaiting()
	{
		_eCurMode = EMode.WAITING;
		_goCursor.SetActive(true);
		_goUI_Title.SetActive(false);
		_goUI_Waiting.SetActive(true);
		_goWaitingSet.SetActive(true);
		_goGestureBtnSet.SetActive(false);
		_goObj_Robot.SetActive(true);
		Clear_Guide();
		_goSuccessGesture.SetActive(false);
		CancelInvoke("CheckPlayer");
		CancelInvoke("CheckGesture");
		CancelInvoke("AvatarCountDown");
		_UILblTimer.enabled = false;
		_strModeMessage = "한 손을 앞으로 뻗어\n버튼을 선택한 뒤\n동그라미가 그려질 때까지\n움직이지 마세요";
		_UILblMessage.text = _strModeMessage;
		CheckPlayer();
		
		if (_nOption_Gesture == 1)
			_goGestureModeBtn.SetActive(true);
		else
			_goGestureModeBtn.SetActive(false);
		
		if (_nOption_Avatar == 1)
			_goAvatarModeBtn.SetActive(true);
		else
			_goAvatarModeBtn.SetActive(false);
		
		FollowOnOff(false);
		Show_RepeatCounter();
		_goBtnDemo.SetActive(true);
		_KinectManager.DisplayUserMap = true;
	}




	
	void DurationCountDown()
	{
		if ( (_nCurDuration <= -1) && (_eCurMode == EMode.WAITING) )
		{
			GoStandby(false); // 다음공연까지의 시간을 기다림-
			return;
		}

		_nCurDuration --;
		
		if (_nCurDuration <= 29)
		{
			//_UILblDuration.color = Color.red;
			_UILblDuration.fontSize = 50 - _nCurDuration;
		}
		else
			_UILblDuration.fontSize = 20;


		if (_nCurDuration > 0)
			_UILblDuration.text = "체험 종료까지 " + _nCurDuration.ToString();
		else
		{
			_UILblDuration.fontSize = 50;
			_UILblDuration.text = "체험 종료합니다";
		}
		
		CancelInvoke("DurationCountDown");
		Invoke("DurationCountDown", 1f);
	}
	
	
	
	
	
	
	
	
	
	
	public void GoGesture()
	{
		_eCurMode = EMode.GESTURE;
		ResetGestureBtn();
		_goUI_Title.SetActive(false);
		_goUI_Waiting.SetActive(true);
		_goWaitingSet.SetActive(false);
		_goGestureBtnSet.SetActive(true);
		_goObj_Robot.SetActive(true);
		_strModeMessage = "버튼을 선택하세요";
		_UILblMessage.text = _strModeMessage;
		_UILblTimer.enabled = true;
		_nAvatarTimer = _nOption_PlayTime;
		AvatarCountDown();
		_nGestureNum = -2;
		SelectGesture(-1);

		_Animation.Play("Ready");

		FollowOnOff(true);
		_goBtnDemo.SetActive(false);
	}
	
	
	
	public void SelectGesture(int p_Num)
	{
		if (p_Num == _nGestureNum)
			return;
		
		_nGestureNum = p_Num;
		
		foreach (GameObject Btn in _goGestureBtn)
		{
			Btn.SetActive(false);
		}
		
		
		if (p_Num == -1)
		{
			for (int i = 0; i < _nOption_Button; i ++)
			{
				_goGestureBtn[i].SetActive(true);
			}
			_goSuccessGesture.SetActive(false);
			Clear_Guide();
			return;
		}
		
		_goCursor.SetActive(false);
		
		_goGestureBtn[p_Num].SetActive(true);
		GuideList[p_Num].SetActive(true);
		
		SetGestureGuide(p_Num);
		
		CheckGesture();

		_strModeMessage = "표시된 동작을 따라해보세요";
		_UILblMessage.text = _strModeMessage;
	}
	
	
	
	void Clear_Guide()
	{
		foreach (GameObject Img in GuideList)
		{
			Img.SetActive(false);
		}		
	}
	
	
	
	void CheckGesture()
	{
		if ( (_scr_Arm_L._bIsContact) && (_scr_Hand_L._bIsContact) && (_scr_Arm_R._bIsContact) && (_scr_Hand_R._bIsContact) )
		{
			_goSuccessGesture.SetActive(true);
			_goSnd_Success.audio.Play();
			Clear_Guide();
			switch(_nGestureNum)
			{
			case 0:
				_Animation.Play("Greeting");
				break;

			case 1:
				_Animation.Play("Love");
				break;

			case 2:
				_Animation.Play("Salute");
				break;

			case 3:
				_Animation.Play("Come on");
				break;

			case 4:
				_Animation.Play("Wave");
				break;
			}
			CheckGestureEnd();
		}
		else
		{
			Invoke("CheckGesture", 0.1f);
		}
	}
	
	
	
	
	
	void CheckGestureEnd()
	{
		if (_Animation.isPlaying == false)
		{
			_goCursor.SetActive(true);
			ResetGestureBtn();

			if (_bIsDemoPlay)
			{
				_goAncCenter.SetActive(true);
				_goBtnDemo.SetActive(true);
			}

			_bIsDemoPlay = false;
		}
		else
			Invoke("CheckGestureEnd", 0.1f);
	}
	
	
	
	
	
	void ResetGestureBtn()
	{
		if (_eCurMode != EMode.GESTURE)
			return;
		
		_strModeMessage = "버튼을 선택하세요";
		_UILblMessage.text = _strModeMessage;

		_goCursor.SetActive(true);
		SelectGesture(-1);
		_goSuccessGesture.SetActive(false);
		Clear_Guide();
	}
	
	


	// 데모 제스쳐 플레이--------------
	public void Play_GestureDemo()
	{
		_nGestureNum = 99;
		_bIsDemoPlay = true;
		_goCursor.SetActive(false);
		_goAncCenter.SetActive(false);
		_goBtnDemo.SetActive(false);
		_Animation.Play("Wave");

		CheckGestureEnd();
	}










	
	
	public void GoAvatar()
	{
		_eCurMode = EMode.AVATAR;
		_goCursor.SetActive(false);
		_goUI_Title.SetActive(false);
		_goUI_Waiting.SetActive(true);
		_goWaitingSet.SetActive(false);
		_goGestureBtnSet.SetActive(false);
		_goObj_Robot.SetActive(true);
		_strModeMessage = "팔을 움직여 보세요.\n로봇이 따라합니다";
		_UILblMessage.text = _strModeMessage;
		_UILblTimer.enabled = true;
		_nAvatarTimer = _nOption_PlayTime;
		AvatarCountDown();
		
		FollowOnOff(true);
		_goBtnDemo.SetActive(false);
	}
	
	
	
	void AvatarCountDown()
	{
		_UILblTimer.text = _nAvatarTimer.ToString();

		if (_nAvatarTimer <= -1)
		{
			_UILblTimer.text = "0";

			if ( (_eCurMode == EMode.AVATAR) || ((_eCurMode == EMode.GESTURE) && (_Animation.isPlaying == false)) )
			{
				GoWaiting();
				return;
			}
		}

		_nAvatarTimer --;
		
		CancelInvoke("AvatarCountDown");
		Invoke("AvatarCountDown", 1f);
	}
	
	
	
	
	void FollowOnOff(bool p_OnOff)
	{
		if (_SimpleJoints.Length == 0)
			return;
		
		foreach(SimpleJoint joint in _SimpleJoints)
		{
			joint.follow = p_OnOff;
		}
	}
	
	
	
	
	
	// SetGestureGuide ----------------------------------------------------------
	void SetGestureGuide(int p_Number)
	{
		foreach(GameObject goGesture in GestureList)
		{
			goGesture.SetActive(false);
		}
		
		GestureList[p_Number].SetActive(true);
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	void CheckPlayer()
	{
		if (_KinectManager.IsPlayerCalibrated(_KinectManager.GetPlayer1ID()))
		{
			_goLinePerson.SetActive(false);
			_goRobot.transform.position = new Vector3(0f, 100f, 0f);
			_UILblMessage.text = _strModeMessage;
		}
		else
		{
			ResetGestureBtn();
			_goLinePerson.SetActive(true);
			_goRobot.transform.position = new Vector3(0f, 200f, 0f);
			_UILblMessage.text = "키넥트 앞에 서세요";
		}

		if (_bIsDemoPlay)
			_UILblMessage.text = "데모 플레이중입니다";

		Invoke("CheckPlayer", 0.1f);
	}
	
	
	
	
	
	
	// Option StartTime ====================================================
	
	public void Option_SetStartTime_Plus()
	{
		_nOption_StartTime ++;
		
		if (_nOption_StartTime > 24)
			_nOption_StartTime = _nOption_StartTime - 24;
		
		_Ulbl_StartTime.text = _nOption_StartTime.ToString();
		Set_TimeTable();
	}
	
	public void Option_SetStartTime_Minus()
	{
		_nOption_StartTime --;
		
		if (_nOption_StartTime <= 0)
			_nOption_StartTime = _nOption_StartTime + 24;
		
		_Ulbl_StartTime.text = _nOption_StartTime.ToString();
		Set_TimeTable();
	}
	
	
	
	
	
	
	// Option Repeat ====================================================
	
	public void Option_SetRepeat_Plus()
	{
		_nOption_Repeat ++;
		
		if (_nOption_Repeat > 99)
			_nOption_Repeat = _nOption_Repeat - 99;
		
		_Ulbl_Repeat.text = _nOption_Repeat.ToString();
		Show_RepeatCounter();
		Set_TimeTable();
	}
	
	public void Option_SetRepeat_Minus()
	{
		_nOption_Repeat --;
		
		if (_nOption_Repeat <= 0)
			_nOption_Repeat = _nOption_Repeat + 99;
		
		_Ulbl_Repeat.text = _nOption_Repeat.ToString();
		Show_RepeatCounter();
		Set_TimeTable();
	}
	
	
	
	// Option Interval ====================================================
	
	public void Option_SetInterval_Plus()
	{
		_nOption_Interval ++;
		
		if (_nOption_Interval > 120)
			_nOption_Interval = _nOption_Interval - 120;
		
		_nOption_Interval_Include = _nOption_Interval + _nOption_Duration;
		_Ulbl_Interval.text = _nOption_Interval_Include.ToString();
		Set_TimeTable();
	}
	
	public void Option_SetInterval_Minus()
	{
		_nOption_Interval --;
		
		if (_nOption_Interval <= 0)
			_nOption_Interval = _nOption_Interval + 120;
		
		_nOption_Interval_Include = _nOption_Interval + _nOption_Duration;
		_Ulbl_Interval.text = _nOption_Interval_Include.ToString();
		Set_TimeTable();
	}
	
	
	
	
	
	// Option Duration ====================================================
	
	public void Option_SetDuration_Plus()
	{
		_nOption_Duration ++;
		
		if (_nOption_Duration > 60)
		{
			_nOption_Duration = _nOption_Duration - 60;
			_nOption_Interval = _nOption_Interval + 60;
		}
		
		_Ulbl_Duration.text = _nOption_Duration.ToString();

		if ((_nOption_Interval_Include > _nOption_Duration) && (_nOption_Interval > 1) )
			_nOption_Interval --;

		_nOption_Interval_Include = _nOption_Interval + _nOption_Duration;
		_Ulbl_Interval.text = _nOption_Interval_Include.ToString();
		Set_TimeTable();
	}
	
	public void Option_SetDuration_Minus()
	{
		_nOption_Duration --;
		
		if (_nOption_Duration <= 0)
		{
			_nOption_Duration = _nOption_Duration + 60;
			
			if (_nOption_Interval_Include > _nOption_Duration)
				_nOption_Interval = _nOption_Interval - 60;
			else
				_nOption_Interval = 0;
		}
		
		_Ulbl_Duration.text = _nOption_Duration.ToString();
		
		_nOption_Interval ++;

		_nOption_Interval_Include = _nOption_Interval + _nOption_Duration;
		_Ulbl_Interval.text = _nOption_Interval_Include.ToString();
		Set_TimeTable();
	}
	
	
	
	// Option PlayTime ====================================================
	
	public void Option_SetPlayTime_Plus()
	{
		_nOption_PlayTime = _nOption_PlayTime + 10;
		
		if (_nOption_PlayTime > 300)
			_nOption_PlayTime = _nOption_PlayTime - 300;
		
		_Ulbl_PlayTime.text = _nOption_PlayTime.ToString();
	}
	
	public void Option_SetPlayTime_Minus()
	{
		_nOption_PlayTime = _nOption_PlayTime - 10;
		
		if (_nOption_PlayTime <= 0)
			_nOption_PlayTime = _nOption_PlayTime + 300;
		
		_Ulbl_PlayTime.text = _nOption_PlayTime.ToString();
	}
	
	
	
	
	// Option Button Number ==============================================
	
	public void Option_SetButton_Plus()
	{
		_nOption_Button ++;
		
		if (_nOption_Button > 10)
			_nOption_Button = _nOption_Button - 10;
		
		_Ulbl_Button.text = _nOption_Button.ToString();
	}
	
	public void Option_SetButton_Minus()
	{
		_nOption_Button --;
		
		if (_nOption_Button <= 0)
			_nOption_Button = _nOption_Button + 10;
		
		_Ulbl_Button.text = _nOption_Button.ToString();
	}
	
	
	
	
	// Option Gesture Mode ==============================================
	
	public void Option_SetGestureMode()
	{
		if (_nOption_Gesture == 0)
			_nOption_Gesture = 1;
		else
			_nOption_Gesture = 0;
		
		Change_Tollge_Gesture(_nOption_Gesture);
	}
	
	void Change_Tollge_Gesture(int p_OnOff)
	{
		if (_nOption_Gesture == 1)
		{
			_goGestureOn.SetActive(true);
			_goGestureOff.SetActive(false);
		}
		else
		{
			_goGestureOn.SetActive(false);
			_goGestureOff.SetActive(true);
		}
	}
	
	
	
	
	// Option Avatar Mode ==============================================
	
	public void Option_SetAvatarMode()
	{
		if (_nOption_Avatar == 0)
			_nOption_Avatar = 1;
		else
			_nOption_Avatar = 0;
		
		Change_Tollge_Avatar(_nOption_Avatar);
	}
	
	void Change_Tollge_Avatar(int p_OnOff)
	{
		if (_nOption_Avatar == 1)
		{
			_goAvatarOn.SetActive(true);
			_goAvatarOff.SetActive(false);
		}
		else
		{
			_goAvatarOn.SetActive(false);
			_goAvatarOff.SetActive(true);
		}
	}
	
	
	
	// 클릭사운드----
	public void Sound_Click()
	{
		_goSnd_Click.audio.Play();
	}


	
	
	
	
	
	void OnConnected(object sender, EventArgs e)
	{
		IsConnect_Robot = true;
		_UILblRobotMessage.text = "연결됨"; // NGUI
		CheckStartButton(); // NGUI
	}
	
	void OnConnectionFailed(object sender, EventArgs e)
	{
		IsConnect_Robot = false;
		_UILblRobotMessage.text = "연결 실패"; // NGUI
		_goBtnConnect.SetActive(true);		// NGUI
		CheckStartButton(); // NGUI
	}
	
	void OnDisconnected(object sender, EventArgs e)
	{
		IsConnect_Robot = false;
		_UILblRobotMessage.text = "연결 끊어짐"; // NGUI
		_goBtnConnect.SetActive(true);		// NGUI
		CheckStartButton(); // NGUI
		
		Invoke("Clear_Ports", 0.1f);
		Invoke("Search_Ports", 0.2f);
		GoTitle();
	}
	
	void OnSearchCompleted(object sender, EventArgs e)
	{
		_UIPortList.items.Clear();
		
		if(_RobotProxy.portNames.Count > 0)
		{
			for(int i=0; i<_RobotProxy.portNames.Count; i++)
			{
				_UIPortList.items.Add(_RobotProxy.portNames[i]);
			}
			_UIPortList.value = _UIPortList.items[0];
			_goBtnConnect.SetActive(true);		// NGUI
		}
		else if(_RobotProxy.portNames.Count == 0)
		{
			_UIPortList.items.Add("None");
		}
	}
}

