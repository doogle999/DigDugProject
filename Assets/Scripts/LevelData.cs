using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
	public int width;
	public int height;

	public List<Vector2> removeTiles;
	public List<Vector2> removeVWalls;
	public List<Vector2> removeHWalls;

	public List<Vector2> blimps;
	public Vector2 player;

	public LevelData(int w, int h, List<Vector2> rT, List<Vector2> rV, List<Vector2> rH, List<Vector2> b, Vector2 p)
	{
		width = w;
		height = h;
		removeTiles = rT;
		removeVWalls = rV;
		removeHWalls = rH;
		blimps = b;
		player = p;
	}	
}

/*List<Vector2> til = new List<Vector2>();
				til.Add(new Vector2(4, 4));
				LevelData lD = new LevelData(5, 5, til, new List<Vector2>(), new List<Vector2>());
				grid.loadLevelData(lD);*/
/*AStarPathing pathing = new AStarPathing(grid.getTileLocation(positionScaled), new Vector2(0, 0), grid);
pathing.findPath();

for(int i = 0; i < pathing.path.Count; i++)
{
	GameObject s = Instantiate(sphere);
	s.transform.parent = grid.transform.parent;
	s.transform.localScale = new Vector3(1, 1, 1);
	s.transform.position = grid.convertScaledToLocal(pathing.path[i] + new Vector2(0.5F, 0.5F)) + (Vector2)grid.transform.position;
}
*/
