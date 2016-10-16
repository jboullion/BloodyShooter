using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class gamePlayManager : MonoBehaviour {

	[Tooltip("put game buttons container here")] public GameObject gameButtons;
	[Tooltip("put menu container here")] public GameObject menu;
	[Tooltip("put character selector container here")] public GameObject characterSelector;
	[Tooltip("put map selector container here")] public GameObject mapSelector;
	[Tooltip("put inGame menu container here")] public GameObject inGameMenu;
	[Tooltip("put pop up gameboject here")] public GameObject popUp;

	[Tooltip("put characters here")] public character[] characters;
	[Tooltip("put maps here")] public map[] maps;
	[Tooltip("put level designer here")] public levelDesigner ld;

	[Tooltip("how much enemies need to be killed to get gift")] public int enemiesForGift;
	[Tooltip("gift for killing enemies")] public float giftSize;

	[Tooltip("put game sounds here")] public AudioClip[] gameSounds;
	// 0 - coinSound
	// 1 - failSound
	[Tooltip("put background music tracks here")] public AudioClip[] bgm;
	[Tooltip("put enemy sounds here")] public AudioClip[] enemySounds;
	AudioSource au; // stores audio soure
	AudioSource bgmau; // stores background music audiosource

	GameObject player; // stores player gameobject
	GameObject[] inputManagers; // stores input managers

	int characterNum; // number of character in array, which player plays
	[HideInInspector] public int mapNum; // number of map in array, which player plays
	Image avatar; // used character's avatar
	GameObject nextCharacterButton; // stores next character button
	GameObject prevCharacterButton;// stores previous character button
	GameObject nextMapButton; // stores next map button
	GameObject prevMapButton; // stores prev map button
	GameObject enableAudioButton; // stores enable audio button
	GameObject disableAudioButton; // stores next character button
	Text coinsBar; // stores coins bar text component
	Text distanceBar; // stores distance bar text component

	float startPosition; // initial character's position
	int enemiesKilled; // how much enemies are killed
	float distance; // distance character made from start position
	float maxDistance; // record distance

	// Use this for initialization
	void Start () {
		mapNum = PlayerPrefs.GetInt ("mapNum"); //gets selected map's number in array
		characterNum = PlayerPrefs.GetInt ("characterNum"); //gets selected character's number in array
		ld.mapNum = mapNum; // tells level designer what map to draw
		ld.refreshMap (); // tells level designer to clear map and build new
		Time.timeScale = 1f;
		inputManagers = GameObject.FindGameObjectsWithTag ("im"); // get input managers
		avatar = characterSelector.transform.FindChild ("avatar").GetComponent<Image>(); // avatar object
		nextCharacterButton = characterSelector.transform.FindChild ("right").gameObject; // determines variables
		prevCharacterButton = characterSelector.transform.FindChild ("left").gameObject;
		nextMapButton = mapSelector.transform.FindChild ("right").gameObject;
		prevMapButton = mapSelector.transform.FindChild ("left").gameObject;
		disableAudioButton = inGameMenu.transform.FindChild ("disableAudio").gameObject;
		enableAudioButton = inGameMenu.transform.FindChild ("enableAudio").gameObject;
		coinsBar = GameObject.Find ("money").transform.FindChild("Text").gameObject.GetComponent<Text>();
		coinsBar.text = "" + PlayerPrefs.GetFloat("coins");
		distanceBar = gameButtons.transform.FindChild ("distanceBar").gameObject.GetComponent<Text> ();
	
		prepareCharacters ();
		prepareMaps ();

		au = gameObject.AddComponent<AudioSource> (); // creates audiosource for short sounds 
		au.playOnAwake = false; 
		au.loop = false;

		bgmau = gameObject.AddComponent <AudioSource> (); // creates audiosource for background music
		bgmau.volume = 0.5f;
		bgmau.playOnAwake = false;
		bgmau.loop = true;
		
		maxDistance = PlayerPrefs.GetFloat ("maxDistance"); // gets record distance
		startPosition = ld.mapProgressPosition.x; // gets start position
	}

	void FixedUpdate()
	{
		if (player) // if it is a player
		refreshDistanceBar (); // refreshes distance info
	} 

	void refreshDistanceBar ()
	{
		distance = (player.transform.position.x - startPosition)/100; // we divide ditance with 100 to make it look closer to meters
		distanceBar.text = distance.ToString("0") + " / " + maxDistance.ToString("0"); // updates text bar
	}

	public void prepareCharacters () //opens available characters in character selector
	{
		for (int i = 0; i < characters.Length; i++) 
		{
			if (PlayerPrefs.GetInt ("c" + i) == 1 || characters[i].price == 0) // if character is opened
			{
				characters[i].opened = true;
			}
		}
	}

	public void enemyDied () // player killed enemy
	{
		enemiesKilled++; // counts new killed enemy

		if (enemiesKilled % enemiesForGift == 0) 
		{
			applyCoins (giftSize);
		}

		playEnemySound (); // optional audio effect
	}

	void playEnemySound ()
	{
		playOnce (enemySounds [Random.Range (0, enemySounds.Length)]);
	}

	public void prepareMaps () //opens available maps in map selector
	{
		for (int i = 0; i < maps.Length; i++) 
		{
			if (PlayerPrefs.GetInt ("m" + i) == 1 || maps[i].price == 0) // if map is opened
			{
				maps[i].opened = true; // sets map as available
			}
		}
	}

	public void buyCharacter () // buy character in character selector
	{
		float p = characters [characterNum].price;
		if (PlayerPrefs.GetFloat ("coins") >= p) // if player has enougth coins
		{
			applyCoins (-p);
			characters[characterNum].opened = true;
			PlayerPrefs.SetInt("c" + characterNum, 1);
			PlayerPrefs.SetInt ("characterNum", characterNum);
			refreshCharacterSelection ();
		}
	}

	public void buyMap() // buy map in map selector
	{
		float p = maps [mapNum].price;
		if (PlayerPrefs.GetFloat ("coins") >= p) // if player has enought
		{
			applyCoins (-p);
			maps[mapNum].opened = true;
			PlayerPrefs.SetInt("m" + mapNum, 1);
			refreshMapSelection ();
		}
	}

	public void applyCoins (float plusMoney) // adds coins 
	{
		float newCoins = PlayerPrefs.GetFloat ("coins") + plusMoney;
		PlayerPrefs.SetFloat ("coins", newCoins); // saves coins number
		coinsBar.text = "" + newCoins;	

		popUpText ("" + plusMoney);

		playOnce (gameSounds[0]);

	}

	void popUpText (string text) // pops up a bar 
	{
		popUp.SetActive (true); // enables text bar
		popUp.transform.FindChild ("Text").GetComponent<Text> ().text = text; // puts text in text bar
		Invoke ("disablePopUp", 0.5f); // disables pop up in 0.5 secs
	}

	void disablePopUp ()
	{
		popUp.SetActive (false);
	}

	public void startGame () // starts game process
	{
		player = Instantiate (characters[characterNum].c, ld.mapProgressPosition, Quaternion.identity) as GameObject; // creates playable charbleacter
		player.GetComponent<characterBehaviour> ().isPlayer = true; // marks character as a player
		ld.mapNum = PlayerPrefs.GetInt ("mapNum"); // tells level designer which map to draw
		ld.player = player; // tells level selector who is player's character
		gameButtons.SetActive (true); // enables game buttons
		menu.SetActive (false); // disables menu
		inGameMenu.SetActive (false); // disables inGame menu

		Camera.main.transform.parent.gameObject.GetComponent<cameraContainerScript> ().setPlayer (player); // tells camera which character to follow
		foreach (GameObject im in inputManagers) // tells every input manager which character should be controlled
		{
			im.SendMessage ("setPlayer", player); 
		}

		Time.timeScale = 1f;

		if (bgm.Length > 0) // if there are background music
		{
		bgmau.Stop ();
		bgmau.clip = bgm[Random.Range (0,bgm.Length)]; // sets random background track
		bgmau.Play ();
		}

	}

	public void restartLevel () // function restarts 
	{
		if (bgm.Length > 0) // if there are background music
		{
			bgmau.Stop ();
			bgmau.clip = bgm[Random.Range (0,bgm.Length)]; // select random track
			bgmau.Play (); // play music
		}
		ld.refreshMap (); // deletes map and draws new
		player.transform.position = ld.mapProgressPosition;
		player.GetComponent<characterBehaviour> ().restart (); // restarts character
		gameButtons.SetActive (true); // enables game buttons
		menu.SetActive (false); // disables menu
		inGameMenu.SetActive (false); // disables in game menu
		enemiesKilled = 0; // clears kill score
	}

	public void openMenu () // open menu function
	{
		Time.timeScale = 1f;
		gameButtons.SetActive (false);
		inGameMenu.SetActive (false);
		menu.SetActive (true);
		characterSelector.SetActive (false);
		mapSelector.SetActive (false);

		bgmau.Pause ();
	}

	public void openInGameMenu () // open in game menu
	{
		Time.timeScale = 0;
		gameButtons.SetActive (false);
		inGameMenu.SetActive (true);
		menu.SetActive (false);
		characterSelector.SetActive (false);
		mapSelector.SetActive (false);
		bgmau.Pause();
	}

	public void openCharacterSelection () // open character selection
	{
		gameButtons.SetActive (false);
		menu.SetActive (false);
		inGameMenu.SetActive (false);
		characterSelector.SetActive (true);
		mapSelector.SetActive (false);
		refreshCharacterSelection ();
	}

	public void openMapSelection () // open map selection function
	{
		gameButtons.SetActive (false);
		menu.SetActive (false);
		inGameMenu.SetActive (false);
		characterSelector.SetActive (false);
		mapSelector.SetActive (true);
		refreshMapSelection ();
	}

	public void nextCharacter () // turns to next character in a character selection
	{
		characterNum++;
		refreshCharacterSelection ();
	}

	public void prevCharacter () // turns to prev character in a character selection
	{
		characterNum--;
		refreshCharacterSelection ();
	}

	public void nextMap () // turns to next map in a map selection
	{
		mapNum++;
		refreshMapSelection ();
		ld.refreshMap ();
	}
	
	public void prevMap () // turns to prev map in a prev selection
	{
		mapNum--;
		refreshMapSelection ();
		ld.refreshMap ();
	}

	void refreshMapSelection ()
	{
		ld.mapNum = mapNum;
		if (mapNum >= maps.Length - 1) // if it is the last map
		{
			nextMapButton.SetActive (false);
		} else 
		{
			nextMapButton.SetActive (true);
		}
		
		if (mapNum == 0) // if it is the first map
		{
			prevMapButton.SetActive (false);
		} else 
		{
			prevMapButton.SetActive (true);
		}
		
		GameObject buyButton = mapSelector.transform.FindChild ("buy").gameObject; // determines selection buttons
		GameObject selectButton = mapSelector.transform.FindChild ("select").gameObject; // determines selection buttons
		
		if (!maps [mapNum].opened) // if map is not available
		{
			selectButton.SetActive (false);
			buyButton.SetActive (true);
			buyButton.transform.FindChild ("Text").gameObject.GetComponent<Text> ().text = "" + maps [mapNum].price;
		} else // if map is available
		{
			selectButton.SetActive (true);
			buyButton.SetActive (false);
		}
	}

	void refreshCharacterSelection ()
	{
		avatar.sprite = characters [characterNum].avatar;
		if (characterNum >= characters.Length - 1) 
		{
			nextCharacterButton.SetActive (false);
		} else 
		{
			nextCharacterButton.SetActive (true);
		}

		if (characterNum == 0) 
		{
			prevCharacterButton.SetActive (false);
		} else 
		{
			prevCharacterButton.SetActive (true);
		}

		GameObject buyButton = characterSelector.transform.FindChild ("buy").gameObject;
		GameObject selectButton = characterSelector.transform.FindChild ("select").gameObject;
		
		if (!characters [characterNum].opened) 
		{

			selectButton.SetActive (false);
			buyButton.SetActive (true);
			buyButton.transform.FindChild ("Text").gameObject.GetComponent<Text> ().text = "" + characters [characterNum].price;
		} else 
		{
			selectButton.SetActive (true);
			buyButton.SetActive (false);
		}

	}

	public void selectCharacter () // sets character as a player's character
	{
		PlayerPrefs.SetInt ("characterNum", characterNum);
		openMenu ();
	}

	public void selectMap () // sets map
	{
		PlayerPrefs.SetInt ("mapNum", mapNum);
		openMenu ();
	}

	public void backToMenu ()
	{
		Application.LoadLevel (Application.loadedLevelName);
	}

	public void resumeGame () // continue game button
	{
		gameButtons.SetActive (true);
		menu.SetActive (false);
		inGameMenu.SetActive (false);
		characterSelector.SetActive (false);
		Time.timeScale = 1f;
		bgmau.UnPause ();
	}

	public void fail () // fail function
	{

		if (maxDistance < distance)
		{
			PlayerPrefs.SetFloat("maxDistance", distance);
			maxDistance = distance;
		}

		bgmau.Stop ();
		playOnce (gameSounds[1]);
		Invoke ("openFailMenu", 1f);
	}

	public void openFailMenu ()
	{
		openInGameMenu ();
	}

	public void quit () // closes app
	{
		Application.Quit ();
	}

	public void disableAudio () // mutes all audio
	{
		AudioSource[] allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];

		for (int i = 0; i < allAudioSources.Length; i++)
		{
			allAudioSources[i].volume = 0;
		}

		disableAudioButton.SetActive (false);
		enableAudioButton.SetActive (true);

	}

	public void rate ()
	{
		Application.OpenURL("http://unity3d.com/");
	}

	public void openHightScore ()
	{
		Application.OpenURL("http://unity3d.com/");
	}

	public void enableAudio () // unmutes all audio
	{
		AudioSource[] allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
		
		for (int i = 0; i < allAudioSources.Length; i++)
		{
			if (allAudioSources[i] == bgmau)
			{
				allAudioSources[i].volume = 0.5f;
			} else 
			{
				allAudioSources[i].volume = 1;
			}
		}

		disableAudioButton.SetActive (true);
		enableAudioButton.SetActive (false);
		
	}

	void playOnce (AudioClip sound) // play sound, if sound exists
	{
		if (sound)
			au.PlayOneShot (sound);
	}

}	

public static class ExtensionMethods // extension, which allows other scripts to find gameplay script
{
	public static gamePlayManager findGPM(this GameObject go)
	{
		GameObject get = GameObject.FindGameObjectWithTag("gpm");
		if (get)
		{
			gamePlayManager gpm = get.GetComponent<gamePlayManager>();
			return gpm;
		}
		return null;
	}
}

[System.Serializable]
public class character // character class
{
	[Tooltip("character's name")] public string name;
	[Tooltip("character's prefab")] public GameObject c;
	[Tooltip("if is available")] public bool opened;
	[Tooltip("character's image")] public Sprite avatar;
	[Tooltip("character's price")] public float price;
}

[System.Serializable]
public class map 
{
	[Tooltip("map's name")] public string name;
	[Tooltip("if is available")] public bool opened;
	[Tooltip("map's price")] public float price;
}