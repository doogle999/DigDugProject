using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlimpAIMovement : Movement
{
	public enum Mode
	{
		wandering = 0,
		ghosting = 1,
		hunting = 2,
		fleeing = 3,
	}

	[SerializeField] private Mode mode;
	private AStarPathing pathing;

	private float speedGho = 2.0F; // Speed when ghost
	private float speedReg = 2.0F; // Speed when not ghost

	private float ghostMinTime = 5.0F;
	private float ghostMaxTime = 20.0F;

	private bool ghostedYet = false;
	private bool goGhost = false;

	public bool immobilized = false;
	private float destroyAfterPopTime = 0.3F;

	[SerializeField] private int followRange = 5;

	public PlayerMovement player;

	public int pumpState = 0; // 0 nothing, 1 slight, 2 more, 3 popped

	[SerializeField] private Sprite blimpSprite;
	[SerializeField] private Sprite ghostSprite;
	[SerializeField] private Sprite pumpOneSprite;
	[SerializeField] private Sprite pumpTwoSprite;
	[SerializeField] private Sprite popSprite;

	void Start()
	{
		getGrid();
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
		StartCoroutine("activateGhost");
	}

	void Update()
	{
		if(!immobilized)
		{
			switch(mode)
			{
				case Mode.wandering:
				{
					if(onRoute)
					{
						if(grid.tileInBounds(grid.getTileLocation(getNextPosition())))
						{
							if(moveInDirection(speedReg))
							{
								if(!(grid.tileInBounds(grid.getTileLocation(getNextPosition()))) || Direction.passingTileOrWall(grid.getTileLocation(positionScaled), direction, grid))
								{
									findWanderingDirection();
								}
								recalculatePath(ref pathing, player.positionScaled);
								if(pathing.path.Count != 0 && pathing.path.Count <= followRange)
								{
									mode = Mode.hunting;
									findHuntingDirection();
								}
								if(goGhost)
								{
									mode = Mode.ghosting;
									findGhostingDirection();
								}
							}
						}
						else
						{
							onRoute = false;
						}
					}
					else
					{
						findWanderingDirection();
						recalculatePath(ref pathing, player.positionScaled);
						if(pathing.path.Count != 0 && pathing.path.Count <= followRange)
						{
							mode = Mode.hunting;
							findHuntingDirection();
						}
						if(goGhost)
						{
							mode = Mode.ghosting;
							findGhostingDirection();
						}
					}
					break;
				}
				case Mode.ghosting:
				{
					if(grid.tileInBounds(grid.getTileLocation(getNextPosition())))
					{
						if(moveInDirection(speedReg))
						{
							findGhostingDirection();
							if(!grid.getTileState(positionScaled))
							{
								if(ghostedYet)
								{
									GetComponent<SpriteRenderer>().sprite = blimpSprite;
									ghostedYet = false;
									goGhost = false;
									StartCoroutine("activateGhost");
									mode = Mode.wandering;
									findWanderingDirection();
								}
							}
							else
							{
								GetComponent<SpriteRenderer>().sprite = ghostSprite;
								ghostedYet = true;
							}
						}
					}
					break;
				}
				case Mode.hunting:
				{
					if(onRoute)
					{
						if(grid.tileInBounds(grid.getTileLocation(getNextPosition())))
						{
							if(moveInDirection(speedReg))
							{
								recalculatePath(ref pathing, player.positionScaled);
								if(pathing.path.Count != 0 && pathing.path.Count <= followRange)
								{
									findHuntingDirection();
								}
								else
								{
									mode = Mode.wandering;
									findWanderingDirection();
								}
							}
						}
						else
						{
							onRoute = false;
						}
					}
					else
					{
						mode = Mode.wandering;
						findWanderingDirection();
					}
					break;
				}
				case Mode.fleeing:

					break;
			}
		}
		else
		{
			if(pumpState == 1)
			{
				GetComponent<SpriteRenderer>().sprite = pumpOneSprite;
			}
			else if(pumpState == 2)
			{
				GetComponent<SpriteRenderer>().sprite = pumpTwoSprite;
			}
			else if(pumpState == 3)
			{
				GetComponent<SpriteRenderer>().sprite = popSprite;
				StartCoroutine("destroyAfterPop");
				pumpState = 4;
			}
		}
	}

	private void findWanderingDirection()
	{
		int rand = Mathf.FloorToInt(Random.Range(0, 4));
		for(int i = rand; i < rand + 4; i++)
		{
			if(grid.tileInBounds(grid.getTileLocation(positionScaled + Direction.convertDirToUnitVector2((Direction.Dir)(i % 4)))))
			{
				if(!Direction.passingTileOrWall(grid.getTileLocation(positionScaled), (Direction.Dir)(i % 4), grid))
				{
					direction = (Direction.Dir)(i % 4);
					onRoute = true;
					return;
				}
			}
		}
		onRoute = false;
	}

	private void findGhostingDirection()
	{
		Vector2 dis = player.positionScaled - positionScaled;

		if(Mathf.Abs(dis.x) > Mathf.Abs(dis.y))
		{
			direction = (Direction.Dir)(2 - (int)Mathf.Sign(dis.x));
		}
		else
		{
			direction = (Direction.Dir)(1 - (int)Mathf.Sign(dis.y));
		}
	}

	public void recalculatePath(ref AStarPathing pathing, Vector2 pos)
	{
		pathing = new AStarPathing(grid.getTileLocation(pos), grid.getTileLocation(positionScaled), grid);

		pathing.path.Clear();
		pathing.findPath();

		pathing.path.Insert(0, grid.getTileLocation(pos));
		pathing.path.RemoveAt(pathing.path.Count - 1);
	}

	private void findHuntingDirection()
	{
		if(pathing.path.Count != 0)
		{
			Vector2 dis = pathing.path[pathing.path.Count - 1] - grid.getTileLocation(positionScaled);
			pathing.path.RemoveAt(pathing.path.Count - 1);
			direction = (Direction.Dir)(Mathf.RoundToInt(-dis.y) + Mathf.RoundToInt(Mathf.Abs(dis.y)) + Mathf.RoundToInt(-dis.x) + 2 * Mathf.RoundToInt(Mathf.Abs(dis.x)));
			onRoute = true;
		}
		else
		{
			onRoute = false;
		}
	}

	private IEnumerator activateGhost()
	{
		float waitTime = Random.Range(ghostMinTime, ghostMaxTime);

		yield return new WaitForSeconds(waitTime);

		goGhost = true;
	}

	private IEnumerator destroyAfterPop()
	{
		GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds(destroyAfterPopTime);
		Scoring sc = GameObject.Find("Player One Label").GetComponent<Scoring>();
		sc.increaseScore.Invoke((sc.enemyMaxScore - sc.enemyMinScore) * ((grid.getDimensions().y - positionScaled.y) / grid.getDimensions().y) + sc.enemyMinScore);
		Destroy(gameObject);
	}
}
