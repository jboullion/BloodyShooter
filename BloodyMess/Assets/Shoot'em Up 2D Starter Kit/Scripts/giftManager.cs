using System;
using UnityEngine;
using System.Collections;

public class giftManager : MonoBehaviour {

	[Tooltip("put gift button here")] public GameObject giftButton;
	[Tooltip("how much coins gift contains of")] public float plusCoins;
	[Tooltip("After this time player will get gift again (hours)")] public double giftIntervalHours;
	gamePlayManager gpm;

	// Use this for initialization
	void Start () {
		gpm = GameObject.FindGameObjectWithTag ("gpm").GetComponent<gamePlayManager> ();
	}

	// Update is called once per frame
	void OnEnable () 
	{
		checkForAGiftTime (); // checks if it is time for new gift
	}

	void checkForAGiftTime ()
	{
		int comparison = System.DateTime.Now.CompareTo (getGiftTime ());
		if (comparison >= 0) 
		{
			presentGift ();
		} else 
		{
			giftButton.SetActive (false);
		}
	}

	void saveNewGiftTime()
	{
		DateTime giftTime = System.DateTime.Now.AddHours (giftIntervalHours);
		string gt = "giftDate";
		PlayerPrefs.SetInt (gt + "_day", giftTime.Day);
		PlayerPrefs.SetInt (gt + "_month", giftTime.Month);
		PlayerPrefs.SetInt (gt + "_year", giftTime.Year);
		PlayerPrefs.SetInt (gt + "_hour", giftTime.Hour);
		PlayerPrefs.SetInt (gt + "_minute", giftTime.Minute);
		PlayerPrefs.SetInt (gt + "_second", giftTime.Second);
	}

	DateTime getGiftTime ()
	{
		string gt = "giftDate";
		int day = PlayerPrefs.GetInt (gt + "_day");
		int month = PlayerPrefs.GetInt (gt + "_month");
		int year = PlayerPrefs.GetInt (gt + "_year");
		int hour = PlayerPrefs.GetInt (gt + "_hour");
		int minute = PlayerPrefs.GetInt (gt + "_minute");
		int second = PlayerPrefs.GetInt (gt + "_second");
		if (day < 1 || month < 1 || year < 1) 
		{
			return new DateTime (1, 1, 1);
		} else {
			return new DateTime (year, month, day,hour,minute,second);
		}
	}

	void presentGift ()
	{
		giftButton.SetActive (true);
	}

	public void openGift ()
	{
		saveNewGiftTime ();
		gpm.applyCoins (plusCoins);
		giftButton.SetActive (false);
	}

}
