using UnityEngine;
using System.Collections;

public class controllerInputManager : MonoBehaviour {

	[Tooltip("")] public characterBehaviour[] p;

	gamePlayManager gpm;

	float a;
	float b;

	public bool doDebug = false;
	public int DebugSpeed = 100;
	private int DEBUG_TIMER = 0;
	

	// Use this for initialization
	void Start () 
	{
		gpm = gameObject.findGPM ();
	}

	bool ifIsInteger (float num)
	{
		return Mathf.Approximately (num, Mathf.RoundToInt(num));
	}

	// Update is called once per frame
	void Update () {

		//p is set when game begins
		if (p[0])
		{
			//Be careful to only get the axis values once per update.
			float axis1 = Input.GetAxis("Axis 1");
			float axis2 = -Input.GetAxis("Axis 2"); //Up is negative values from controller, but it is positive values in game

			if (doDebug && DEBUG_TIMER >= DebugSpeed)
			{
				Debug.Log("1: " + axis1);
				Debug.Log("2: " + axis2);
				DEBUG_TIMER = 0;
			}

			//activate enemies
			foreach (characterBehaviour character in p) //the line the error is pointing to
			{
				
				//Move exactly how the controller moves.
				//This could probably be done more smoothly
				if (character.animator)
				{
					character.move(new Vector2(axis1, axis2).normalized);

					if (Input.GetKey(KeyCode.JoystickButton0) == true)
					{
						//A
						character.attack();
					}
					else if (Input.GetKey(KeyCode.JoystickButton0) != true)
					{
						character.triggerUp();
					}
				}
			}
			
			DEBUG_TIMER++;
		}
	} 

	public void setPlayer (GameObject newPlayer)
	{
		for (int i = 0; i < p.Length; i++)
		{
			p[i] = newPlayer.GetComponent<characterBehaviour>();
		}

	}

}
