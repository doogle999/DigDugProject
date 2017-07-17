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
	private float speedReg = 3.0F; // Speed when not ghost

	private bool ghostedYet = false;
	private bool goGhost = false;

	[SerializeField] private int followRange = 5;

	private PlayerMovement player;

	void Start()
	{
		getGrid();
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

		mode = Mode.hunting;
		recalculatePath(ref pathing, player.positionScaled);
		findHuntingDirection();
	}

	void Update()
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
								ghostedYet = false;
								mode = Mode.wandering;
								findWanderingDirection();
							}
						}
						else
						{
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
							findHuntingDirection();
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
		print(direction);
	}

	public void recalculatePath(ref AStarPathing pathing, Vector2 pos)
	{
		pathing = new AStarPathing(grid.getTileLocation(pos), grid.getTileLocation(positionScaled), grid);

		pathing.findPath();

		print(pathing.path.Count);

		pathing.path.RemoveAt(pathing.path.Count - 1);
		pathing.path.Insert(0, grid.getTileLocation(pos));
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
}
