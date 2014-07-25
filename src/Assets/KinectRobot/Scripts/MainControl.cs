using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityRobot;





public enum EMode
{
	TITLE,
	STANDBY,
	OPTION,
	WAITING,
	GESTURE,
	AVATAR,
}





public class MainControl : MonoBehaviour 
{
	public RobotProxy		_RobotProxy;
	public KinectManager	_KinectManager;

	bool IsConnect_Robot = false;
	bool IsConnect_Kinect = false;

	EMode	_eCurMode = EMode.TITLE;
	string	_statusMessage = "Ready";


	// NGUI ========================
	public GameObject		_goUI_Title;
	public GameObject		_goPnl_Title;
	public GameObject		_goPnl_Option;
	public GameObject		_goPnl_Deco;

	public GameObject		_goLbl_Standby;
	public UILabel			_ULbl_StandbyTime;

	public GameObject		_goUI_Waiting;
	public GameObject		_goObj_Robot;


	public UIPopupList		_UIPortList;
	public GameObject		_goBtnConnect;
	public GameObject		_goBtnStart;
	public UILabel			_UILblRobotMessage;
	public UILabel			_UILblKinectMessage;


	public GameObject		_goCursor;
	public GameObject		_goWaitingSet;
	public GameObject		_goGestureBtnSet;
	public GameObject[]		_goGestureBtn = new GameObject[10];
	public GameObject		_goLinePerson;
	public GameObject		_goRobot;
	public UILabel			_UILblMessage;
	public UILabel			_UILblTimer;
	String					_strMode = "";

	public UILabel			_UILblRepeat_Title;
	public UILabel			_UILblRepeat_Waiting;

	public UILabel			_UILblDuration;
	// NGUI ========================

	// OPTION & NGUI ========================
	public int		_nOption_StartTime = 1;
	public int		_nOption_Repeat = 1;
	public int		_nOption_Interval = 1;
	public int		_nOption_Duration = 1;
	public int		_nOption_PlayTime = 10;
	public int		_nOption_Button = 10;

	public UILabel	_Ulbl_StartTime;
	public UILabel	_Ulbl_Repeat;
	public UILabel	_Ulbl_Interval;
	public UILabel	_Ulbl_Duration;
	public UILabel	_Ulbl_PlayTime;
	public UILabel	_Ulbl_Button;
	// OPTION & NGUI ========================


	int _nCurRepeat = 0; // 현재반복횟수--
	int _nCurDuration = 0;	// 회당 제한시간---
	int _nCurInterval = 0;	// 다음공연까지의 남은시간-

	int _nAvatarTimer = 0;


	public CMDModule		_CMDModule;

	AvatarJoint[] _AvatarJoints;



	int _nGestureNum = -2;

	public List<GameObject> GestureList = new List<GameObject>();
	public List<GameObject> GuideList = new List<GameObject>();

	public GameObject		_goSuccessGesture;

	public EachGestureJoint		_scr_Arm_L;
	public EachGestureJoint		_scr_Hand_L;
	public EachGestureJoint		_scr_Arm_R;
	public EachGestureJoint		_scr_Hand_R;












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

		_AvatarJoints = FindObjectsOfType(typeof(AvatarJoint)) as AvatarJoint[];
	
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
		if (PlayerPrefs.GetInt("PP_Interval") != 0 )
		{
			_nOption_Interval = PlayerPrefs.GetInt("PP_Interval");
			_Ulbl_Interval.text = _nOption_Interval.ToString();
		}
		if (PlayerPrefs.GetInt("PP_Duration") != 0 )
		{
			_nOption_Duration = PlayerPrefs.GetInt("PP_Duration");
			_Ulbl_Duration.text = _nOption_Duration.ToString();
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
	}



	public void SaveOptions()
	{
		PlayerPrefs.SetInt("PP_StartTime",_nOption_StartTime);
		PlayerPrefs.SetInt("PP_Repeat",_nOption_Repeat);
		PlayerPrefs.SetInt("PP_Interval",_nOption_Interval);
		PlayerPrefs.SetInt("PP_Duration",_nOption_Duration);
		PlayerPrefs.SetInt("PP_PlayTime",_nOption_PlayTime);
		PlayerPrefs.SetInt("PP_Button",_nOption_Button);
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
		_UILblRobotMessage.text = "Tring..."; // NGUI
		_goBtnConnect.SetActive(false);	// NGUI
	}


