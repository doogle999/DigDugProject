using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour {
	public Button button;
	public Text buttonText;
	public Text status;

	int count = 0;

	// Use this for initialization
	void Start () {
		//button.onClick.AddListener(onClick);
		status.enabled = false;
	}

	public void onClick(){
		print("a");
		if (count % 2 == 0) {
			buttonText.text = "Continue";
			status.enabled = true;
		} else {
			buttonText.text = "Pause";
			status.enabled = false;
		}
		count++;
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
