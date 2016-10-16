using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class cameraContainerScript : MonoBehaviour {

	[Tooltip("")] public Transform player;
	[Tooltip("")] public List<Vector2> dots;
	[Tooltip("")] public float radius;
	[Tooltip("")] public float speed;
	
	// Update is called once per frame
	void Update () {
		if (player) // if there are player to follow
		followPlayer (); // follows player
	}

	void followPlayer ()
	{
		transform.position = player.position; // puts camera container to a player's position
	}

	public void setPlayer (GameObject newPlayer)
	{
		player = newPlayer.transform;
	}
		

}
