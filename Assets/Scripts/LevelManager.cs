using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager
{
	static List<Vector2> removedTiles = new List<Vector2>();
	static List<Vector2> removedVertical = new List<Vector2>();
	static List<Vector2> removedHorizontal = new List<Vector2>();
	static List<Vector2> blimp = new List<Vector2>();
	static Vector2 player = new Vector2();

	public static LevelData loadLevelOne()
	{
		removedTiles.Add (new Vector2(1.0F, 10.0F));
		removedTiles.Add (new Vector2(1.0F, 9.0F));
		removedTiles.Add (new Vector2(1.0F, 8.0F));
		removedTiles.Add (new Vector2(1.0F, 7.0F));
		removedTiles.Add (new Vector2(1.0F, 4.0F));
		removedTiles.Add (new Vector2(2.0F, 4.0F));
		removedTiles.Add (new Vector2(3.0F, 4.0F));
		removedTiles.Add (new Vector2(4.0F, 4.0F));
		removedTiles.Add (new Vector2(4.0F, 6.0F));
		removedTiles.Add (new Vector2(5.0F, 6.0F));
		removedTiles.Add (new Vector2(6.0F, 6.0F));
		removedTiles.Add (new Vector2(5.0F, 7.0F));
		removedTiles.Add (new Vector2(5.0F, 8.0F));
		removedTiles.Add (new Vector2(5.0F, 9.0F));
		removedTiles.Add (new Vector2(5.0F, 10.0F));
		removedTiles.Add (new Vector2(5.0F, 11.0F));
		removedTiles.Add (new Vector2(8.0F, 5.0F));
		removedTiles.Add (new Vector2(8.0F, 4.0F));
		removedTiles.Add (new Vector2(8.0F, 3.0F));
		removedTiles.Add (new Vector2(8.0F, 2.0F));
		removedTiles.Add (new Vector2(8.0F, 1.0F));
		removedTiles.Add (new Vector2(8.0F, 10.0F));
		removedTiles.Add (new Vector2(9.0F, 10.0F));
		removedTiles.Add (new Vector2(10.0F, 10.0F));
		removedTiles.Add (new Vector2(0.0F, 12.0F));
		removedTiles.Add (new Vector2(1.0F, 12.0F));
		removedTiles.Add (new Vector2(2.0F, 12.0F));
		removedTiles.Add (new Vector2(3.0F, 12.0F));
		removedTiles.Add (new Vector2(4.0F, 12.0F));
		removedTiles.Add (new Vector2(5.0F, 12.0F));
		removedTiles.Add (new Vector2(6.0F, 12.0F));
		removedTiles.Add (new Vector2(7.0F, 12.0F));
		removedTiles.Add (new Vector2(8.0F, 12.0F));
		removedTiles.Add (new Vector2(9.0F, 12.0F));
		removedTiles.Add (new Vector2(10.0F, 12.0F));
		removedTiles.Add (new Vector2(11.0F, 12.0F));

		removedVertical.Add (new Vector2(1.0F, 4.0F));
		removedVertical.Add (new Vector2(2.0F, 4.0F));
		removedVertical.Add (new Vector2(3.0F, 4.0F));
		removedVertical.Add (new Vector2(4.0F, 6.0F));
		removedVertical.Add (new Vector2(5.0F, 6.0F));
		removedVertical.Add (new Vector2(8.0F, 10.0F));
		removedVertical.Add (new Vector2(9.0F, 10.0F));

		removedVertical.Add (new Vector2(0.0F, 12.0F));
		removedVertical.Add (new Vector2(1.0F, 12.0F));
		removedVertical.Add (new Vector2(2.0F, 12.0F));
		removedVertical.Add (new Vector2(3.0F, 12.0F));
		removedVertical.Add (new Vector2(4.0F, 12.0F));
		removedVertical.Add (new Vector2(5.0F, 12.0F));
		removedVertical.Add (new Vector2(6.0F, 12.0F));
		removedVertical.Add (new Vector2(7.0F, 12.0F));
		removedVertical.Add (new Vector2(8.0F, 12.0F));
		removedVertical.Add (new Vector2(9.0F, 12.0F));
		removedVertical.Add (new Vector2(10.0F, 12.0F));
		removedVertical.Add (new Vector2(11.0F, 12.0F));

		removedHorizontal.Add (new Vector2(1.0F, 9.0F));
		removedHorizontal.Add (new Vector2(1.0F, 8.0F));
		removedHorizontal.Add (new Vector2(1.0F, 7.0F));
		removedHorizontal.Add (new Vector2(5.0F, 5.0F));
		removedHorizontal.Add (new Vector2(5.0F, 6.0F));
		removedHorizontal.Add (new Vector2(5.0F, 7.0F));
		removedHorizontal.Add (new Vector2(5.0F, 8.0F));
		removedHorizontal.Add (new Vector2(5.0F, 9.0F));
		removedHorizontal.Add (new Vector2(5.0F, 10.0F));
		removedHorizontal.Add (new Vector2(5.0F, 11.0F));
		removedHorizontal.Add (new Vector2(8.0F, 4.0F));
		removedHorizontal.Add (new Vector2(8.0F, 3.0F));
		removedHorizontal.Add (new Vector2(8.0F, 2.0F));
		removedHorizontal.Add (new Vector2(8.0F, 1.0F));

		blimp.Add (new Vector2 (1.5F, 10.5F));
		blimp.Add (new Vector2 (3.5F, 4.5F));
		blimp.Add (new Vector2 (8.5F, 3.5F));
		blimp.Add (new Vector2 (9.5F, 10.5F));
		player.x = 5.5F;
		player.y = 6.5F;

		LevelData levelOne = new LevelData(12, 13, removedTiles, removedVertical, removedHorizontal, blimp, player);
		return levelOne;
	}
}
