using UnityEngine;
using System.Collections;

public class barrierBehaviour : MonoBehaviour {

	[Tooltip("Color material, which will be applied when barrier is hitted")] public Material damaged; 
	Material defaultMaterial; // stores default material
	[Tooltip("barrier's health")] public float health;
	 SpriteRenderer sr; // stores sprite renderer
	[Tooltip("order in layer offset")] public int offset; // play with it to find the one you need

	[Tooltip("put half-destroyed sprites here")] public Sprite[] sprites;
	// 0 -half destroyed

	[Tooltip("explosion class")] public explosible explosion;
	[Tooltip("put destroy audioclips here")] public AudioClip[] destroySounds;

	AudioSource au; // stores audio source
	bool destroyed; // if barrier is destroyed

	// Use this for initialization
	void Start () {

		// determines variables
		sr = gameObject.GetComponent<SpriteRenderer> ();
		sr.sortingOrder = -Mathf.FloorToInt ((transform.position.y + offset) * 10);
		defaultMaterial = sr.material;
		au = gameObject.GetComponent<AudioSource> ();

		createExplosions (); // creates array of visual explosion objects
	}

	public void applyDamage (float damage) // applies damage
	{
		health += damage;

		sr.material = damaged; // enables damaged material
		Invoke ("setDefaultMaterial", 0.25f); // sets default material back in 0.25 secs

		if (health <= 0) // if destroyed
		{
			destroy ();
		} else if (health < 50) // if half destroyed
		{
			breakDown ();
		}
	}

	void createExplosions () // creates array of explosions
	{
		if (explosion.numberOfExplosions > 0) { // if there are visual explosions
			explosion.explosions = new GameObject[explosion.numberOfExplosions]; // initializes array
			for (int i = 0; i < explosion.numberOfExplosions; i++) {
				explosion.explosions [i] = Instantiate (explosion.explosion, Vector3.zero, Quaternion.identity) as GameObject; // creates explosion
				explosion.explosions [i].SetActive (false); // disables explosion (puts in a pool)
			}
		}
	}

	void breakDown ()
	{
		sr.sprite = null; // disables object visually
	}

	void destroy ()
	{
		if (!destroyed) 
		{

			destroyed = true;

			CancelInvoke ();

			setDefaultMaterial ();

			if (explosion.numberOfExplosions > 0) { // if there are supposed to be visual explosions
				explode (); // explodes
			}

			gameObject.layer = 14; // sets baerrier to trash layer to elliminate futher collisions

			if (sprites.Length > 1) { // if there are destroyed sprite
				sr.sprite = sprites [1]; // sets destroyed sprite
			} else { // if there is no destroyed sprite
				sr.sprite = null; // sets barrier invisible
			}
		}
	}

	void explode ()
	{

		if (destroySounds.Length > 0) // if barrier's destruction makes some sound
		au.PlayOneShot (destroySounds[Random.Range(0, destroySounds.Length)]); // play random destruction sound

		for (int i = 0; i < explosion.numberOfExplosions; i++)
		{
			explosion.explosions [i].transform.position = transform.position + Random.insideUnitSphere * explosion.explosionRadius; // puts explosion in a random place in explosion radius
			explosion.explosions [i].SetActive (true); // enables explosion
		}

		Invoke ("disableExplosions", 0.4f);// disables visual explosions in 0.4 secs

			Collider2D[] hitted = Physics2D.OverlapCircleAll (transform.position, explosion.explosionRadius, explosion.canBeExploded); //stores colliders, hitted by explosion
			for (int i = 0; i < hitted.Length; i++) {
				if (hitted [i].gameObject.layer == 9) { // if it is an enemy
					hitted [i].gameObject.GetComponent<zombieBehavior> ().applyDamage (-100); // tells to apply damage
				} else { // if it is another barrier
					hitted [i].GetComponent<barrierBehaviour> ().applyDamage (-100);// tells to apply damage
				}
			}
	}
		

	void disableExplosions () // disables visual explosions
	{
		for (int i = 0; i < explosion.numberOfExplosions; i++)
		{
			explosion.explosions [i].SetActive (false);
		}
	}

	void setDefaultMaterial () // sets default material
	{
		sr.material = defaultMaterial;
	}

}

[System.Serializable] // explosion class
public class explosible 
{
	[Tooltip("explosion prefab")] public GameObject explosion;
	[HideInInspector] public GameObject[] explosions;
	[Tooltip("number of visual explosions to appear")] public int numberOfExplosions;
	[Tooltip("explosion radius")] public float explosionRadius;
	[Tooltip("Gameobjects with those layers will be exploded")] public LayerMask canBeExploded;
}
