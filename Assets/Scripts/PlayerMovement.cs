using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
	private readonly KeyCode[] KEY_MAP = new KeyCode[4] { KeyCode.UpArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.LeftArrow }; // Mapped to directions
	private readonly KeyCode SHOOT_KEY = KeyCode.Space;
	
	private float speedDig = 2.0F; // Speed per block per second when digging (1.0 would be one block per second)
	private float speedReg = 3.0F; // Speed when not digging

	public Vector2UnityEvent movedToTile = new Vector2UnityEvent();
	public Vector2UnityEvent movedThroughVWall = new Vector2UnityEvent();
	public Vector2UnityEvent movedThroughHWall = new Vector2UnityEvent();

	private bool shooting = false;
	private bool shotHit = false;
	private float shootExtendDistance = 2.0F;
	private float shootThickness = 0.1F;
	private float shootExtendTime = 0.5F;

	private float rotationMax = 15.0F;
	private float speedRotate = 5.0F;

	private GameObject harpoon;

	void Start()
	{
		getGrid();

		harpoon = GameObject.FindGameObjectWithTag("Harpoon");

		movedToTile.AddListener(grid.destroyTile);
		movedThroughVWall.AddListener(grid.destroyVWall);
		movedThroughHWall.AddListener(grid.destroyHWall);
	}

	void Update()
	{
		if(Input.GetKeyDown(SHOOT_KEY))
		{
			if(!shooting)
			{
				shooting = true;
				StartCoroutine("fireHarpoon");
			}
		}
			
		if(onRoute)
		{
			Direction.Dir possibleDirection = direction;
			bool anyKeyPressed = false;
			for(int i = 0; i < 4; i++) // Determine the direction that the player is pressing in
			{
				if(Input.GetKey(KEY_MAP[((int)direction + i) % 4])) // Check the four directions
				{
					possibleDirection = (Direction.Dir)(((int)direction + i) % 4);
					anyKeyPressed = true;
				}
			}

			rotateDirection();

			if(anyKeyPressed)
			{
				if(possibleDirection != (Direction.Dir)(((int)direction + 2) % 4)) // Non-opposite direction, move
				{
					Vector2 oldPosition = positionScaled;
					if(grid.tileInBounds(grid.getTileLocation(getNextPosition())))
					{
						if(moveInDirection(Direction.passingTileOrWall(positionScaled, direction, grid) ? speedDig : speedReg))
						{
							destroyTilesAndWalls(oldPosition, direction);
							direction = possibleDirection;
						}
					}
					else
					{
						onRoute = false;
					}
				}
				else // Opposite direction, turn around and don't move
				{
					positionScaled += Direction.convertDirToUnitVector2(direction);
					direction = possibleDirection;
					distance = 1.0F - distance;
				}
			}
		}
		else
		{
			for(int i = 0; i < 4; i++) // Determine the direction that the player is pressing in
			{
				if(Input.GetKey(KEY_MAP[((int)direction + i) % 4])) // Check the four directions
				{
					direction = (Direction.Dir)(((int)direction + i) % 4);
					onRoute = true;
				}
			}
		}
	}

	private void destroyTilesAndWalls(Vector2 positionScaled, Direction.Dir direction)
	{
		Vector2 usePositionScaled = positionScaled + Direction.convertDirToUnitVector2(direction);
		movedToTile.Invoke(grid.getTileLocation(usePositionScaled));
		if(((int)direction / 2) == 0)
		{
			usePositionScaled = positionScaled;
		}
		if(((int)direction % 2) == 0)
		{
			if(grid.hWallInBounds(usePositionScaled))
			{
				if(grid.getHWallState(usePositionScaled))
				{
					movedThroughHWall.Invoke(usePositionScaled);
				}
			}
		}
		else
		{
			if(grid.vWallInBounds(usePositionScaled))
			{
				if(grid.getVWallState(usePositionScaled))
				{
					movedThroughVWall.Invoke(usePositionScaled);
				}
			}
		}
	}

	private IEnumerator fireHarpoon()
	{
		for(float i = 0; i < shootExtendTime; i += (float)0.03)
		{
			harpoon.transform.localScale = Direction.convertDirToUnitVector2((Direction.Dir)((int)direction % 2)) * (shootExtendDistance * i / shootExtendTime);
			harpoon.transform.localScale += (Vector3)(Direction.convertDirToUnitVector2((Direction.Dir)(((int)direction + 1) % 2)) * shootThickness);
			harpoon.transform.localPosition = transform.localPosition + (Vector3)(Direction.convertDirToUnitVector2(direction) * (shootExtendDistance / 2.0F * i / shootExtendTime));

			Vector2 location = grid.getTileLocation((Vector2)harpoon.transform.localPosition + Direction.convertDirToUnitVector2(direction) * (shootExtendDistance / 2.0F * i / shootExtendTime));

			if(!grid.tileInBounds(location) || grid.getTileState(location))
			{
				break;
			}
			yield return new WaitForSeconds(0.03f);
		}
		harpoon.transform.localScale = Vector3.zero;
		shooting = false;
	}

	private void rotateDirection()
	{
		Vector3 targetEuler = Vector3.zero;
		switch(direction)
		{
			case Direction.Dir.L:
				targetEuler.Set(0, 0, rotationMax);
				break;
			case Direction.Dir.R:
				targetEuler.Set(0, 0, -rotationMax);
				break;
			case Direction.Dir.U:
				targetEuler.Set(0, 0, 0);
				break;
			case Direction.Dir.D:
				targetEuler.Set(0, 0, 0);
				break;
		}

		//Vector3 difference = ((targetEuler - transform.localEulerAngles).sqrMagnitude < (transform.localEulerAngles - targetEuler).sqrMagnitude ? targetEuler - transform.localEulerAngles : transform.localEulerAngles - targetEuler);

		Vector3 difference = targetEuler - transform.localEulerAngles;
		if(Mathf.Abs(difference.z) > 180)
		{
			difference.z = 360 - difference.z;
			transform.Rotate(-difference * Time.deltaTime * speedRotate);
		}
		else
		{
			transform.Rotate(difference * Time.deltaTime * speedRotate);
		}
	}
}