using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class zombieBehavior : MonoBehaviour {

	[Tooltip("health points")] public float health;
	[Tooltip("movement speed")] public float speed;

	[Tooltip("piece prefab (visual effect)")] public GameObject piece;
	[Tooltip("number of pieces to fall to")] public int numberOfPieces;
	List<GameObject> pieces;

	public Animator an;
	public SpriteRenderer s;
	public SpriteRenderer hs;
	public Animator ha;
	public Rigidbody2D rb;

	gamePlayManager gpm; // stores gameplay refference

	int k = 1;
	bool dead;

	// Use this for initialization
	void Awake () {

		prepareZombie ();

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		setLayer ();
	}

	public void prepareZombie ()
	{
		ha = gameObject.transform.GetChild (0).GetChild (0).gameObject.GetComponent<Animator> ();
		hs = gameObject.transform.GetChild (0).GetChild (0).gameObject.GetComponent<SpriteRenderer> ();

		pieces = new List<GameObject> ();
		for (int i = 0; i < numberOfPieces; i++)
		{
			pieces.Add (Instantiate(piece, Vector2.zero, Quaternion.identity) as GameObject);
			pieces [i].SetActive (false);
		}

		gpm = GameObject.FindGameObjectWithTag ("gpm").GetComponent<gamePlayManager>();

	}

	void setLayer ()
	{
		float p = transform.position.y * 10;
		s.sortingOrder = -Mathf.FloorToInt (p);
		hs.sortingOrder = -Mathf.FloorToInt (p - 1);
	}

	public void move (Vector3 pos)
	{
		Vector3 direction = (pos - transform.position).normalized;
		an.SetInteger ("state", 1);
		ha.SetInteger ("state", 1);
		if (direction.x > 0) 
		{
			rotateRight ();
		} else 
		{
			rotateLeft ();
		}
		rb.velocity = direction * speed;
	}

	void rotateRight ()
	{
		transform.rotation = Quaternion.Euler (0, 0, 0);
		k = 1;
	}

	void rotateLeft ()
	{
		transform.rotation = Quaternion.Euler (0, 180, 0);
		k = -1;
	}

	public void applyDamage (float plusHealth)
	{
		health += plusHealth;
		if (health <= 0 && !dead)
		{
			die ();
		}
	}

	void die ()
	{
		dead = true;
		throwPieces ();
		gpm.enemyDied ();
		//this.gameObject.SetActive (false);
	}

	void throwPieces () // zombie falls to pieces
	{
		for (int i = 0; i < numberOfPieces; i++) 
		{
			pieces [i].transform.position = transform.position;
			pieces [i].SetActive (true);
			pieces [i].GetComponent<moveBehaviour> ().applyDirection(Random.insideUnitCircle);
		}
		Invoke ("disablePiece", 0.55f);
	}

	void disablePiece ()
	{
		for (int i = 0; i < numberOfPieces; i++) 
		{
			pieces [i].gameObject.SetActive (false);
		}
	}

	public void restart ()
	{
		for (int i = 0; i < numberOfPieces; i++)
		{
			pieces [i].SetActive (false);
		}
		dead = false;
		health = 100;
	}

}
