using UnityEngine;
using System.Collections;

public class moveBehaviour : MonoBehaviour {

	[Tooltip("movement speed")] public float speed;
	Vector2 direction; // stores direction

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		move ();
	}

	void move ()
	{
		transform.position += (Vector3) direction * speed;
	}

	public void applyDirection (Vector2 newDirection)
	{
		direction = newDirection;
	}

}
