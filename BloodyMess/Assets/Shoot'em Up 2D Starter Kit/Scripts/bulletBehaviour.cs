using UnityEngine;
using System.Collections;

public class bulletBehaviour : MonoBehaviour {

	[Tooltip("moving speed")] public float speed;
	[Tooltip("damage, which bullet makes")] public float damage;
	float actualSpeed;
	Vector2 dir; // stores direction for bullet to fly
	Collider2D t; // stores hitted collider
	[Tooltip("stores animator")] public Animator an;
	[Tooltip("Gameobjects with this layers will be hitted")] public LayerMask target;

	[Tooltip("stores sprite renderer")] public SpriteRenderer sr;
	bool hitted; // if bullet hitted something


	[Tooltip("Sound made on contact")] public AudioClip audioHit = null;

	private AudioManager audioManager = new AudioManager();
	private bool bulletHitting = false;


	public void applyDirection (Vector2 direction, float angle, Vector3 place) // applies shot info from weapon
	{
		hitted = false;
		transform.position = place; // puts bullet in a proper place
		actualSpeed = speed; 
		dir = direction; // sets direction
		transform.rotation = Quaternion.Euler (0,0, angle); // rotates bullet according to direction
		an.SetInteger ("state", 0); // sets bullet to default animation
	}

	void Update ()
	{
		transform.position += new Vector3(dir.x, dir.y,0) * actualSpeed * Time.deltaTime; // moves bullet
		t = Physics2D.OverlapArea (transform.position - new Vector3 (15,0), transform.position + new Vector3 (15, -180), target); // checks for victim
		if (t && !hitted) // if bullet findes target
		{
			hitted = true; // hitted
			t.transform.gameObject.SendMessage("applyDamage", damage, SendMessageOptions.DontRequireReceiver); // tells victim to apply damage
			an.SetInteger ("state", 1); // sets hit animation
			Invoke ("disable", 0.7f); // disables bullet in 0.7 seconds
			if (!bulletHitting)
			{
				GetComponent<AudioSource>().PlayOneShot(audioHit);
				Invoke("resetSound", audioHit.length);
			}

			actualSpeed = 0; // stops bullet
		}
		if (!sr.isVisible) // if bullet is out of view
		{
			disable(); // disables bullet
		}  
	}

	void disable () // disables bullet
	{
		gameObject.SetActive (false);
	}

	void resetBulletSound()
	{
		bulletHitting = false;
	}
}
