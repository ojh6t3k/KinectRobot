using UnityEngine;
using System.Collections;
using System;
using UnityRobot;





public enum EMode
{
	TITLE,
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
	// NGUI ========================

	int _nAvatarTimer = 0;


	public CMDModule		_CMDModule;


	public AvatarJoint[] _AvatarJoints;










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
		_goUI_Waiting.SetActive(false);
		_goObj_Robot.SetActive(false);
		_goRobot.transform.position = new Vector3(0f, 200f, 0f);
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
		CancelInvoke("CheckPlayer");
		_UILblTimer.enabled = false;
		CheckPlayer();

		FollowOnOff(false);
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
	}



	public void SelectGesture(int p_Num)
	{
		if (p_Num == -1)
		{
			foreach (GameObject Btn in _goGestureBtn)
			{
				Btn.SetActive(true);
			}
			return;
		}

		_goCursor.SetActive(false);

		foreach (GameObject Btn in _goGestureBtn)
		{
			Btn.SetActive(false);
		}

		_goGestureBtn[p_Num].SetActive(true);

		_CMDModule.Value = (Byte)(p_Num + 1);


		CheckGestureEnd();
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
		_nAvatarTimer = 10;
		AvatarCountDown();

		//FollowOnOff(true);
	}



	void AvatarCountDown()
	{
		if (_nAvatarTimer <= -1)
		{
			GoWaiting();
			return;
		}

		_UILblTimer.text = _nAvatarTimer.ToString();

//		if (_KinectManager.IsPlayerCalibrated(_KinectManager.GetPlayer1ID()))
//			_nAvatarTimer --;

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
			_goLinePerson.SetActive(true);
			_goRobot.transform.position = new Vector3(0f, 200f, 0f);
			_UILblMessage.text = "Waiting Player";
		}
		
		Invoke("CheckPlayer", 0.1f);
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

