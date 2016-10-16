using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class minesBehavior : MonoBehaviour, IWeapon { // implements weapon interface

	[Tooltip("put mine prefab here")] public GameObject mine;
	[Tooltip("how big should mine array be")] public int numberOfMines;
	[Tooltip("how far from character will mines appear")] public float mineDistance;
	[Tooltip("min time delay between putting mines")] public float putTimeInterval;
	[Tooltip("Sorting layer offset")] public float sortingLayerOffset;
	List<GameObject> mines; // mines array
	bool attacked;

	// Use this for initialization
	void Start () {
		prepareMines ();
	}

	public void attack (Vector2 direction)
	{
		if (!attacked) 
		{
			GameObject usedMine = mines [0];
			Transform p = transform.parent.parent;
			usedMine.transform.position = (Vector2)p.position + p.gameObject.GetComponent<BoxCollider2D>().offset + direction * mineDistance;
			usedMine.SetActive (true);
			SpriteRenderer sr = usedMine.GetComponent<SpriteRenderer>();
			sr.enabled = true;
			sr.sortingOrder = -(int) (usedMine.transform.position.y + sortingLayerOffset) * 10;
			attacked = true;
			Invoke ("triggerUp", putTimeInterval);

			mines.Remove (usedMine);
			mines.Add (usedMine);
		}
	}

	void prepareMines ()
	{
		mines = new List<GameObject>();
		for (int i = 0; i < numberOfMines; i++)
		{
			GameObject newMine = Instantiate (mine, Vector2.zero, Quaternion.identity) as GameObject;
			mines.Add(newMine);
			newMine.SetActive(false);
		}
	}

	public void triggerUp()
	{
		attacked = false;
	}

}
