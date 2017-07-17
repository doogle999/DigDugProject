using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundCounter : MonoBehaviour {
	

	public GameObject original;
	public Text textUI;
	public static int round = 1;
	GameObject[] icon;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		icon = GameObject.FindGameObjectsWithTag("RoundCount");
		textUI.text = "Round " + round.ToString();
		for (int x = 0; x < round; x++) {
			if (icon.Length < round) {
				Vector3 position;
				position.x = x;
				position.y = 0;
				position.z = 0;
				Instantiate (original, original.transform.position - position, Quaternion.identity);
			}
		}

	}


}
