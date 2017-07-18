using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : Movement
{
	private readonly KeyCode[] KEY_MAP = new KeyCode[4] { KeyCode.UpArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.LeftArrow }; // Mapped to directions
	private readonly KeyCode SHOOT_KEY = KeyCode.Space;
	
	private float speedDig = 2.0F; // Speed per block per second when digging (1.0 would be one block per second)
	private float speedReg = 3.0F; // Speed when not digging

	public Vector2UnityEvent movedToTile = new Vector2UnityEvent();
	public Vector2UnityEvent movedThroughVWall = new Vector2UnityEvent();
	public Vector2UnityEvent movedThroughHWall = new Vector2UnityEvent();

	public UnityEvent playerDied = new UnityEvent();

	private bool shooting = false;
	private bool shotHit = false;
	private GameObject enemyHit;

	private float shootExtendDistance = 2.0F;
	private float shootThickness = 0.1F;
	private float shootExtendTime = 0.5F;
	private float pumpCooldownTime = 0.15F;
	private bool canPump = true;

	private float rotationMax = 15.0F;
	//private float speedRotate = 0.01F;

	private GameObject harpoon;

	private AudioSource audioSource;
	private PlayerSounds playerSounds;

	void Start()
	{
		getGrid();

		harpoon = GameObject.FindGameObjectWithTag("Harpoon");
		harpoon.transform.localScale = Vector3.zero;
		harpoon.GetComponent<HarpoonHit>().harpoonHit.AddListener(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().harpoonTrigger);
		audioSource = GetComponent<AudioSource>();
		playerSounds = GetComponent<PlayerSounds>();

		movedToTile.AddListener(grid.destroyTile);
		movedThroughVWall.AddListener(grid.destroyVWall);
		movedThroughHWall.AddListener(grid.destroyHWall);

		playerDied.AddListener(grid.onPlayerDie);
	}

	void Update()
	{
		if(Time.timeScale != 0.0F)
		{
			if(Input.GetKeyDown(SHOOT_KEY))
			{
				if(!shooting && !shotHit)
				{
					audioSource.clip = playerSounds.shootingSound;
					audioSource.Play();
					shooting = true;
					StartCoroutine("fireHarpoon");
				}
				if(shotHit & canPump)
				{
					audioSource.clip = playerSounds.pumpingSound;
					audioSource.Play();
					enemyHit.GetComponent<BlimpAIMovement>().pumpState++;
					if(enemyHit.GetComponent<BlimpAIMovement>().pumpState >= 3)
					{
						shotHit = false;
						enemyHit = null;
						harpoon.transform.localScale = Vector3.zero;
					}
					StartCoroutine("pumpCooldownTimer");
				}
			}

			if(!shooting && !shotHit)
			{
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
						if(!audioSource.isPlaying)
						{
							audioSource.clip = playerSounds.walkingSound;
							audioSource.Play();
						}
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
					else
					{
						if(audioSource.isPlaying)
						{

							audioSource.Pause();
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

			if(shotHit)
			{
				enemyHit.GetComponent<BlimpAIMovement>().immobilized = true;
				break;
			}
			if(!grid.tileInBounds(location) || grid.getTileState(location))
			{
				break;
			}
			yield return new WaitForSeconds(0.03f);
		}
		if(!shotHit)
		{
			harpoon.transform.localScale = Vector3.zero;
		}
		shooting = false;
	}
	public void harpoonTrigger(GameObject other)
	{
		shotHit = true;
		enemyHit = other;
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
				targetEuler.Set(0, 0, 360 - rotationMax);
				break;
			case Direction.Dir.U:
				targetEuler.Set(0, 0, 0);
				break;
			case Direction.Dir.D:
				targetEuler.Set(0, 0, 0);
				break;
		}

		transform.eulerAngles = targetEuler;
		/*
			Vector3 difference = targetEuler - transform.localEulerAngles;
			if(Mathf.Abs(difference.z) > 180)
			{
				print("a");
				difference.z = 360 - difference.z;
				transform.Rotate(difference * speedRotate);
			}
			else
			{
				print("b");
				transform.Rotate(difference * speedRotate);
			}
		*/
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Blimps")
		{
			if(other.GetComponent<BlimpAIMovement>().pumpState == 0)
			{
				playerDied.Invoke();
			}
		}
	}

	private IEnumerator pumpCooldownTimer()
	{
		canPump = false;
		yield return new WaitForSeconds(pumpCooldownTime);
		canPump = true;
	}
}