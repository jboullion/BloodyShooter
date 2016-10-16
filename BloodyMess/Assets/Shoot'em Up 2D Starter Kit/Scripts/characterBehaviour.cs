using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class characterBehaviour : MonoBehaviour {

	[Tooltip("character's health")] public float health;
	float initialHealth; // stores default health
	[Tooltip("getting damage from zombie")] public float damageFromZombie;
	[Tooltip("movement speed")] public float speed;
	[Tooltip("time after last attack, after which character will turn back to movement direction")] public float shootRunDelay;
	Rigidbody2D rb; // stores Rigidbody2D 
	public Animator animator; // stores animator
	SpriteRenderer sr; // stores sprite renderer

	gamePlayManager gpm; // stores game play manager script refference

	Transform weaponContainer; // stores weapon container
	IWeapon w; // stores weapon script
	SpriteRenderer wsr; // stores weapon's sprite renderer
	int k; // line of sight factor
	bool shooting; // if shooting
	[HideInInspector] public bool isPlayer; // if this character is controlled by player

	[HideInInspector] public bool dead; // if character is dead

	[Tooltip("Gameobjects with this layers will be considered enemies")] public LayerMask enemy;
	[Tooltip("Gameobjects with this layers will be considered friends")] public LayerMask ally;
	[Tooltip("Gameobjects with this layers will be considered doors")] public LayerMask door;
	[Tooltip("max distance between character and enemy, where they see each other")] public float enemyRadius;

	RectTransform healthLine; // stores UI element, which represents health line
	float fullHealthLineLength; 
	float healthPointLength;

	Vector2 size; // box collider size
	Vector2 offset; // box collider offset

	Vector2 facing = Vector2.left;

	public bool doDebug = false;
	public int DebugSpeed = 30;
	private int DEBUG_TIMER = 0;

	// Use this for initialization
	void Start () {
		prepareFighter (); // determines variables
		setLayerOrder (); // sets proper sorting order test

		//healthLine = GameObject.Find ("Canvas").transform.FindChild ("gameUI").FindChild ("healthBar").FindChild ("line").gameObject.GetComponent<RectTransform> ();  // determine health bar
		//fullHealthLineLength = healthLine.sizeDelta.x; // full length
		//healthPointLength = fullHealthLineLength / health; // one health point length 

		size = gameObject.GetComponent<BoxCollider2D> ().size; // determines size
		offset = gameObject.GetComponent<BoxCollider2D> ().offset; // determines offset

	}

	// Update is called once per frame
	void FixedUpdate () {
		if (!isPlayer) // if not a player character
		{
			checkForEnemies (); // checks for enemies
		}
		attractEnemies (); // attract enemies to follow character
		setLayerOrder (); // refreshes sorting layer
	} 

	void prepareFighter() // determines variables
	{
		rb = gameObject.GetComponent<Rigidbody2D> ();
		animator = gameObject.GetComponent<Animator> ();
		sr = gameObject.GetComponent<SpriteRenderer> ();
		k = 1;
		initialHealth = health;
		findWeapon ();
		gpm = gameObject.findGPM ();
	}

	public void move (Vector2 direction) // movement function
	{
		
		rb.velocity = direction * speed; // applies velocity

		if (!shooting) // if character is not shooting
		{
			if (rb.velocity.x > 0) //if goes right 
			{
				rotateRight (); // rotates right
			} else 
			{
				rotateLeft ();
			}
		}

		//if the user is moving at all, update their current facing vector
		if(direction.x != 0 || direction.y != 0)
		{
			if (direction.x != facing.x)
				facing.x = direction.x;

			if (direction.y != facing.y)
				facing.y = direction.y;
		}
		
		/*
		if (doDebug && DEBUG_TIMER >= DebugSpeed)
		{
			Debug.Log("x: " + facing.x + ", y: " + facing.y);
			DEBUG_TIMER = 0;
		}
		DEBUG_TIMER++;
		*/

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

	void rotate ()
	{
		if (k == 1) 
		{
			rotateLeft ();
		} else 
		{
			rotateRight ();
		}
	}

	public void stopX ()
	{
		rb.velocity = new Vector2 (0, rb.velocity.y);
		checkForStanding ();
	}

	public void stopY ()
	{
		rb.velocity = new Vector2 (rb.velocity.x, 0);
		checkForStanding ();
	}

	void checkForStanding ()
	{
		if (rb.velocity == Vector2.zero) // if character stays
		{
			animator.SetInteger ("state", 0); // sets idle animation
		}
	}

	public void attack (Vector2 direction) // attack function
	{
		if (!shooting) // if character is not shooting
		{
			if (direction.x >= 0 && k == -1) // if attack direction is opposite to a line of sight
			{
				rotateRight(); // turns character right
			}
			else if (direction.x < 0 && k == 1)
			{
				rotateLeft();
			}

			int lx = direction.x.CompareTo(0);
			if (lx == 0)
			{
				lx = 1;
			}
			int ly = direction.y.CompareTo(0);
			if (ly == 0)
			{
				ly = 1;
			}

			float angle = ly * Vector2.Angle(lx * Vector2.right, lx * direction);
			weaponContainer.localScale = new Vector3(1, lx, 1);
			weaponContainer.rotation = Quaternion.Euler(0, 0, angle);

			w.attack(direction); // tells weapon script to attack
			shooting = true;
			CancelInvoke("notShooting");
			Invoke("notShooting", shootRunDelay); // rotate back, if character stops attacking
		}
		
	}

	public void attack() // attack function
	{

		if (!shooting) // if character is not shooting
		{

			if (facing.x >= 0 && k == -1) // if attack direction is opposite to a line of sight
			{
				rotateRight(); // turns character right
			}
			else if (facing.x < 0 && k == 1)
			{
				rotateLeft();
			}

			int lx = facing.x.CompareTo(0);
			if (lx == 0)
			{
				lx = 1;
			}
			int ly = facing.y.CompareTo(0);
			if (ly == 0)
			{
				ly = 1;
			}

			float angle = ly * Vector2.Angle(lx * Vector2.right, lx * facing);
			weaponContainer.localScale = new Vector3(1, lx, 1);
			weaponContainer.rotation = Quaternion.Euler(0, 0, angle);

			w.attack(facing); // tells weapon script to attack
			shooting = true;
			CancelInvoke("notShooting");
			Invoke("notShooting", shootRunDelay); // rotate back, if character stops attacking
		}
	}

	public void stop ()
	{
		animator.SetInteger ("state", 0); // sets idle animation
		rb.velocity = Vector2.zero; // velocity to zero
	}

	void findWeapon () // determines weapon variables
	{
		weaponContainer = transform.FindChild ("weapon"); // finds weapon container
		for (int i = 0; i < weaponContainer.childCount; i++) // checks all childs for being weapon
		{
			GameObject c = weaponContainer.GetChild (i).gameObject;
			if (c.gameObject.GetComponent<IWeapon> () != null) // if child contains weapon script
			{
				w = c.gameObject.GetComponent<IWeapon> (); // determines weapon script
				wsr = c.gameObject.GetComponent<SpriteRenderer> (); // determines weapon's sprite renderer script
				break; // stops checking child for being weapon
			} 

		}
	}

	public void triggerUp () // weapon's trigger up
	{
		w.triggerUp ();
	}

	public void notShooting ()
	{
		shooting = false;
	}

	public void applyHealth (float newHealth) // applies health;
	{
		if (health + newHealth < initialHealth) // conditions makes health not jump over initial health
		{
			health += newHealth;
		} else 
		{
			health = initialHealth;
		}
		if (health <= 0 && !dead) // if health is above zero
		{
			die ();
		}

		healthLine.sizeDelta = healthLine.sizeDelta + Vector2.right * newHealth * healthPointLength; // refreshes health bar

	}

	void die () // character die function
	{
		dead = true;
		GameObject.Find ("levelDesigner").GetComponent<levelDesigner> ().fail (); // tells level designer that player failed
		GameObject.Find ("gamePlayManager").GetComponent<gamePlayManager> ().fail (); // tells game play manager that player failed
		gameObject.SetActive (false); // disables player
	}

	public void restart () // restarts game
	{
		Time.timeScale = 1f;
		healthLine.sizeDelta = new Vector2 (0, healthLine.sizeDelta.y) +  Vector2.right * initialHealth * healthPointLength; // brings health line back to initial size
		health = initialHealth; // sets health to inisial number
		dead = false;
		gameObject.SetActive (true); // enables character
	}

	void checkForEnemies () // looks for enemies around to shoot
	{
		Collider2D en = Physics2D.OverlapCircle ((Vector2) transform.position + offset, enemyRadius, enemy); // enemy
		if (en) // if enemy exists
		{
			attack ((en.transform.position - transform.position).normalized); // attacks enemy
		}
	}

	void attractEnemies() // attracks enemies around && checks for close enemies
	{
		Collider2D[] en = Physics2D.OverlapCircleAll ((Vector2) transform.position + offset, enemyRadius, enemy); // enemies around
		if (en.Length > 0) // if there are enemies around
		{
			for (int i = 0; i < en.Length; i++) // tells every one of them to follow player
			{
				en [i].gameObject.GetComponent<zombieBehavior> ().move (transform.position);
			}
		}

		Collider2D[] closeEn = Physics2D.OverlapCircleAll ((Vector2) transform.position + offset, size.x + 1, enemy); // enemies close
		for (int i = 0; i < closeEn.Length; i++)
		{
			applyHealth (damageFromZombie); // applies damage from close enemies
		}

	}

	void setLayerOrder () // refreshes sorting layer according to Y position
	{
		float p = transform.position.y * 10;
		sr.sortingOrder = -Mathf.FloorToInt (p);
		wsr.sortingOrder = -Mathf.FloorToInt (p - 1);
	}

}
	