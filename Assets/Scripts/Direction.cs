using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction
{
	public enum Dir
	{
		U = 0,
		R = 1,
		D = 2,
		L = 3
	}

	public static Vector2 convertDirToUnitVector2(Dir dir)
	{
		return new Vector2(Mathf.Round(Mathf.Sin(Mathf.PI / 2.0F * (float)dir)), Mathf.Round(Mathf.Cos(Mathf.PI / 2.0F * (float)dir)));
	}
	public static bool passingTileOrWall(Vector2 positionScaled, Dir direction, Grid grid)
	{
		Vector2 nextPositionScaled = positionScaled + convertDirToUnitVector2(direction);

		if(grid.getTileState(grid.getTileLocation(nextPositionScaled)))
		{
			return true;
		}

		Vector2 usePositionScaled = nextPositionScaled;
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
					return true;
				}
			}
		}
		else
		{
			if(grid.vWallInBounds(usePositionScaled))
			{
				if(grid.getVWallState(usePositionScaled))
				{
					return true;
				}
			}
		}
		return false;
	}
}