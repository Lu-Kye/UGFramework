using UnityEngine;

public class TilemapDrawerExample : MonoBehaviour 
{
	TilemapDrawer tilemapDrawer;

	void Awake()
	{
		// Ctor tilemap
		int xTileCount = 50;
		int yTileCount = 100;
		int[][] tilemap = new int[xTileCount][];
		for (int i = 0; i < xTileCount; i++)
		{
			tilemap[i] = new int[yTileCount];
			for (int j = 0; j < yTileCount; j++)
			{
				tilemap[i][j] = 1;
			}
		}

		// Init tilemap drawer
		tilemapDrawer = TilemapDrawer.Create(
			tilemap,
			Vector2.zero,
			1,
			1
		);

		tilemapDrawer.Tilemap[13][20] = 0;
		tilemapDrawer.Tilemap[33][33] = 0;
		tilemapDrawer.Tilemap[33][34] = 0;
		tilemapDrawer.Tilemap[48][88] = 0;
	}
}
