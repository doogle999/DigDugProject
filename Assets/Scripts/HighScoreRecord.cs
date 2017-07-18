using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreRecord : MonoBehaviour {

	public Text highScoreLabel;

	public static int highScore = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		highScoreLabel.text = "HIGH SCORE:\n" + highScore.ToString ();
	}
}
