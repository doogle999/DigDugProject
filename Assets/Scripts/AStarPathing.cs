using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathing
{
	private Vector2 start;
	private Vector2 end;
	private Grid grid;

	private int[,] stateMap; // 0 untested, 1 closed, 2 open
	private Vector2[,] parentMap;

	public List<Vector2> path = new List<Vector2>();

	public AStarPathing(Vector2 s, Vector2 e, Grid g)
	{
		start = s;
		end = e;
		grid = g;
		stateMap = new int[(int)g.getDimensions().x, (int)g.getDimensions().y];
		parentMap = new Vector2[(int)g.getDimensions().x, (int)g.getDimensions().y];

		for(int i = 0; i < stateMap.GetLength(0); i++)
		{
			for(int j = 0; j < stateMap.GetLength(1); j++)
			{
				stateMap[i, j] = 0;
				parentMap[i, j] = new Vector2(i, j);
			}
		}
	}

	public void findPath()
	{
		bool worked = search(start);
		if(worked)
		{
			Vector2 prevPos = end;
			while(parentMap[(int)prevPos.x, (int)prevPos.y] != prevPos)
			{
				path.Add(prevPos);
				prevPos = parentMap[(int)prevPos.x, (int)prevPos.y];
			}
			path.Reverse();
		}
	}

	private bool search(Vector2 pos)
	{
		stateMap[(int)pos.x, (int)pos.y] = 1;

		List<Vector2> possibleNextPoints = new List<Vector2>();

		for(int i = 0; i < 4; i++)
		{
			Vector2 dirPos = pos + Direction.convertDirToUnitVector2((Direction.Dir)i);

			if(grid.tileInBounds(grid.getTileLocation(dirPos)))
			{
				if(!Direction.passingTileOrWall(pos, (Direction.Dir)i, grid) && stateMap[(int)dirPos.x, (int)dirPos.y] != 1)
				{
					if(stateMap[(int)dirPos.x, (int)dirPos.y] == 2)
					{
						float distanceCurrentParent = getDistanceOnPath(dirPos);
						Vector2 tempParent = parentMap[(int)dirPos.x, (int)dirPos.y];
						parentMap[(int)dirPos.x, (int)dirPos.y] = pos;
						float distanceNewParent = getDistanceOnPath(dirPos);

						if(distanceNewParent < distanceCurrentParent)
						{
							possibleNextPoints.Add(dirPos);
						}
						else
						{
							parentMap[(int)dirPos.x, (int)dirPos.y] = tempParent;
						}
					}
					else
					{
						parentMap[(int)dirPos.x, (int)dirPos.y] = pos;
						stateMap[(int)dirPos.x, (int)dirPos.y] = 2;
						possibleNextPoints.Add(dirPos);
					}
				}
			}
		}

		possibleNextPoints.Sort
		(
			delegate(Vector2 p1, Vector2 p2)
			{
				return (getDistanceOnPath(p1) + getDistance(p1, end)).CompareTo(getDistanceOnPath(p2) + getDistance(p2, end));
			}
		);
		for(int i = possibleNextPoints.Count - 1; i >= 0; i--)
		{
			if(possibleNextPoints[i] == end)
			{
				return true;
			}
			else
			{
				if(search(possibleNextPoints[i]))
				{
					return true;
				}
			}
		}

		return false;
	}

	private float getDistance(Vector2 start, Vector2 end) // Manhattan
	{
		return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);
	}

	private float getDistanceOnPath(Vector2 pos)
	{
		int i = 0;
		Vector2 prevPos = pos;
		while(parentMap[(int)prevPos.x, (int)prevPos.y] != prevPos)
		{
			i++;
			prevPos = parentMap[(int)prevPos.x, (int)prevPos.y];
		}
		return i;
	}
}