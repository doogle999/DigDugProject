using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlimpAIManager : MonoBehaviour
{
	public GameObject[] blimps;

	float[] startTime;
	// Use this for initialization
	void Start()
	{
		blimps = GameObject.FindGameObjectsWithTag ("Blimps");
		startTime = new float[blimps.Length];
		float[] timeValue = new float[blimps.Length];
		timeValue [0] = 3;
		timeValue [1] = 5;
		timeValue [2] = 13;
		timeValue [3] = 15;

		for (int x = 0; x < blimps.Length; x++)
		{
			if (x == 4)
			{
				timeValue[4] = 17;
			}
			startTime[x] = timeValue[x];
		}
	}
}
