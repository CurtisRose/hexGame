using UnityEngine;
using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour {

	public HexMesh terrain, rivers, roads, water, waterShore, estuaries;

	HexCell[] cells;

	Canvas gridCanvas;

	static Color weights1 = new Color(1f, 0f, 0f);
	static Color weights2 = new Color(0f, 1f, 0f);
	static Color weights3 = new Color(0f, 0f, 1f);

	void Awake () {
		gridCanvas = GetComponentInChildren<Canvas>();

		cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
	}

	public void AddCell (int index, HexCell cell) {
		cells[index] = cell;
		cell.chunk = this;
		cell.transform.SetParent(transform, false);
		cell.uiRect.SetParent(gridCanvas.transform, false);
	}

	public void Refresh () {
		enabled = true;
	}

	public void ShowUI (bool visible) {
		gridCanvas.gameObject.SetActive(visible);
	}

	void LateUpdate () {
		Triangulate();
		enabled = false;
	}

    private void Update()
    {

    }

	public Vector3 GetCenterPosition()
    {
		Vector3 position = cells[12].transform.position;
		position.y = 0.0f;
		return position;
    }

	public void ToggleMeshVisibility(bool toggle)
	{
		//public HexMesh terrain, rivers, roads, water, waterShore, estuaries;
		foreach(MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
			// If the meshRenderer has a HexMesh in the same level, meaning, it is one of our HexMeshes
			if (meshRenderer.GetComponent<HexMesh>())
            {
				meshRenderer.enabled = toggle;
            }
        }
	}

	void ClearHexMesh()
    {
		terrain.Clear();
		rivers.Clear();
		roads.Clear();
		water.Clear();
		waterShore.Clear();
		estuaries.Clear();
	}

	void ApplyHexMesh()
    {
		terrain.Apply();
		rivers.Apply();
		roads.Apply();
		water.Apply();
		waterShore.Apply();
		estuaries.Apply();
	}

	public void Triangulate () {
		ClearHexMesh();
		for (int i = 0; i < cells.Length; i++) {
			Triangulate(cells[i]);
		}
		ApplyHexMesh();
	}

	void Triangulate (HexCell cell) {
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
		{
			TriangulateWater(d, cell, cell.Position);
		}
		cell.Triangulate();
	}

	void TriangulateWater (
		HexDirection direction, HexCell cell, Vector3 center
	) {
		center.y = cell.WaterSurfaceY;

		HexCell neighbor = cell.GetNeighbor(direction);
		if (neighbor != null && !neighbor.IsUnderwater) {
			TriangulateWaterShore(direction, cell, neighbor, center);
		}
		else {
			TriangulateOpenWater(direction, cell, neighbor, center);
		}
	}

	void TriangulateOpenWater (
		HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
	) {
		Vector3 c1 = center + HexMetrics.GetFirstWaterCorner(direction);
		Vector3 c2 = center + HexMetrics.GetSecondWaterCorner(direction);

		water.AddTriangle(center, c1, c2);
		Vector3 indices;
		indices.x = indices.y = indices.z = cell.Index;
		water.AddTriangleCellData(indices, weights1);

		if (direction <= HexDirection.SE && neighbor != null) {
			Vector3 bridge = HexMetrics.GetWaterBridge(direction);
			Vector3 e1 = c1 + bridge;
			Vector3 e2 = c2 + bridge;

			water.AddQuad(c1, c2, e1, e2);
			indices.y = neighbor.Index;
			water.AddQuadCellData(indices, weights1, weights2);

			if (direction <= HexDirection.E) {
				HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
				if (nextNeighbor == null || !nextNeighbor.IsUnderwater) {
					return;
				}
				water.AddTriangle(
					c2, e2, c2 + HexMetrics.GetWaterBridge(direction.Next())
				);
				indices.z = nextNeighbor.Index;
				water.AddTriangleCellData(
					indices, weights1, weights2, weights3
				);
			}
		}
	}

	void TriangulateWaterShore (
		HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
	) {
		EdgeVertices e1 = new EdgeVertices(
			center + HexMetrics.GetFirstWaterCorner(direction),
			center + HexMetrics.GetSecondWaterCorner(direction)
		);
		water.AddTriangle(center, e1.v1, e1.v2);
		water.AddTriangle(center, e1.v2, e1.v3);
		water.AddTriangle(center, e1.v3, e1.v4);
		water.AddTriangle(center, e1.v4, e1.v5);
		Vector3 indices;
		indices.x = indices.z = cell.Index;
		indices.y = neighbor.Index;
		water.AddTriangleCellData(indices, weights1);
		water.AddTriangleCellData(indices, weights1);
		water.AddTriangleCellData(indices, weights1);
		water.AddTriangleCellData(indices, weights1);

		Vector3 center2 = neighbor.Position;
		if (neighbor.ColumnIndex < cell.ColumnIndex - 1) {
			center2.x += HexMetrics.wrapSize * HexMetrics.innerDiameter;
		}
		else if (neighbor.ColumnIndex > cell.ColumnIndex + 1) {
			center2.x -= HexMetrics.wrapSize * HexMetrics.innerDiameter;
		}
		center2.y = center.y;
		EdgeVertices e2 = new EdgeVertices(
			center2 + HexMetrics.GetSecondSolidCorner(direction.Opposite()),
			center2 + HexMetrics.GetFirstSolidCorner(direction.Opposite())
		);

		if (cell.HasRiverThroughEdge(direction)) {
			TriangulateEstuary(
				e1, e2,
				cell.HasIncomingRiver && cell.IncomingRiver == direction, indices
			);
		}
		else {
			waterShore.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
			waterShore.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
			waterShore.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
			waterShore.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
			waterShore.AddQuadUV(0f, 0f, 0f, 1f);
			waterShore.AddQuadUV(0f, 0f, 0f, 1f);
			waterShore.AddQuadUV(0f, 0f, 0f, 1f);
			waterShore.AddQuadUV(0f, 0f, 0f, 1f);
			waterShore.AddQuadCellData(indices, weights1, weights2);
			waterShore.AddQuadCellData(indices, weights1, weights2);
			waterShore.AddQuadCellData(indices, weights1, weights2);
			waterShore.AddQuadCellData(indices, weights1, weights2);
		}

		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (nextNeighbor != null) {
			Vector3 center3 = nextNeighbor.Position;
			if (nextNeighbor.ColumnIndex < cell.ColumnIndex - 1) {
				center3.x += HexMetrics.wrapSize * HexMetrics.innerDiameter;
			}
			else if (nextNeighbor.ColumnIndex > cell.ColumnIndex + 1) {
				center3.x -= HexMetrics.wrapSize * HexMetrics.innerDiameter;
			}
			Vector3 v3 = center3 + (nextNeighbor.IsUnderwater ?
				HexMetrics.GetFirstWaterCorner(direction.Previous()) :
				HexMetrics.GetFirstSolidCorner(direction.Previous()));
			v3.y = center.y;
			waterShore.AddTriangle(e1.v5, e2.v5, v3);
			waterShore.AddTriangleUV(
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(0f, nextNeighbor.IsUnderwater ? 0f : 1f)
			);
			indices.z = nextNeighbor.Index;
			waterShore.AddTriangleCellData(
				indices, weights1, weights2, weights3
			);
		}
	}

	void TriangulateEstuary (
		EdgeVertices e1, EdgeVertices e2, bool incomingRiver, Vector3 indices
	) {
		waterShore.AddTriangle(e2.v1, e1.v2, e1.v1);
		waterShore.AddTriangle(e2.v5, e1.v5, e1.v4);
		waterShore.AddTriangleUV(
			new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)
		);
		waterShore.AddTriangleUV(
			new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)
		);
		waterShore.AddTriangleCellData(indices, weights2, weights1, weights1);
		waterShore.AddTriangleCellData(indices, weights2, weights1, weights1);

		estuaries.AddQuad(e2.v1, e1.v2, e2.v2, e1.v3);
		estuaries.AddTriangle(e1.v3, e2.v2, e2.v4);
		estuaries.AddQuad(e1.v3, e1.v4, e2.v4, e2.v5);

		estuaries.AddQuadUV(
			new Vector2(0f, 1f), new Vector2(0f, 0f),
			new Vector2(1f, 1f), new Vector2(0f, 0f)
		);
		estuaries.AddTriangleUV(
			new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(1f, 1f)
		);
		estuaries.AddQuadUV(
			new Vector2(0f, 0f), new Vector2(0f, 0f),
			new Vector2(1f, 1f), new Vector2(0f, 1f)
		);
		estuaries.AddQuadCellData(
			indices, weights2, weights1, weights2, weights1
		);
		estuaries.AddTriangleCellData(indices, weights1, weights2, weights2);
		estuaries.AddQuadCellData(indices, weights1, weights2);

		if (incomingRiver) {
			estuaries.AddQuadUV2(
				new Vector2(1.5f, 1f), new Vector2(0.7f, 1.15f),
				new Vector2(1f, 0.8f), new Vector2(0.5f, 1.1f)
			);
			estuaries.AddTriangleUV2(
				new Vector2(0.5f, 1.1f),
				new Vector2(1f, 0.8f),
				new Vector2(0f, 0.8f)
			);
			estuaries.AddQuadUV2(
				new Vector2(0.5f, 1.1f), new Vector2(0.3f, 1.15f),
				new Vector2(0f, 0.8f), new Vector2(-0.5f, 1f)
			);
		}
		else {
			estuaries.AddQuadUV2(
				new Vector2(-0.5f, -0.2f), new Vector2(0.3f, -0.35f),
				new Vector2(0f, 0f), new Vector2(0.5f, -0.3f)
			);
			estuaries.AddTriangleUV2(
				new Vector2(0.5f, -0.3f),
				new Vector2(0f, 0f),
				new Vector2(1f, 0f)
			);
			estuaries.AddQuadUV2(
				new Vector2(0.5f, -0.3f), new Vector2(0.7f, -0.35f),
				new Vector2(1f, 0f), new Vector2(1.5f, -0.2f)
			);
		}
	}

}