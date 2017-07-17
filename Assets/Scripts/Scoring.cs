using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoring : MonoBehaviour {
	
	public Text scoreLabel;
	string initText;

	public int score = 0;


	// Use this for initialization
	void Start () {
		initText = scoreLabel.text;
	}
	
	// Update is called once per frame
	void Update () {
		scoreLabel.text = initText + score.ToString ();
	}
}
