using UnityEngine;

public class TilemapDrawerExample : MonoBehaviour 
{
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
		TilemapDrawer.Create(
			tilemap,
			Vector2.zero,
			1,
			1
		);

		tilemap[13][20] = 0;
		tilemap[33][33] = 0;
		tilemap[33][34] = 0;
		tilemap[48][88] = 0;
	}
}
