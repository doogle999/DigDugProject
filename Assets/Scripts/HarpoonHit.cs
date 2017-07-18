using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonHit : MonoBehaviour
{
	public GameObjectUnityEvent harpoonHit = new GameObjectUnityEvent();

	void Start()
	{
		harpoonHit.AddListener(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().harpoonTrigger);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "Blimps")
		{
			harpoonHit.Invoke(other.gameObject);
		}
	}
}
