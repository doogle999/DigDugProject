using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeGameArea : MonoBehaviour
{
	void Start()
	{
		resize();
	}
	
	void Update()
	{
		resize();
	}

	void resize()
	{
		Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		Vector2 cameraDimensions = new Vector2((2.0F * camera.orthographicSize * camera.aspect) * (1.0F - UIArea.widthPercentage), 2.0F * camera.orthographicSize);
		Vector2 gridDimensions = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>().getDimensions();
		Vector2 ratio = new Vector2(gridDimensions.x / cameraDimensions.x, gridDimensions.y / cameraDimensions.y);
		bool useX = ratio.x > ratio.y;

		transform.localScale = new Vector3(1, 1, 1);
		if(useX)
		{
			transform.localScale *= 1.0F / ratio.x;
		}
		else
		{
			transform.localScale *= 1.0F / ratio.y;
		}

		Vector2 position = Vector2.zero;
		if(useX)
		{
			position.x = -camera.orthographicSize * camera.aspect;
			position.y = -camera.orthographicSize;
		}
		else
		{
			position.y = -camera.orthographicSize;
			position.x = (cameraDimensions.x * (1.0F - (ratio.x / ratio.y)) / 2.0F) - (cameraDimensions.x / 2.0F) - (camera.orthographicSize * camera.aspect * UIArea.widthPercentage);
		}
		transform.position = position;
	}
}
