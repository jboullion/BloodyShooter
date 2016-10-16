using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class cameraBehavior : MonoBehaviour {

	[Tooltip("preferred camera's height")] public float prefferedSize;
	[Tooltip("Pixel Per Units")] public float PPU;

	// Use this for initialization
	void Start () {
		calculateCameraSize ();
	}

	void calculateCameraSize () // calculates closest to preffered size pixel perfect height of a camera  
	{

		float minDelta = 0; 
		int multiply = 0;

		int m = 20;
		float[] num = new float[m];
		float a = Mathf.Min (Screen.height, Screen.width);
		float size = 0;
		num [0] = (a/ (1 * PPU)) * 0.5f;
		int s = 0;
		for (int i = 1; i < m; i++)
		{
			num[i] = (a/ (i * PPU)) * 0.5f;
			if (Mathf.Abs (prefferedSize - num [i]) <= Mathf.Abs (prefferedSize - num [i - 1])) 
			{
				size = num [i];
			}
		}
			
		Camera.main.orthographicSize = size;
	}



}
