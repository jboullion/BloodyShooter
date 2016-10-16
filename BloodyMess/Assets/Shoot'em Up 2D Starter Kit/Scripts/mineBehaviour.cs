using UnityEngine;
using System.Collections;

public class mineBehaviour : MonoBehaviour {

	[Tooltip("damage, which explosion will make")] public float damage;
	[Tooltip("Max distance between mine and enemies")] public float checkRadius;
	[Tooltip("Radius of explosion")] public float explosionRadius;
	[Tooltip("Gameobjects with this layer will be detected/hitted")] public LayerMask enemyLayers;
	[Tooltip("Explosion prefab (visual effect)")] public GameObject explosion;
	[Tooltip("Number of explosions")] public int numberOfExplosions;
	[Tooltip("Explosion sound")] public AudioClip[] explosionSounds;
	GameObject[] explosions;
	AudioSource au;

	// Use this for initialization
	void Start () {
		au = gameObject.GetComponent<AudioSource> ();
		prepareExplosions ();
	}
	
	// Update is called once per frame
	void Update () {
		checkForEnemies (); // detects enemies nearby
	}

	void prepareExplosions () // creates and array of explosions
	{
		explosions = new GameObject[numberOfExplosions];
		for (int i = 0; i < numberOfExplosions; i++) 
		{
			explosions[i] = Instantiate (explosion, Vector2.zero, Quaternion.identity) as GameObject;
			explosions[i].SetActive (false);
		}
	}

	void explode() // explode function
	{

		gameObject.GetComponent<SpriteRenderer> ().enabled = false; // disables mine visually

		Collider2D[] enemies = Physics2D.OverlapCircleAll (transform.position, explosionRadius, enemyLayers); // array of hitted colliders

		for (int i = 0; i < explosions.Length; i++) // creates visual effect of explosion
		{
			GameObject usedExplosion = explosions[i];
			usedExplosion.transform.position = (Vector2) transform.position + Random.insideUnitCircle * explosionRadius;
			usedExplosion.SetActive(true);
		}

		for (int i = 0; i < enemies.Length; i++)
		{
			enemies[i].gameObject.SendMessage ("applyDamage", damage, SendMessageOptions.DontRequireReceiver);
		}

		playOnce (explosionSounds[Random.Range(0, explosionSounds.Length)]);

		Invoke ("disable", 0.5f);
	}

	void checkForEnemies () // explode, if enemy is detected
	{
		Collider2D enemies = Physics2D.OverlapCircle (transform.position, checkRadius, enemyLayers);

		if (enemies)
		{
			explode ();
		}
	}

	void disable () // disable mine and explosions
	{
		for (int i = 0; i < numberOfExplosions; i++)
		{
			explosions[i].SetActive (false);
		}

		gameObject.SetActive (false);
	}

	void playOnce (AudioClip sound) // play track one time
	{
		if (sound)
			au.PlayOneShot (sound);
	}

}