	void CheckKinect()
	{
		if (_KinectManager.IsInitialized())
		{
			IsConnect_Kinect = true;
			_UILblKinectMessage.text = "Connceted"; // NGUI
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
		_UILblRepeat_Title.text = "R " + _nCurRepeat.ToString() + "/" + _nOption_Repeat.ToString();
		_UILblRepeat_Waiting.text = "R " + _nCurRepeat.ToString() + "/" + _nOption_Repeat.ToString();
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
	}




	public void GoQuickStart()
	{
		_nCurDuration = _nOption_Duration * 60; // 회당 제한 시간-
		DurationCountDown();
		GoWaiting();
	}



	public void GoStart()
	{
		if (_nOption_StartTime <= DateTime.Now.Hour)
		{
			if (AddRepeatCounter())
			{
				_nCurDuration = _nOption_Duration * 60; // 회당 제한 시간-
				DurationCountDown();
				GoWaiting();
			}
		}
		else
			GoStandby(true); // 시작시간을 기다림-
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
			_nCurInterval = _nOption_Interval * 60;
			CancelInvoke("Show_ImtervalTime");
			Invoke("Show_ImtervalTime", 1f);
		}
	}


	// 시작시간을 기다림--------------------------------------------------------
	void Show_StanbyTime()
	{
		if (_nOption_StartTime <= DateTime.Now.Hour)
		{
			if (AddRepeatCounter())
			{
				_nCurDuration = _nOption_Duration * 60; // 회당 제한 시간-
				DurationCountDown();
				GoWaiting();
			}
			return;
		}

		if (_nCurRepeat >= _nOption_Repeat)
		{
			_ULbl_StandbyTime.text = "CLOSED";
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
		if (_nCurInterval <= -1)
		{
			if (AddRepeatCounter())
			{
				_nCurDuration = _nOption_Duration * 60; // 회당 제한 시간-
				DurationCountDown();
				GoWaiting();
			}
			return;
		}
		
		if (_nCurRepeat >= _nOption_Repeat)
		{
			_ULbl_StandbyTime.text = "CLOSED";
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
		CancelInvoke("AvatarCountDown");
		_UILblTimer.enabled = false;
		CheckPlayer();

		FollowOnOff(false);
		Show_RepeatCounter();
	}


	void DurationCountDown()
	{
		if (_nCurDuration <= -1)
		{
			GoStandby(false); // 다음공연까지의 시간을 기다림-
			return;
		}
		
		_UILblDuration.text = "D " + _nCurDuration.ToString();
		_nCurDuration --;

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
		_strMode = "Gesture Mode";
		_UILblMessage.text = _strMode;
		_UILblTimer.enabled = true;
		_nAvatarTimer = _nOption_PlayTime;
		AvatarCountDown();
		_nGestureNum = -2;
		SelectGesture(-1);
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
			Clear_Guide();
			_CMDModule.Value = (Byte)(_nGestureNum + 1);
			CheckGestureEnd();
		}
		else
		{
			Invoke("CheckGesture", 0.1f);
		}
	}





	void CheckGestureEnd()
	{
		if (_CMDModule.Value == 0)
			ResetGestureBtn();
		else
			Invoke("CheckGestureEnd", 0.1f);
	}





	void ResetGestureBtn()
	{
		_goCursor.SetActive(true);
		SelectGesture(-1);
		_goSuccessGesture.SetActive(false);
		Clear_Guide();
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
		_strMode = "Avatar Mode";
		_UILblMessage.text = _strMode;
		_UILblTimer.enabled = true;
		_nAvatarTimer = _nOption_PlayTime;
		AvatarCountDown();

		FollowOnOff(true);
	}



	void AvatarCountDown()
	{
		if (_nAvatarTimer <= -1)
		{
			GoWaiting();
			return;
		}

		_UILblTimer.text = _nAvatarTimer.ToString();
		_nAvatarTimer --;

		CancelInvoke("AvatarCountDown");
		Invoke("AvatarCountDown", 1f);
	}




	void FollowOnOff(bool p_OnOff)
	{
		if (_AvatarJoints.Length == 0)
			return;

		foreach(AvatarJoint joint in _AvatarJoints)
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
			_UILblMessage.text = _strMode;
		}
		else
		{
			ResetGestureBtn();
			_goLinePerson.SetActive(true);
			_goRobot.transform.position = new Vector3(0f, 200f, 0f);
			_UILblMessage.text = "Waiting Player";
		}
		
		Invoke("CheckPlayer", 0.1f);
	}






