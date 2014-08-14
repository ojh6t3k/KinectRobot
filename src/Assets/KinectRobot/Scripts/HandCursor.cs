using UnityEngine;
using System.Collections;

public class HandCursor : MonoBehaviour 
{
	public MainControl _MainControl;
	public MainControl_Internal _MainControl_Internal;
	public Camera NGUI_Camera;
	public KinectManager _KinectManager;
	public SimpleGestureListener _SimpleGestureListener;
	public UISprite _UIProgress;


	bool _bIsClick_BugFix = false;

	private Vector3 _v3CursorPosition = Vector3.zero;
	




	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (_SimpleGestureListener._bIsUserDetected)
		{
			_v3CursorPosition = _KinectManager.v3CursorScreenPosition;
			_UIProgress.fillAmount = (_SimpleGestureListener._fProgress - 0.3f) * (1f / 0.69f);
		}
		else
		{
			_v3CursorPosition = new Vector3(0f, 0f, 0f);
			_UIProgress.fillAmount = 0f;
		}
		transform.position = Vector3.Lerp(transform.position, NGUI_Camera.ViewportToWorldPoint(_v3CursorPosition), 30f * Time.deltaTime);
	}




	// OnTriggerEnter ----------------------------
	void OnTriggerStay (Collider p_Hit) 
	{
		if ((_SimpleGestureListener._bIsClick) && (!_bIsClick_BugFix))
		{
			_bIsClick_BugFix = _SimpleGestureListener._bIsClick;

			if (p_Hit.gameObject.name == "Btn_Gesture")
			{
				if (_MainControl != null)
					_MainControl.GoGesture();
				if (_MainControl_Internal != null)
					_MainControl_Internal.GoGesture();
			}
			else if (p_Hit.gameObject.name == "Btn_Avatar")
			{
				if (_MainControl != null)
					_MainControl.GoAvatar();
				if (_MainControl_Internal != null)
					_MainControl_Internal.GoAvatar();
			}
//			else if (p_Hit.gameObject.name == "Btn_Back")
//				_MainControl.GoWaiting();


			SelectGestureBtn(p_Hit.gameObject);

			if (_MainControl != null)
				_MainControl.Sound_Click();
			if (_MainControl_Internal != null)
				_MainControl_Internal.Sound_Click();
		}


		if (!_SimpleGestureListener._bIsClick)
			_bIsClick_BugFix = false;
	}






	public void SelectGestureBtn(GameObject p_Btn)
	{
		if(_MainControl != null)
		{
			if (p_Btn.name == "Btn_Gesture1")
				_MainControl.SelectGesture(0);
			else if (p_Btn.name == "Btn_Gesture2")
				_MainControl.SelectGesture(1);
			else if (p_Btn.name == "Btn_Gesture3")
				_MainControl.SelectGesture(2);
			else if (p_Btn.name == "Btn_Gesture4")
				_MainControl.SelectGesture(3);
			else if (p_Btn.name == "Btn_Gesture5")
				_MainControl.SelectGesture(4);
			else if (p_Btn.name == "Btn_Gesture6")
				_MainControl.SelectGesture(5);
			else if (p_Btn.name == "Btn_Gesture7")
				_MainControl.SelectGesture(6);
			else if (p_Btn.name == "Btn_Gesture8")
				_MainControl.SelectGesture(7);
			else if (p_Btn.name == "Btn_Gesture9")
				_MainControl.SelectGesture(8);
			else if (p_Btn.name == "Btn_Gesture10")
				_MainControl.SelectGesture(9);
		}

		if(_MainControl_Internal != null)
		{
			if (p_Btn.name == "Btn_Gesture1")
				_MainControl_Internal.SelectGesture(0);
			else if (p_Btn.name == "Btn_Gesture2")
				_MainControl_Internal.SelectGesture(1);
			else if (p_Btn.name == "Btn_Gesture3")
				_MainControl_Internal.SelectGesture(2);
			else if (p_Btn.name == "Btn_Gesture4")
				_MainControl_Internal.SelectGesture(3);
			else if (p_Btn.name == "Btn_Gesture5")
				_MainControl_Internal.SelectGesture(4);
			else if (p_Btn.name == "Btn_Gesture6")
				_MainControl_Internal.SelectGesture(5);
			else if (p_Btn.name == "Btn_Gesture7")
				_MainControl_Internal.SelectGesture(6);
			else if (p_Btn.name == "Btn_Gesture8")
				_MainControl_Internal.SelectGesture(7);
			else if (p_Btn.name == "Btn_Gesture9")
				_MainControl_Internal.SelectGesture(8);
			else if (p_Btn.name == "Btn_Gesture10")
				_MainControl_Internal.SelectGesture(9);
		}
	}










}
