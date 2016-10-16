using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class rangeWeaponBehaviour : MonoBehaviour, IWeapon {


	[Tooltip("bullet prefab")] public GameObject bullet;
	[Tooltip("if shoot all barrels at once")] public bool once;
	[Tooltip("set and customize gun's barrels")] public List<gun> guns;
	[Tooltip("size of a bullet array")] public int numberOfBullets;
	[Tooltip("distance from character to bullet, when it appers")] public float shootDistance;
	[Tooltip("Min time interval between shots")] public float shootInterval;
	[Tooltip("Put shotfire object here (optional visual effect)")] public SpriteRenderer shotFire;
	[Tooltip("put shot sounds here")] public AudioClip[] shotSounds;
	AudioSource au;
	bool attacked;

	Vector2 size;

	Animator an;
	Animator anim;
	List<GameObject> ammo;
	float angle;

	// Use this for initialization
	void Start () {

		prepareAmmo ();
		BoxCollider2D bc = transform.parent.parent.gameObject.GetComponent<BoxCollider2D> ();
		size = bc.size - (bc.offset * 2);

		au = gameObject.GetComponent<AudioSource> ();

	}

	void prepareAmmo()
	{
		ammo = new List<GameObject> ();
		for (int i = 0; i < numberOfBullets; i++) 
		{
			ammo.Add (Instantiate (bullet, Vector2.zero, Quaternion.identity) as GameObject);
			ammo [i].SetActive (false);
		}

		if (shotFire)
		disableShotFire ();

		anim = gameObject.GetComponent<Animator> ();
		an = transform.parent.gameObject.GetComponent<Animator> ();
	}

	public void attack (Vector2 direction)
	{
		if (!attacked) // if did not shooted yer
		{
			attacked = true;
			if (once) // if all barrels at once
			{
				for (int i = 0; i < guns.Count; i++)
				{
					throwBullet (direction);
				}
			} else 
			{
				throwBullet (direction);
			}
			
			if (anim) // set shooting animation
			{
				anim.SetInteger ("state", 1);
			}
			
			if (shotFire) // disables shotfire effect in 0.1 sec
				Invoke ("disableShotFire", 0.1f);

			playOnce(shotSounds[Random.Range(0,shotSounds.Length)]);

			Invoke ("triggerUp", shootInterval); 
		}
	}

	void throwBullet (Vector2 direction) 
	{
		GameObject shootedBullet = ammo [0];
		gun g = guns[0];
		float s = g.scale;
			
		Vector3 shootPlace = transform.position + transform.rotation * new Vector3 (g.deltaPos.x * direction.x.CompareTo(0), g.deltaPos.y, 0) + transform.right * shootDistance;
		Vector2 shootDirection = Quaternion.Euler (0, 0, s) * transform.right; 

		shootedBullet.SetActive (true);
		shootedBullet.GetComponent<bulletBehaviour>().applyDirection(shootDirection, angle, shootPlace);

		if (shotFire) 
		{
			shotFire.enabled = true;
		} 

		ammo.Remove (shootedBullet);
		ammo.Add (shootedBullet);
		guns.Remove (g);
		guns.Add (g); 

	}

	void disableShotFire ()
	{
		shotFire.enabled = false;
	}

	void resetAn ()
	{
		an.SetInteger ("state", 0);
	}

	public void triggerUp () // ready to shoot again
	{
		CancelInvoke ("triggerUp");
		attacked = false;
		if (anim)
		anim.SetInteger ("state", 0);
	}

	void playOnce (AudioClip sound)
	{
		if (sound)
		au.PlayOneShot (sound);
	}

}
	
[System.Serializable]
public class gun 
{
	[Tooltip("where barrel is located relatively to weapon")] public Vector3 deltaPos;
	[Tooltip("shoot angle")] public float scale;
}