	// Option StartTime ====================================================

	public void Option_SetStartTime_Plus()
	{
		_nOption_StartTime ++;

		if (_nOption_StartTime > 24)
			_nOption_StartTime = _nOption_StartTime - 24;
		
		_Ulbl_StartTime.text = _nOption_StartTime.ToString();
	}

	public void Option_SetStartTime_Minus()
	{
		_nOption_StartTime --;
		
		if (_nOption_StartTime <= 0)
			_nOption_StartTime = _nOption_StartTime + 24;
		
		_Ulbl_StartTime.text = _nOption_StartTime.ToString();
	}






	// Option Repeat ====================================================

	public void Option_SetRepeat_Plus()
	{
		_nOption_Repeat ++;
		
		if (_nOption_Repeat > 30)
			_nOption_Repeat = _nOption_Repeat - 30;
		
		_Ulbl_Repeat.text = _nOption_Repeat.ToString();
		Show_RepeatCounter();
	}
	
	public void Option_SetRepeat_Minus()
	{
		_nOption_Repeat --;
		
		if (_nOption_Repeat <= 0)
			_nOption_Repeat = _nOption_Repeat + 30;
		
		_Ulbl_Repeat.text = _nOption_Repeat.ToString();
		Show_RepeatCounter();
	}



	// Option Interval ====================================================

	public void Option_SetInterval_Plus()
	{
		_nOption_Interval ++;
		
		if (_nOption_Interval > 120)
			_nOption_Interval = _nOption_Interval - 120;
		
		_Ulbl_Interval.text = _nOption_Interval.ToString();
	}
	
	public void Option_SetInterval_Minus()
	{
		_nOption_Interval --;
		
		if (_nOption_Interval <= 0)
			_nOption_Interval = _nOption_Interval + 120;
		
		_Ulbl_Interval.text = _nOption_Interval.ToString();
	}





	// Option Duration ====================================================

	public void Option_SetDuration_Plus()
	{
		_nOption_Duration ++;
		
		if (_nOption_Duration > 60)
			_nOption_Duration = _nOption_Duration - 60;
		
		_Ulbl_Duration.text = _nOption_Duration.ToString();
	}
	
	public void Option_SetDuration_Minus()
	{
		_nOption_Duration --;
		
		if (_nOption_Duration <= 0)
			_nOption_Duration = _nOption_Duration + 60;
		
		_Ulbl_Duration.text = _nOption_Duration.ToString();
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












	void OnConnected(object sender, EventArgs e)
	{
		IsConnect_Robot = true;
		_UILblRobotMessage.text = "Connceted"; // NGUI
		CheckStartButton(); // NGUI
	}
	
	void OnConnectionFailed(object sender, EventArgs e)
	{
		IsConnect_Robot = false;
		_UILblRobotMessage.text = "Failed"; // NGUI
		_goBtnConnect.SetActive(true);		// NGUI
		CheckStartButton(); // NGUI
	}
	
	void OnDisconnected(object sender, EventArgs e)
	{
		IsConnect_Robot = false;
		_UILblRobotMessage.text = "Disconnected"; // NGUI
		_goBtnConnect.SetActive(true);		// NGUI
		CheckStartButton(); // NGUI

		Invoke("Clear_Ports", 0.1f);
		Invoke("Search_Ports", 0.2f);
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

