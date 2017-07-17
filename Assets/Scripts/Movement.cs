using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
	protected Grid grid;

	protected Direction.Dir direction = Direction.Dir.U;
	protected bool onRoute = false;
	protected float distance = 0.0F; // Distance onRoute, 0 to 1
	public Vector2 positionScaled = new Vector2(0.5F, 0.5F);

	protected void getGrid()
	{
		grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
	}

	protected void setTransform(Vector2 positionScaled, Direction.Dir direction, float distance)
	{
		transform.position = grid.convertScaledToLocal(positionScaled) + (Vector2)grid.transform.position;
		transform.position += (Vector3)grid.convertScaledToLocal(Direction.convertDirToUnitVector2(direction) * distance);
	}

	protected bool moveInDirection(float speed)
	{
		bool finished = false;

		Vector2 nextPositionScaled = getNextPosition();
		moveAtSpeed(speed);
		if(distance >= 1.0F) // Passed through whole square, change direction
		{
			resetAtNewPosition();
			finished = true;
		}
		setTransform(positionScaled, direction, distance);

		return finished;
	}
	protected void resetAtNewPosition()
	{
		distance = 0.0F;
		positionScaled = getNextPosition();
	}

	protected void moveAtSpeed(float speed)
	{
		distance += speed * Time.deltaTime;
	}

	protected Vector2 getNextPosition()
	{
		return positionScaled + Direction.convertDirToUnitVector2(direction);
	}
}
