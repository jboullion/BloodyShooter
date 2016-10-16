using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class spitfireBehaviour : MonoBehaviour, IWeapon {

	[Tooltip("damage weapon makes")] public float damage;
	[Tooltip("fire prefab (for visual effect)")] public GameObject fire;
	[Tooltip("number of fire objects")] public int fireNum;
	[Tooltip("speed of a fire")] public float fireSpeed;
	[Tooltip("min visual distance between character and fire")] public float fireDistance;
	[Tooltip("circle's radius, where fire strikes")] public float radius;
	[Tooltip("Gameobjects with this layer will be hitted")] public LayerMask enemyLayer;
	[Tooltip("sound of a flame")] public AudioClip flameSound;
	List<GameObject> ammo;
	AudioSource au;

	// Use this for initialization
	void Start () {
		prepareAmmo ();

		au = gameObject.GetComponent<AudioSource> ();
	}

	public void attack(Vector2 direction)
	{
		Collider2D[] enemies = Physics2D.OverlapCircleAll ((Vector2) transform.position + direction * radius, radius, enemyLayer); // stores hiited colliders
		
		for (int i = 0; i < enemies.Length; i++)
		{
			enemies[i].gameObject.SendMessage ("applyDamage", damage, SendMessageOptions.DontRequireReceiver); // tells them to apply damage
		}
		for (int i = 1; i < 5; i++)
		{
			throwFire (Quaternion.Euler(0,0, Random.Range (-45,45)) * direction); //creates visual fire effect 
		}

		if (!au.isPlaying && flameSound) { 
			au.clip = flameSound; // sets fire sound
			au.Play (); // plays fire sound
		}
	}

	public void triggerUp () // trigger pulled up
	{
		for (int i = 0; i < ammo.Count; i++) // disables fire
		{
			ammo[i].SetActive (false);
		}
		au.Stop ();
	}

	void throwFire (Vector2 direction)
	{
		GameObject usedFire = ammo [0];
		usedFire.transform.position = transform.position + (Vector3)direction * fireDistance;
		usedFire.SetActive (true);
		usedFire.GetComponent<moveBehaviour> ().applyDirection (direction);
		ammo.Remove (usedFire);
		ammo.Add (usedFire);
	}

	public void prepareAmmo ()
	{
		ammo = new List<GameObject> (); 
		for (int i = 0; i < fireNum; i++)
		{
			GameObject newFire = Instantiate (fire, Vector2.zero, Quaternion.identity) as GameObject;
			ammo.Add (newFire);
			moveBehaviour m = newFire.AddComponent<moveBehaviour>();
			m.speed = fireSpeed;
			newFire.SetActive (false);
		}
	}

}
