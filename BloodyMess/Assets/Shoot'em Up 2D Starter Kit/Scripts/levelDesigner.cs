using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class levelDesigner : MonoBehaviour {

	[Tooltip("block size")] public float step;
	[Tooltip("number of lines in height")] public int numberOfLines;
	[Tooltip("size of map (in blocks)")] public Vector2 mapSize;
	float lineHeight; // height of one line

	[Tooltip("put enemy prefabs here")] public GameObject[] enemyPrefabs;
	[Tooltip("")] public int numberOfEnemies;
	[Tooltip("")] public int numberOfEnemiesInMap;
	List<GameObject> enemies;

	grid [,] g; // map's grid

	[Tooltip("create and customize maps here")] public environment[] environments;
	int steps;

	[Tooltip("stores player's refference")] public GameObject player;
	[Tooltip("coordinates where game begins")] public Vector2 mapProgressPosition;
	Vector2 initialMapProgressPosition;

	List<Vector3> enemyCoordinates;

	public Animator visualOverlay;

	Vector2 overlapPos;
	[HideInInspector]public int mapNum;
	 
	List<GameObject> map; // array, which stores all map objects

	// Use this for initialization
	void Start () {
		initialMapProgressPosition = mapProgressPosition;
		map = new List<GameObject>();
		prepareEnemies ();

		lineHeight = mapSize.y;
		mapSize = new Vector2 (mapSize.x, mapSize.y * numberOfLines);

	}
	
	// Update is called once per frame
	void Update () {
		if (player) { // if there is player
			checkForPlayer (); // check if creating next map step is needed
			checkForEnemiesAround ();  // check is enemies are close and need to be enabled
		}
	}

	public void drawMap () // extands map on one step
	{
		steps++;
		putGround (mapProgressPosition, mapNum); // puts ground
		drawSecondaryVisualEffects (mapProgressPosition, mapNum); // draws visuals
		drawThirdVisualEffects (mapProgressPosition, mapNum); // draws visuals
		putEnemiesCoordinates (mapProgressPosition, numberOfEnemiesInMap); // scatters the enemies on the map 
		putFances (mapProgressPosition, mapNum); // creates undestroyable fances, so player can't go off playable map
		overlapPos = new Vector2 (mapProgressPosition.x, (mapSize.y - 1)/2 * step);

		Vector2 mapCorner = mapProgressPosition + new Vector2 (-(mapSize.x - 1) / 2, (mapSize.y - 1) / 2) * step; // finds top left point of a map
		 
		g = new grid [(int) mapSize.y, (int) mapSize.x]; // creates grid of blocks

		for (int i = 0; i < mapSize.y; i++) // initializes every grid element
		{
			for (int c = 0; c < mapSize.x; c++)
			{
				g [i, c] = new grid (mapCorner + new Vector2 (c, -i) * step, false);
			}
		}
			
		bool putBuilding = steps % environments [mapNum].buildingDensity == 0 ? true : false; // if building should be builded
		if (putBuilding) // yes
		{
			int k = Random.Range (-1,1) >= 0 ? 0:1; // decides where to put building
			int kk = k == 1 ? 0:1;
			build (Vector2.right * lineHeight * k, mapNum); // builds 
			putRoad (Vector2.right * lineHeight * kk);
		} else // no
		{
			putRoad (new Vector2 (0,0));
			putRoad (new Vector2 (lineHeight,0));
		} 
	}

	public void buildStartBase () // creates start place
	{
		putGround (mapProgressPosition, mapNum);
		drawSecondaryVisualEffects (mapProgressPosition, mapNum);
		drawThirdVisualEffects (mapProgressPosition, mapNum);
		putFances (mapProgressPosition, mapNum);

		Vector2 mapCorner = mapProgressPosition + new Vector2 (-(mapSize.x - 1) / 2, (mapSize.y - 1) / 2) * step;
		
		g = new grid [(int) mapSize.y, (int) mapSize.x];
		
		for (int i = 0; i < mapSize.y; i++)
		{
			for (int c = 0; c < mapSize.x; c++)
			{
				g [i, c] = new grid (mapCorner + new Vector2 (c, -i) * step, false);
			}
		}

		bool putBuilding = steps % environments [mapNum].buildingDensity == 0 ? true : false;
		putRoad (new Vector2 (0,0));
		putRoad (new Vector2 (lineHeight,0));

		building b = environments [mapNum].getBuilding ();
		for (int i = 0; i < mapSize.y; i++)
		{
			GameObject newFance = Instantiate (b.side, g[i,0].position - Vector2.right * step/2 - new Vector2 (10,0), Quaternion.Euler (0,0,randomSight())) as GameObject;
			newFance.GetComponent<barrierBehaviour>().health = Mathf.Infinity;
		}

		makeStep ();
		drawMap ();

	}

	void build (Vector2 mapCorner, int num)
	{
		building b = environments [num].getBuilding (); // chooses random building
		for (int i = 1; i < lineHeight; i++)
		{
			for (int c = 1; c < mapSize.x - 1; c++)
			{
				bool putFloor = Random.Range(-1,2) >= 0 ? true:false;
			//	if (putFloor)
				Instantiate (b.floor, g[(int)mapCorner.x + i, (int)mapCorner.y + c].position, Quaternion.identity);
				bool putBarrier = Random.Range(0,10) >= 7 ? true:false;
				if (putBarrier)
				{
					map.Add(Instantiate (getBarrier(), g[(int)mapCorner.x + i, (int)mapCorner.y + c].position, Quaternion.identity) as GameObject);
				}
			}
		}

		for (int c = 1; c < mapSize.x - 1; c++) // builds fances
		{
			map.Add(Instantiate (b.top, g[(int)mapCorner.x + 1, (int)mapCorner.y + c].position + new Vector2 (0,step), Quaternion.identity) as GameObject);
			map.Add(Instantiate (b.down, g[(int)mapCorner.x + (int)lineHeight - 1, (int)mapCorner.y + c].position, Quaternion.identity) as GameObject);
		}

	}

	void putRoad (Vector2 mapCorner) // puts road (optional visual effect)
	{
		float f = (lineHeight - 1) / 2;
		for (int i = 0; i < mapSize.x; i+=1)
		{
			Vector2 pos = g[(int) f + (int) mapCorner.x,i].position;
			map.Add(Instantiate (environments[mapNum].road, pos, Quaternion.identity) as GameObject);
			if (i%environments[mapNum].barriersDensity == 0) 
			{
				map.Add(Instantiate (getBarrier (), pos + Random.insideUnitCircle * step/2, Quaternion.Euler (0,randomSight(),0)) as GameObject);
			}
		}
	}

	GameObject getBarrier () // get random barrier
	{
		return environments[mapNum].barriers [Random.Range(0, environments[mapNum].barriers.Length)];
	}

	int randomSight ()
	{
		int angle = Random.Range (-1, 1) >= 0 ? 180 : 0;
		return angle;
	}

	void clearGrid ()
	{
		foreach (grid gg in g) 
		{
			gg.engaged = false;
		}
	}

	void putFances (Vector2 origin, int num) // puts fances so player can't go off playable map
	{
		building b = environments [num].getBuilding (); // gets random building type
		GameObject wall; // stores wall of this building type
		for (int i = 0; i < mapSize.x; i++) // puts horizontal walls
		{
			wall = b.down;
			Vector2 pos = origin + new Vector2 ((-(mapSize.x - 1)/2) + i,(mapSize.y - 1)/2) * step + new Vector2 (0, step);
			GameObject newFance = Instantiate (wall, pos, Quaternion.identity) as GameObject;
			newFance.GetComponent<barrierBehaviour> ().health = Mathf.Infinity;
			map.Add (newFance);

			for (int u = 0; u < 7; u ++)
			{
				map.Add (Instantiate (b.floor, pos + new Vector2 (0, step * u), Quaternion.identity) as GameObject);
				map.Add(Instantiate (b.floor, pos - new Vector2 (0,mapSize.y + u) * step, Quaternion.identity) as GameObject);
			}

			wall = b.top;
			pos = origin + new Vector2 ((-(mapSize.x - 1)/2) + i,-(mapSize.y - 1)/2) * step;
			newFance = Instantiate (wall, pos, Quaternion.identity) as GameObject;
			newFance.GetComponent<barrierBehaviour> ().health = Mathf.Infinity;
			map.Add (newFance);
		}

		for (int u = 0; u < 7; u ++) // puts vertical walls
		{
			Vector2 pos = origin + new Vector2 ((-(mapSize.x - 1)/2),(mapSize.y - 1)/2) * step + new Vector2 (0, step);
			Instantiate (b.side, pos + new Vector2 (-step/2 + 10, step * u), Quaternion.Euler (0, 180, 0));

			pos = origin + new Vector2 ((-(mapSize.x - 1)/2),-(mapSize.y - 1)/2) * step - new Vector2 (0, step);
			Instantiate (b.side, pos + new Vector2 (-step/2 + 10, -step * u), Quaternion.Euler (0, 180, 0));

			pos = origin + new Vector2 (((mapSize.x - 1)/2),(mapSize.y - 1)/2) * step + new Vector2 (0, step); 
			Instantiate (b.side, pos + new Vector2 (step/2 - 10, step * u), Quaternion.Euler (0, 180, 0));

			pos = origin + new Vector2 (((mapSize.x - 1)/2),-(mapSize.y - 1)/2) * step - new Vector2 (0, step);
			Instantiate (b.side, pos + new Vector2 (step/2 - 10, -step * u), Quaternion.Euler (0, 180, 0)); 
		} 
			
	}

	void putGround (Vector2 origin, int num)
	{
		GameObject ground = Instantiate (environments[num].ground, origin, Quaternion.identity) as GameObject;

		ground.transform.localScale = new Vector3 (mapSize.x + 10, mapSize.y + 10, 1);

		map.Add (ground);
	}

	void drawSecondaryVisualEffects (Vector2 origin, int num)
	{
		for (int i = 0; i < mapSize.x; i++)
		{
			for (int l = 0; l < mapSize.y; l++)
			{
				if (Random.Range (-1, 1) >= 0) {
					Vector2 pos = origin + new Vector2 (-step * ((mapSize.x - 1) / 2), step * ((mapSize.y - 1) / 2)) + new Vector2 (step * l, -step * i) + Random.insideUnitCircle * step;
					GameObject newSec = Instantiate (environments[num].getSecondary (), pos, Quaternion.identity) as GameObject;
					newSec.transform.rotation = Random.Range (-1, 1) >= 0 ? Quaternion.Euler (0, 0, 0) : Quaternion.Euler (0, 180, 0); 
					map.Add (newSec);
				}
			}
		}
	}

	void drawThirdVisualEffects (Vector2 origin, int num)
	{
		for (int i = 0; i < mapSize.x; i++)
		{
			for (int l = 0; l < mapSize.y; l++)
			{
				if (Random.Range (-2, 1) >= 0) {
					Vector2 pos = origin + new Vector2 (-step * ((mapSize.x - 1) / 2), step * ((mapSize.y - 1) / 2)) + new Vector2 (step * l, -step * i) + Random.insideUnitCircle * step;
					GameObject newThr = Instantiate (environments[num].getThird(), pos, Quaternion.identity) as GameObject;
					newThr.transform.rotation = Random.Range (-1, 1) >= 0 ? Quaternion.Euler (0, 0, 0) : Quaternion.Euler (0, 180, 0); 
					map.Add(newThr);
				}
			}
		}
	}

	void putEnemiesCoordinates (Vector2 origin, int number) 
	{
		enemyCoordinates = new List<Vector3>();
		Vector2 pos = origin + new Vector2 (-(mapSize.x - 1) / 2 * step, (mapSize.y - 1) / 2 * step); 
		for (int i = 0; i < numberOfEnemiesInMap; i++)
		{
			enemyCoordinates.Add(pos + new Vector2 (Random.Range (0, step * mapSize.x), Random.Range (-step * mapSize.y, 0))); // calculates random place and adds it to array
		}
	}
		
	void checkForEnemiesAround ()
	{
		for (int i = 0; i < enemyCoordinates.Count; i++)
		{
			if (Vector3.Distance (player.transform.position, enemyCoordinates[i]) < 1200) // if enemy coordinate is close enough to enable enemy
			{
				putEnemy (enemyCoordinates[i]); // enables enemy
				enemyCoordinates.Remove (enemyCoordinates[i]); // removes coordinate, cause it is used
			}
		}
	}

	void putEnemy (Vector3 pos) // enables enemy in a proper place
	{
		enemies [0].transform.position = pos;
		enemies [0].SetActive (true);
		enemies [0].GetComponent<zombieBehavior> ().restart();
		enemies.Add (enemies[0]);
		enemies.RemoveAt (0);
	}

	void prepareEnemies () // creates and array of enemies
	{
		enemies = new List<GameObject> ();
		for (int i = 0; i < numberOfEnemies; i+= enemyPrefabs.Length)
		{
			for (int z = 0; z < enemyPrefabs.Length; z++) 
			{
				GameObject newEnemy = Instantiate (enemyPrefabs[z], Vector2.zero, Quaternion.identity) as GameObject;
				enemies.Add (newEnemy);
				newEnemy.GetComponent<zombieBehavior> ().prepareZombie ();
				newEnemy.SetActive (false); // disables enemy
			}
		}
		enemyCoordinates = new List<Vector3> ();
	}

	void checkForPlayer ()
	{
		if (player.transform.position.x >= overlapPos.x)
		{
			makeStep ();
		}
	}

	public void refreshMap()
	{
		clearMap ();
		buildStartBase ();
	}

	public void clearMap()
	{
		for (int i = 0; i < enemies.Count; i++)
		{
			enemies[i].SetActive (false);
		}
		for (int i = 0; i < map.Count; i++) 
		{
			map[i].SetActive (false);
		}
		mapProgressPosition = initialMapProgressPosition;
	}

	public void makeStep ()
	{
		mapProgressPosition += Vector2.right * step * mapSize.x;
		drawMap ();
	}

	public void fail ()
	{
		Instantiate (enemyPrefabs [Random.Range (0, enemyPrefabs.Length)], player.transform.position, Quaternion.identity); // puts zombie in a character's place (OPTIONAL)
	}

}

[System.Serializable]
public class environment
{
	public string name;
	public GameObject ground;
	public GameObject[] secondary;
	public GameObject[] third;
	public GameObject[] barriers;
	public float barriersDensity;
	public int buildingDensity;
	public building[] buildings;
	public GameObject road;

	public GameObject getSecondary ()
	{
		return secondary[Random.Range (0, secondary.Length)];
	}
	public GameObject getThird ()
	{
		return third[Random.Range (0, third.Length)];
	}
	public building getBuilding ()
	{
		return buildings [Random.Range (0, buildings.Length)];
	}
}

[System.Serializable]
public class grid
{
	public grid (Vector2 pos, bool eng)
	{
		position = pos;
		engaged = eng;
	}
	public grid (Vector2 pos, bool eng, GameObject o)
	{
		position = pos;
		engaged = eng;
		this.o = o;
	}

	public GameObject o;
	public Vector2 position;
	public bool engaged;
}

[System.Serializable]
public class building
{
	public string name;
	public GameObject top;
	public GameObject side;
	public GameObject down;
	public GameObject topDoor;
	public GameObject downDoor;
	public GameObject floor;

}