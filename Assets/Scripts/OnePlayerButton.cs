using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnePlayerButton : MonoBehaviour {

	public Button onePlayerButton;

	// Use this for initialization
	void Start () {
		onePlayerButton.onClick.AddListener (() => {
			onClick ();
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void onClick(){
		
		SceneManager.LoadScene ("Level 1");
	}
}
