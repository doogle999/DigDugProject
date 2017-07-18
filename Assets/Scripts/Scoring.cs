using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoring : MonoBehaviour
{
	public readonly float tileScore = 10.0F;
	public readonly float enemyMaxScore = 500.0F;
	public readonly float enemyMinScore = 100.0F;

	public Text scoreLabel;
	string initText;

	public int score = 0;

	public FloatUnityEvent increaseScore = new FloatUnityEvent();

	// Use this for initialization
	void Start()
	{
		increaseScore.AddListener(onIncreaseScore);
		initText = scoreLabel.text;
	}
	
	// Update is called once per frame
	void Update () {
		scoreLabel.text = initText + score.ToString ();
	}

	private void onIncreaseScore(float increase)
	{
		score += (int)increase;
	}
}
