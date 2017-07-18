using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Life : MonoBehaviour {

	private Text livesText;

	public static int lives = 3;

	// Use this for initialization
	void Start()
	{
		livesText = GetComponent<Text>();
	}

	// Update is called once per frame
	void Update()
	{
		string result = "LIVES:\n" + lives.ToString();
		livesText.text = result;
	}
}
