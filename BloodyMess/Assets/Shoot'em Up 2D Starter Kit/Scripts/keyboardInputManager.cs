using UnityEngine;
using System.Collections;

public class keyboardInputManager : MonoBehaviour {

	[Tooltip("")] public characterBehaviour p;

	gamePlayManager gpm;

	float a;
	float b;
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
		if (Input.GetKey (KeyCode.D) && Input.GetKey (KeyCode.W)) {
			p.move (new Vector2 (1, 1).normalized);
		} else if (Input.GetKey (KeyCode.D) && Input.GetKey (KeyCode.S)) {
			p.move (new Vector2 (1, -1).normalized);
		} else if (Input.GetKey (KeyCode.A) && Input.GetKey (KeyCode.W)) {
			p.move (new Vector2 (-1, 1).normalized);
		} else if (Input.GetKey (KeyCode.A) && Input.GetKey (KeyCode.S)) {
			p.move (new Vector2 (-1, -1).normalized);
		} else if (Input.GetKey (KeyCode.D)) {
			p.move (Vector2.right);
		} else if (Input.GetKey (KeyCode.A)) {
			p.move (-Vector2.right);
		} else if (Input.GetKey (KeyCode.W)) {
			p.move (Vector2.up);
		} else if (Input.GetKey (KeyCode.S)) {
			p.move (-Vector2.up);
		} else if (Input.GetKeyUp (KeyCode.S)) 
		{
			p.stopY ();
		} else if (Input.GetKeyUp (KeyCode.A)) 
		{
			p.stopX ();
		} else if (Input.GetKeyUp (KeyCode.D)) 
		{
			p.stopX ();
		} else if (Input.GetKeyUp (KeyCode.W)) 
		{
			p.stopY ();
		}

		if (Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.UpArrow)) {
			p.attack (new Vector2 (1, 1).normalized);
		} else if (Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.DownArrow)) {
			p.attack (new Vector2 (1, -1).normalized);
		} else if (Input.GetKey (KeyCode.LeftArrow) && Input.GetKey (KeyCode.UpArrow)) {
			p.attack (new Vector2 (-1, 1).normalized);
		} else if (Input.GetKey (KeyCode.LeftArrow) && Input.GetKey (KeyCode.DownArrow)) {
			p.attack (new Vector2 (-1, -1).normalized);
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			p.attack (Vector2.right);
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			p.attack (-Vector2.right);
		} else if (Input.GetKey (KeyCode.UpArrow)) {
			p.attack (Vector2.up);
		} else if (Input.GetKey (KeyCode.DownArrow)) {
			p.attack (-Vector2.up);
		} else if (Input.GetKeyUp (KeyCode.DownArrow)) 
		{
			p.triggerUp ();
		} else if (Input.GetKeyUp (KeyCode.LeftArrow)) 
		{
			p.triggerUp ();
		} else if (Input.GetKeyUp (KeyCode.RightArrow)) 
		{
			p.triggerUp ();
		} else if (Input.GetKeyUp (KeyCode.UpArrow)) 
		{
			p.triggerUp ();
		}

	} 

	public void setPlayer (GameObject newPlayer)
	{
		p = newPlayer.GetComponent<characterBehaviour> ();	
	}

}
