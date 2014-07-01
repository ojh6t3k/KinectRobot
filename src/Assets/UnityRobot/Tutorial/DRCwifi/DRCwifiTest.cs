using UnityEngine;
using System.Collections;

public class DRCwifiTest : MonoBehaviour
{
	public DRCwifi drcWifi;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnGUI()
	{
		if(drcWifi.Connected == false)
		{
			Rect guiRect = new Rect(10, 10, 300, 25);
			GUI.Label(guiRect, "Connecting...");
		}
		else
		{
			Texture2D image = drcWifi.image;
			if(image != null)
			{
				Rect guiRect = new Rect(20, 20, 320, 240);
				GUI.DrawTexture(guiRect, image);
			}
		}
	}
}
