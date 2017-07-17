using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
	[SerializeField] private int width;
	[SerializeField] private int height;

	private float tileSize = 1.0F;
	private float tileTextureSize = 1.0F;
	private bool[,] tileState;

	private float wallThickness = 0.1F;
	private float wallTextureThickness = 0.1F;
	private bool[,] vWallState;
	private bool[,] hWallState;

	[SerializeField] private Material tileMaterial;

	[SerializeField] private GameObject gridMeshChild;
	private GameObject[] gridMeshChildren = new GameObject[3];
	
	enum ChildIndexes
	{
		tiles = 0,
		vWalls = 1,
		hWalls = 2,
	}

	void Start()
	{
		generateMeshChildren();
	}
	
	void Update()
	{
		
	}
	
	public void loadLevelData(LevelData lD)
	{
		width = lD.width;
		height = lD.height;

		destroyMeshChildren();
		generateMeshChildren();

		for(int i = 0; i < lD.removeTiles.Count; i++)
		{
			destroyTile(lD.removeTiles[i]);
		}
		for(int i = 0; i < lD.removeVWalls.Count; i++)
		{
			destroyVWall(lD.removeVWalls[i]);
		}
		for(int i = 0; i < lD.removeHWalls.Count; i++)
		{
			destroyHWall(lD.removeHWalls[i]);
		}
	}
	private void generateMeshChildren()
	{
		for(int i = 0; i < 3; i++)
		{
			gridMeshChildren[i] = Instantiate(gridMeshChild);
			gridMeshChildren[i].transform.parent = transform;
			gridMeshChildren[i].transform.localPosition = Vector3.zero;
			gridMeshChildren[i].transform.localScale = new Vector3(1, 1, 1);
			Mesh mesh = new Mesh();
			switch(i)
			{
				case (int)ChildIndexes.tiles:
					mesh = generateTileMesh();
					break;
				case (int)ChildIndexes.vWalls:
					mesh = generateVerticalWallMesh();
					break;
				case (int)ChildIndexes.hWalls:
					mesh = generateHorizontalWallMesh();
					break;
			}
			gridMeshChildren[i].GetComponent<MeshFilter>().mesh = mesh;
		}

		gridMeshChildren[(int)ChildIndexes.tiles].GetComponent<MeshRenderer>().material = tileMaterial;

		tileState = new bool[width, height];
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				tileState[i, j] = true;
			}
		}
		vWallState = new bool[width - 1, height];
		for(int i = 0; i < width - 1; i++)
		{
			for(int j = 0; j < height; j++)
			{
				vWallState[i, j] = true;
			}
		}
		hWallState = new bool[width, height - 1];
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height - 1; j++)
			{
				hWallState[i, j] = true;
			}
		}
	}
	private void destroyMeshChildren()
	{
		foreach(Transform child in transform)
		{
			Destroy(child.gameObject);
		}
	}

	public Vector2 getDimensions()
	{
		return new Vector2(width, height) * tileSize;
	}

	public Vector2 convertLocalToScaled(Vector2 local) // Each tile is one wide
	{
		return local * (1.0F / tileSize) / transform.lossyScale.x;
	}
	public Vector2 convertScaledToLocal(Vector2 scaled) // Each tile is one wide
	{
		return scaled * tileSize * transform.lossyScale.x;
	}

	public Vector2 getTileLocation(Vector2 scaled) // Which tile am I in - bottom left is inside, top right is outside
	{
		return new Vector2(Mathf.Floor(scaled.x), Mathf.Floor(scaled.y));
	}

	private int[] getTileUVOrVerticesIndexes(Vector2 tile) // Gets the indexes of the UVs or vertices for a specific tile (order is bl, tl, br, tr)
	{
		int[] UVIndexes = new int[4];
		for(int k = 0; k < 4; k++)
		{
			UVIndexes[k] = ((int)tile.x + (int)tile.y * width) * 4 + k;
		}
		return UVIndexes;
	}
	private int[] getVWallUVOrVerticesIndexes(Vector2 vWall) // Gets the indexes of the UVs or vertices for a specific vWall (order is bl, tl, br, tr)
	{
		int[] UVIndexes = new int[4];
		for(int k = 0; k < 4; k++)
		{
			UVIndexes[k] = ((int)vWall.x + (int)vWall.y * (width - 1)) * 4 + k;
		}
		return UVIndexes;
	}
	private int[] getHWallUVOrVerticesIndexes(Vector2 hWall) // Gets the indexes of the UVs or vertices for a specific hWall (order is bl, tl, br, tr)
	{
		int[] UVIndexes = new int[4];
		for(int k = 0; k < 4; k++)
		{
			UVIndexes[k] = ((int)hWall.x * (height - 1) + (int)hWall.y) * 4 + k;
		}
		return UVIndexes;
	}

	private int[] getTileTriangleIndexes(Vector2 tile) // Gets the indexes of the triangles for a specific tile (order is TRI ONE(bl, tl, br) TRI TWO(tr, br, tl))
	{
		int[] triangles = new int[6];
		for(int k = 0; k < 3; k++)
		{
			triangles[k] = ((int)tile.x + (int)tile.y * width) * 6 + k;
		}
		for(int k = 3; k < 6; k++)
		{
			triangles[k] = ((int)tile.x + (int)tile.y * width) * 6 + k;
		}
		return triangles;
	}

	public void destroyTile(Vector2 tile) // Removes the tile, moving all of its vertices to (0, 0) and setting it false, expensive because we need to change the entire vertices array as part of the standard
	{
		if(tileInBounds(tile))
		{
			if(tileState[(int)tile.x, (int)tile.y])
			{
				tileState[(int)tile.x, (int)tile.y] = false;

				Vector3[] v = gridMeshChildren[(int)ChildIndexes.tiles].GetComponent<MeshFilter>().mesh.vertices;

				int[] VI = getTileUVOrVerticesIndexes(tile);
				for(int i = 0; i < VI.Length; i++)
				{
					v[VI[i]] = Vector3.zero;
				}

				gridMeshChildren[(int)ChildIndexes.tiles].GetComponent<MeshFilter>().mesh.vertices = v;
			}
		}
	}
	public void destroyVWall(Vector2 vWall) // Removes the vWall
	{
		if(vWallInBounds(vWall))
		{
			if(vWallState[(int)vWall.x, (int)vWall.y])
			{
				vWallState[(int)vWall.x, (int)vWall.y] = false;

				Vector3[] v = gridMeshChildren[(int)ChildIndexes.vWalls].GetComponent<MeshFilter>().mesh.vertices;

				int[] VI = getVWallUVOrVerticesIndexes(vWall);
				for(int i = 0; i < VI.Length; i++)
				{
					v[VI[i]] = Vector3.zero;
				}

				gridMeshChildren[(int)ChildIndexes.vWalls].GetComponent<MeshFilter>().mesh.vertices = v;
			}
		}
	}
	public void destroyHWall(Vector2 hWall) // Removes the hWall
	{
		if(hWallInBounds(hWall))
		{
			if(hWallState[(int)hWall.x, (int)hWall.y])
			{
				hWallState[(int)hWall.x, (int)hWall.y] = false;

				Vector3[] v = gridMeshChildren[(int)ChildIndexes.hWalls].GetComponent<MeshFilter>().mesh.vertices;

				int[] VI = getHWallUVOrVerticesIndexes(hWall);
				for(int i = 0; i < VI.Length; i++)
				{
					v[VI[i]] = Vector3.zero;
				}

				gridMeshChildren[(int)ChildIndexes.hWalls].GetComponent<MeshFilter>().mesh.vertices = v;
			}
		}
	}

	public bool tileInBounds(Vector2 tile)
	{
		return (tile.x >= 0 && tile.x < width && tile.y >= 0 && tile.y < height);
	} // Is this tile in bounds
	public bool vWallInBounds(Vector2 vWall)
	{
		return (vWall.x >= 0 && vWall.x < width - 1 && vWall.y >= 0 && vWall.y < height);
	}
	public bool hWallInBounds(Vector2 hWall)
	{
		return (hWall.x >= 0 && hWall.x < width && hWall.y >= 0 && hWall.y < height - 1);
	}

	public bool getTileState(Vector2 tile)
	{
		return tileState[(int)tile.x, (int)tile.y];
	}
	public bool getVWallState(Vector2 vWall)
	{
		return vWallState[(int)vWall.x, (int)vWall.y];
	}
	public bool getHWallState(Vector2 hWall)
	{
		return hWallState[(int)hWall.x, (int)hWall.y];
	}

	private Mesh generateTileMesh() // Generates a mesh for all the tiles, two triangles per tile
	{
		Vector3[] vertices = new Vector3[width * height * 4]; // Each tile gets its own four vertices so that each tile can be uv mapped seperately
		Vector2[] uvs = new Vector2[width * height * 4];
		int[] triangles = new int[width * height * 6];
		for(int i = 0; i < height; i++)
		{
			for(int j = 0; j < width; j++)
			{
				for(int k = 0; k < 4; k++)
				{
					vertices[(j + i * width) * 4 + k] = new Vector3(j * tileSize, i * tileSize) + new Vector3(k / 2 * tileSize, k % 2 * tileSize); // Adds in this order: bottom left, top left, bottom right, top right
					uvs[(j + i * width) * 4 + k] = new Vector2(k / 2 * tileTextureSize, k % 2 * tileTextureSize);
				}
				for(int k = 0; k < 3; k++)
				{
					triangles[(j + i * width) * 6 + k] = (j + i * width) * 4 + k; // First triangle
				}
				for(int k = 3; k < 6; k++)
				{
					triangles[(j + i * width) * 6 + k] = (j + i * width) * 4 + 6 - k; // Second triangle
				}
			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();

		return mesh;
	}
	private Mesh generateVerticalWallMesh() // Generates a mesh for all the vertical walls, two triangles per wall
	{
		Vector3[] vertices = new Vector3[(width - 1) * height * 4]; // Each wall gets its own four vertices so that each tile can be uv mapped seperately
		Vector2[] uvs = new Vector2[(width - 1) * height * 4];
		int[] triangles = new int[(width - 1) * height * 6];
		for(int i = 0; i < height; i++)
		{
			for(int j = 0; j < width - 1; j++)
			{
				for(int k = 0; k < 4; k++)
				{
					vertices[(j + i * (width - 1)) * 4 + k] = new Vector3((j + 1) * tileSize, i * tileSize) + new Vector3((k / 2 * 2 - 1) * wallThickness / 2.0F, k % 2 * tileSize); // Adds in this order: bottom left, top left, bottom right, top right
					uvs[(j + i * (width - 1)) * 4 + k] = new Vector2(k / 2 * wallTextureThickness, k % 2 * tileTextureSize);
				}
				for(int k = 0; k < 3; k++)
				{
					triangles[(j + i * (width - 1)) * 6 + k] = (j + i * (width - 1)) * 4 + k; // First triangle
				}
				for(int k = 3; k < 6; k++)
				{
					triangles[(j + i * (width - 1)) * 6 + k] = (j + i * (width - 1)) * 4 + 6 - k; // Second triangle
				}
			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		return mesh;
	}
	private Mesh generateHorizontalWallMesh() // Generates a mesh for all the horizontal walls, two triangles per wall
	{
		Vector3[] vertices = new Vector3[(height - 1) * width * 4]; // Each wall gets its own four vertices so that each tile can be uv mapped seperately
		Vector2[] uvs = new Vector2[(height - 1) * width * 4];
		int[] triangles = new int[(height - 1) * width * 6];
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height - 1; j++)
			{
				for(int k = 0; k < 4; k++)
				{
					vertices[(j + i * (height - 1)) * 4 + k] = new Vector3(i * tileSize, (j + 1) * tileSize) + new Vector3(k / 2 * tileSize, (k % 2 * 2 - 1) * wallThickness / 2.0F); // Adds in this order: bottom left, top left, bottom right, top right
					uvs[(j + i * (height - 1)) * 4 + k] = new Vector2(k / 2 * wallTextureThickness, k % 2 * tileTextureSize);
				}
				for(int k = 0; k < 3; k++)
				{
					triangles[(j + i * (height - 1)) * 6 + k] = (j + i * (height - 1)) * 4 + k; // First triangle
				}
				for(int k = 3; k < 6; k++)
				{
					triangles[(j + i * (height - 1)) * 6 + k] = (j + i * (height - 1)) * 4 + 6 - k; // Second triangle
				}
			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		return mesh;
	}
}