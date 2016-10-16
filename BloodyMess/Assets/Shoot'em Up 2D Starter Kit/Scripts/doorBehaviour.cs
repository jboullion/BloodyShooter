using UnityEngine;
using System.Collections;

public class doorBehaviour : MonoBehaviour {

	[Tooltip("radius to check for passer")] public float radius;
	[Tooltip("Gameobjects with this layers can pass")] public LayerMask passers;
	bool opened;
	[Tooltip("image of opened door")] public Sprite spriteOpened;
	Sprite[] sprites;

	SpriteRenderer sr;

	// Use this for initialization
	IEnumerator Start () {

		sr = gameObject.GetComponent<SpriteRenderer> ();
		sprites = new Sprite[2];
		sprites [0] = sr.sprite;
		sprites [1] = spriteOpened;
		yield return new WaitForEndOfFrame ();
		sr.sortingOrder += 1;
	}
	
	// Update is called once per frame

	void checkForUsers()
	{
		Collider2D passer = Physics2D.OverlapCircle (transform.position, radius, passers);
		if (!passer) 
		{
			close ();
			CancelInvoke ("checkForUsers");
		}
	}

	public void open () // open door
	{
		if (!opened) {
			opened = true;
			sr.sprite = sprites [1]; 
			InvokeRepeating ("checkForUsers", 0.25f, 0.25f); // check if user passed
		}
	}

	void close () // close door
	{
		opened = false;
		sr.sprite = sprites [0];
	}

}
