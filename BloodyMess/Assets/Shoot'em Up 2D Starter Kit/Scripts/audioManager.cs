using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	private bool bulletHitting = false;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void bulletHit(AudioClip sound)
	{
		if (bulletHitting)
			return;

		if (sound)
			GetComponent<AudioSource>().PlayOneShot(sound);

		Invoke("resetBulletSound", sound.length);

	}

	void resetBulletSound()
	{
		bulletHitting = false;
	}

	public void playOnce(AudioClip sound)
	{
		if (sound)
			GetComponent<AudioSource>().PlayOneShot(sound);

	}
}
