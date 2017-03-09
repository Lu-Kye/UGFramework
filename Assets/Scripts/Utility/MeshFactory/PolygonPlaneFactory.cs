using UnityEngine;
using System;

namespace UGFramework.MeshFactory
{
	public class PolygonPlane : AbstractMesh<PolygonPlaneConfig>
	{
		protected override void InitMesh()
		{
			base.InitMesh();

			// Calculate triangles
			int trianglesNum = Config.Vertices.Length - 2; // 三角形的数量等于顶点数减2
			int[] triangles = new int[trianglesNum * 3]; // triangles数组大小等于三角形数量乘3
			int f = 0, b = Config.Vertices.Length - 1; // f记录前半部分遍历位置，b记录后半部分遍历位置
			for (int i = 1; i <= trianglesNum; i++) // 每次给triangles数组中的三个元素赋值，共赋值
			{ 
				if (i%2 == 1)
				{
					triangles[3*i - 3] = f++;
					triangles[3*i - 2] = f;
					triangles[3*i - 1] = b; // 正向赋值，对于i=1赋值为：0,1,2
				}
				else
				{
					triangles[3*i - 1] = b--;
					triangles[3*i - 2] = b;
					triangles[3*i - 3] = f; // 逆向赋值，对于i=2赋值为：1,5,6
				}
			}
			this.meshFilter.sharedMesh.triangles = triangles;
			this.meshFilter.sharedMesh.vertices = Config.Vertices;
		}		
	}

	/**
	 * --- DOC BEGIN ---
	 * Polygon vertices order should be clockwise.
	 * --- DOC END ---
	 */
	public class PolygonPlaneConfig : AbstractMeshConfig
	{
		public override MeshType Type
		{
			get
			{
				return MeshType.POLYGON_PLANE;
			}
		}

		public Vector3[] Vertices { get; set; }
	}

	public class PolygonPlaneFactory : AbstractMeshFactory<PolygonPlane, PolygonPlaneConfig>
	{
		public static readonly PolygonPlaneFactory Instance = new PolygonPlaneFactory();
		PolygonPlaneFactory() {}

		public override PolygonPlane Create(PolygonPlaneConfig meshConfig)
		{
			base.Create(meshConfig);

			if (meshConfig.Vertices.Length <= 2)
			{
				throw new Exception("Verts can not be less than 3!");
			}

			var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
			var polygonPlane = go.AddComponent<PolygonPlane>();
			polygonPlane.Config = meshConfig;

			return polygonPlane;
		}
	}
}
