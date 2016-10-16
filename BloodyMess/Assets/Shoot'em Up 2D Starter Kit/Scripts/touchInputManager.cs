using UnityEngine;
using System.Collections;

public class touchInputManager : MonoBehaviour {

	[Tooltip("stores character's script refference")] public characterBehaviour p;
	bool moveButtonPressed;
	Vector2 moveDelta;
	bool shootButtonPressed;
	[Tooltip("Max distance between finger position and button position to detect touch")] public float delta;

	Vector2 startMoveButtonPosition;
	Vector2 startShootButtonPosition;

	// Use this for initialization
	void Start () {
		startMoveButtonPosition = GameObject.Find ("Canvas").transform.FindChild ("gameUI").FindChild ("moveButton").position;
		startShootButtonPosition = GameObject.Find ("Canvas").transform.FindChild ("gameUI").FindChild ("shootButton").position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0) // if there are touches
		{
		for(int i = 0; i < Input.touchCount; i++)
		{
			Touch t = Input.GetTouch (i);
			Vector2 touchPos = t.position;

				if (Vector2.Distance (touchPos, startMoveButtonPosition) < Vector2.Distance (touchPos, startShootButtonPosition)) { // if touch is closer to move button
					if (Vector2.Distance (touchPos, startMoveButtonPosition) <= delta || moveButtonPressed) { // if move button is pressed

						moveButtonPressed = true;
						p.move ((touchPos - startMoveButtonPosition).normalized); // tells character to move
						if (t.phase == TouchPhase.Ended) // if player pulled finger
						{
							moveButtonPressed = false;
							p.stop (); // tells character to stop
						}
					}
				} else 
				{
					if (Vector2.Distance (touchPos, startShootButtonPosition) <= delta || shootButtonPressed) 
					{

						shootButtonPressed = true;
						p.attack ((touchPos - startShootButtonPosition).normalized); // tells character to attack
						if (t.phase == TouchPhase.Ended) // if player pulled finger
						{
							p.triggerUp ();
							shootButtonPressed = false;
						}
					}
				}
		}
		}
	}
	public void move (Transform t)
	{
		p.move (getDirection(t));
	}

	public void attack (Transform t)
	{
		p.attack (getDirection(t));
	}

	Vector2 getDirection (Transform t)
	{
		return (Camera.main.ScreenToWorldPoint(Input.mousePosition) - t.position).normalized;
	}

	public void setPlayer(GameObject newPlayer)
	{
		p = newPlayer.GetComponent<characterBehaviour> ();
	}

}
