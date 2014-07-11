using UnityEngine;
using System.Collections;
using System;
using UnityRobot;













public class MainControl_Bremen : MonoBehaviour 
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

	
	public GameObject[]		_goGestureBtn = new GameObject[10];
	public GameObject		_goLinePerson;
	public GameObject		_goRobot;
	public UILabel			_UILblMessage;
	public UILabel			_UILblTimer;
	String					_strMode = "";

	public Camera			_UICamera;
	public Transform		_trRobotHand_L;
	public Transform		_trRobotHand_R;
	public GameObject		_goHandCollider_L;
	public GameObject		_goHandCollider_R;

	Vector3 _v3ViewportL;
	Vector3 _v3ViewportR;

	public GameObject[]		_goGem = new GameObject[6];



	// NGUI ========================



	int _nGameTimer = 0;


	public CMDModule		_CMDModule;











	// Start ------------------------------------------------------------------------------
	void Start ()
	{
		_RobotProxy.OnConnected += OnConnected;
		_RobotProxy.OnConnectionFailed += OnConnectionFailed;
		_RobotProxy.OnDisconnected += OnDisconnected;
		_RobotProxy.OnSearchCompleted += OnSearchCompleted;

		_goBtnStart.SetActive(false);
		_goRobot.transform.position = new Vector3(0f, 200f, 0f);

		_RobotProxy.PortSearch();
		CheckKinect();

		GoTitle();
	}




	// Update ----------------------------------------------------------------------------------
	void Update ()
	{
		Update_HandsControl();
	}





	void Update_HandsControl()
	{
		_v3ViewportL = Camera.main.WorldToViewportPoint(_trRobotHand_L.position);
		_v3ViewportR = Camera.main.WorldToViewportPoint(_trRobotHand_R.position);

		_goHandCollider_L.transform.position = new Vector3(_UICamera.ViewportToWorldPoint(_v3ViewportL).x,
		                                                   _UICamera.ViewportToWorldPoint(_v3ViewportL).y, _goGem[0].transform.position.z);
		_goHandCollider_R.transform.position = new Vector3(_UICamera.ViewportToWorldPoint(_v3ViewportR).x,
		                                                   _UICamera.ViewportToWorldPoint(_v3ViewportR).y, _goGem[0].transform.position.z);

		_goHandCollider_L.transform.localScale = Vector3.one * (_trRobotHand_L.position.z + 2f) * 0.6f;
		_goHandCollider_R.transform.localScale = Vector3.one * (_trRobotHand_R.position.z + 2f) * 0.6f;
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
	}



	public void GoTitle()
	{
		_eCurMode = EMode.TITLE;
		_goUI_Title.SetActive(true);
		_goUI_Waiting.SetActive(false);
		_goObj_Robot.SetActive(false);
		_goRobot.transform.position = new Vector3(0f, 200f, 0f);
	}


	public void GoGame()
	{
		_eCurMode = EMode.WAITING;
		_goUI_Title.SetActive(false);
		_goUI_Waiting.SetActive(true);
		_goObj_Robot.SetActive(true);
		CancelInvoke("CheckPlayer");
		CheckPlayer();

		_nGameTimer = 30;
		GameCountDown();
	}







	void GameCountDown()
	{
		if (_nGameTimer <= -1)
		{
			GoTitle();
			return;
		}

		_UILblTimer.text = _nGameTimer.ToString();

		if (_KinectManager.IsPlayerCalibrated(_KinectManager.GetPlayer1ID()))
			_nGameTimer --;

		CancelInvoke("GameCountDown");
		Invoke("GameCountDown", 1f);
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

