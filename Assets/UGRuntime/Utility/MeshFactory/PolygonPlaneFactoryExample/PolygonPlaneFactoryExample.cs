using UnityEngine;

namespace UGFramework.Utility
{
	public class PolygonPlaneFactoryExample : MonoBehaviour 
	{
		public GameObject[] verts;
		
		public Material material;

		PolygonPlane polygonPlane;

		[ContextMenu("CreatePolygonPlane")]
		void CreatePolygonPlane()
		{
			if (this.polygonPlane != null)
			{
				GameObject.DestroyImmediate(this.polygonPlane.gameObject);
				this.polygonPlane = null;
			}

			var meshConfig = new PolygonPlaneConfig();
			meshConfig.Vertices = new Vector3[verts.Length];
			meshConfig.Material = material;
			for (int i = 0; i < verts.Length; i++)
			{
				meshConfig.Vertices[i] = verts[i].transform.position;
			}

			this.polygonPlane = PolygonPlaneFactory.Instance.Create(meshConfig);
		}
	}
}