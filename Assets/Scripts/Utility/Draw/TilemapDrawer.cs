using UnityEngine;

public class TilemapDrawer : MonoBehaviour
{
	public static TilemapDrawer Create(int[][] tilemap, Vector3 startPos, float xTileWidth, float yTileWidth)
	{
		var go = new GameObject("TilemapDrawer");
		var drawer = go.AddComponent<TilemapDrawer>();

		drawer.Tilemap = tilemap;
		drawer.transform.position = startPos;
		drawer.XTileWidth = xTileWidth;
		drawer.ZTileWidth = yTileWidth;

		return drawer;
	}

	/**
	 * new int[2][2] {
     *   new int[] {0, 1},	
     *   new int[] {0, 1},	
	 * }
	 * 1 is enable
	 * 0 is disable
	 */
	public int[][] Tilemap { get; set; }

	public float XTileWidth { get; set; }
	public float ZTileWidth { get; set; }

	float _spaceBetweenRects = 0.2f;
	public float SpaceBetweenRects 
	{
		get
		{
			return _spaceBetweenRects;
		}
		set
		{
			_spaceBetweenRects = value;
		}
	}

	// Color _lineColor = new Color(Color.green.r, Color.green.g, Color.green.b, Color.green.a * 0.5f);
	// public Color LineColor 
	// {
	// 	get
	// 	{
	// 		return _lineColor;
	// 	}
	// 	set
	// 	{
	// 		_lineColor = value;
	// 	}
	// }
	Color _enableColor = Color.green;
	public Color EnableColor 
	{
		get
		{
			return _enableColor;
		}
		set
		{
			_enableColor = value;
		}
	}

	Color _disableColor = Color.red;
	public Color DisableColor 
	{
		get
		{
			return _disableColor;
		}
		set
		{
			_disableColor = value;
		}
	}

	Material _material;
	void CreateLineMaterial()
	{
		if (!_material)
		{
			// Unity has a built-in shader that is useful for drawing
			// simple colored things.
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			_material = new Material(shader);
			_material.hideFlags = HideFlags.HideAndDontSave;
			// Turn on alpha blending
			_material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			_material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			// Turn backface culling off
			_material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			// Turn off depth writes
			_material.SetInt("_ZWrite", 0);
		}
	}

	// Will be called after all regular rendering is done
	void OnRenderObject()
	{
		CreateLineMaterial();
		// Apply the material
		_material.SetPass(0);

		GL.PushMatrix();
		// Set transformation matrix for drawing to
		// match our transform
		GL.MultMatrix(transform.localToWorldMatrix);

		// Draw lines
		// GL.Begin(GL.LINES);
		// // Vertex colors change from red to green
		// GL.Color(this.LineColor);

		// // GL.Vertex3(0, 0, 0);
		// // GL.Vertex3(1, 0, 0);

		// // Draw z from start to end
		// for (int x = 0; x < this.Tilemap.Length; ++x)
		// {
		// 	GL.Vertex3(x * this.XTileWidth, 0, 0);
		// 	GL.Vertex3(x * this.XTileWidth, 0, (this.Tilemap[0].Length - 1) * this.ZTileWidth);	
		// }

		// // Draw x from start to end
		// for (int z = 0; z < this.Tilemap[0].Length; ++z)
		// {
		// 	GL.Vertex3(0, 0, z * this.ZTileWidth);
		// 	GL.Vertex3((this.Tilemap.Length - 1) * this.XTileWidth, 0, z * this.ZTileWidth);
		// }
		// GL.End();

		// Draw rects
		GL.Begin(GL.QUADS);
		for (int x = 0; x < this.Tilemap.Length - 1; ++x)	
		{
			for (int z = 0; z < this.Tilemap[0].Length - 1; ++z)
			{
				bool enable = this.Tilemap[x][z] == 1;
				GL.Color(enable ? this.EnableColor : this.DisableColor);

				// lt
				GL.Vertex3(x * this.XTileWidth + _spaceBetweenRects, 0, (z + 1) * this.ZTileWidth - _spaceBetweenRects);	
				// rt
				GL.Vertex3((x + 1) * this.XTileWidth - _spaceBetweenRects, 0, (z + 1) * this.ZTileWidth - _spaceBetweenRects);	
				// rb
				GL.Vertex3((x + 1) * this.XTileWidth - _spaceBetweenRects, 0, z * this.ZTileWidth + _spaceBetweenRects);	
				// lb
				GL.Vertex3(x * this.XTileWidth + _spaceBetweenRects, 0, z * this.ZTileWidth + _spaceBetweenRects);	
			}
		}
		GL.End();

		GL.PopMatrix();
	}
}
