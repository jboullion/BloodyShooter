using UnityEngine;
using System.Collections;

public class meleeWeaponBehaviour : MonoBehaviour, IWeapon { // implements weapon interface

	[Tooltip("damage, which weapon makes")] public float damage;
	[Tooltip("radius of a circle, where weapon strikes")] public float radius;
	[Tooltip("Gameobjects of this layer will be hitted")] public LayerMask target;
	[Tooltip("Interval between attacks")] public float shootInterval;
	[Tooltip("Attack sound")] public AudioClip strikeSound;
	bool attacked;
	Animator an;
	AudioSource au;

	Vector2 offset;

	void Start()
	{
		an = gameObject.GetComponent<Animator> ();
		au = gameObject.GetComponent<AudioSource> ();
		offset = transform.parent.parent.gameObject.GetComponent<BoxCollider2D> ().offset;
	}

	public void attack (Vector2 direction)
	{
		if (!attacked) // if did not attacked yet
		{
			if (!au.isPlaying && strikeSound)
			{
				au.clip = strikeSound;
				au.Play ();
			}
			Collider2D[] victim = Physics2D.OverlapCircleAll ((Vector2)transform.position + offset + direction * radius, radius, target); // stores hitted colliders
			if (victim.Length > 0) { // if there are hitted coliiders
				for (int i = 0; i < victim.Length; i++) {
					victim [i].SendMessage ("applyDamage", damage, SendMessageOptions.DontRequireReceiver); // tells them to apply damage
				}
				if (shootInterval > 0)
				{
				attacked = true;
				Invoke ("triggerUp", shootInterval); 
				}
			}
			an.SetInteger("state", 1);
		}
	}

	public void triggerUp() // weapon is ready to attack again
	{
		CancelInvoke ("triggerUp");
		attacked = false;
		an.SetInteger("state", 0);
		au.Stop ();
	}

}
