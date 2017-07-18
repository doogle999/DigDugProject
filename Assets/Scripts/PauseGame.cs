using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour {
	//public GameObject button1;
	public Button button;
	public Text buttonText;
	public AudioSource music;

	int count = 0;

	// Use this for initialization
	void Start () {
		button.onClick.AddListener(() => taskOnClick());
		//button.onClick.AddListener(taskOnClick);
		//status.enabled = false;
		print (button);
	}

	void taskOnClick(){
		print("a");
		if (count % 2 == 0) {
			buttonText.text = "Continue";
			Time.timeScale = 0;
			music.Pause ();
			//status.enabled = true;
		} else {
			buttonText.text = "Pause";
			Time.timeScale = 1;
			music.UnPause ();
			//status.enabled = false;
		}
		count++;
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
