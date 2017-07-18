using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startMusicPauseAllElse : MonoBehaviour
{
	void Update()
	{
		if(!GetComponent<AudioSource>().isPlaying)
		{
			Time.timeScale = 1.0F;
			GameObject.FindGameObjectWithTag("PauseButton").GetComponent<PauseGame>().enabled = true;
			gameObject.SetActive(false);

			PlayerMovement pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
			pm.positionScaled = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>().defaultPlayerPosition;
			pm.setTransform(pm.positionScaled, 0, 0);


			GameObject[] blimps = GameObject.FindGameObjectsWithTag("Blimps");
			for(int i = 0; i < blimps.Length; i++)
			{
				blimps[i].GetComponent<BlimpAIMovement>().player = pm;
			}
		}
	}

	void OnEnable()
	{
		Time.timeScale = 0.0F;
		GameObject.FindGameObjectWithTag("PauseButton").GetComponent<PauseGame>().enabled = false;
		GetComponent<AudioSource>().Play();
	}
}
