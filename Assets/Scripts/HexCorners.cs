using UnityEngine;

public static class HexCorners
{
	static Color weights1 = new Color(1f, 0f, 0f);
	static Color weights2 = new Color(0f, 1f, 0f);
	static Color weights3 = new Color(0f, 0f, 1f);

    public static void Triangulate(HexDirection rightDirection, HexCell cell)
    {
		Vector3 bridge = HexMetrics.GetBridge(rightDirection);
		HexCell neighbor = cell.GetNeighbor(rightDirection);
		HexMesh hexMesh = cell.chunk.terrain;

		Vector3 center = cell.Position;
		EdgeVertices e1 = new EdgeVertices(
			center + HexMetrics.GetFirstSolidCorner(rightDirection),
			center + HexMetrics.GetSecondSolidCorner(rightDirection)
		);

		bridge.y = neighbor.Position.y - cell.Position.y;
		EdgeVertices e2 = new EdgeVertices(
			e1.v1 + bridge,
			e1.v5 + bridge
		);

		HexCell nextNeighbor = cell.GetNeighbor(rightDirection.Next());
		if (rightDirection <= HexDirection.E && nextNeighbor != null)
		{
			Vector3 v5 = e1.v5 + HexMetrics.GetBridge(rightDirection.Next());
			v5.y = nextNeighbor.Position.y;

			if (cell.Elevation <= neighbor.Elevation)
			{
				if (cell.Elevation <= nextNeighbor.Elevation)
				{
					TriangulateCorner(
						e1.v5, cell, e2.v5, neighbor, v5, nextNeighbor, hexMesh
					);
				}
				else
				{
					TriangulateCorner(
						v5, nextNeighbor, e1.v5, cell, e2.v5, neighbor, hexMesh
					);
				}
			}
			else if (neighbor.Elevation <= nextNeighbor.Elevation)
			{
				TriangulateCorner(
					e2.v5, neighbor, v5, nextNeighbor, e1.v5, cell, hexMesh
				);
			}
			else
			{
				TriangulateCorner(
					v5, nextNeighbor, e1.v5, cell, e2.v5, neighbor, hexMesh
				);
			}
		}
    }

	static void TriangulateCorner(
		Vector3 bottom, HexCell bottomCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell,
		HexMesh hexMesh)
	{
		HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
		HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

		if (leftEdgeType == HexEdgeType.Slope)
		{
			if (rightEdgeType == HexEdgeType.Slope)
			{
				TriangulateCornerTerraces(
					bottom, bottomCell, left, leftCell, right, rightCell, hexMesh
				);
			}
			else if (rightEdgeType == HexEdgeType.Flat)
			{
				TriangulateCornerTerraces(
					left, leftCell, right, rightCell, bottom, bottomCell, hexMesh
				);
			}
			else
			{
				TriangulateCornerTerracesCliff(
					bottom, bottomCell, left, leftCell, right, rightCell, hexMesh
				);
			}
		}
		else if (rightEdgeType == HexEdgeType.Slope)
		{
			if (leftEdgeType == HexEdgeType.Flat)
			{
				TriangulateCornerTerraces(
					right, rightCell, bottom, bottomCell, left, leftCell, hexMesh
				);
			}
			else
			{
				TriangulateCornerCliffTerraces(
					bottom, bottomCell, left, leftCell, right, rightCell, hexMesh
				);
			}
		}
		else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
		{
			if (leftCell.Elevation < rightCell.Elevation)
			{
				TriangulateCornerCliffTerraces(
					right, rightCell, bottom, bottomCell, left, leftCell, hexMesh
				);
			}
			else
			{
				TriangulateCornerTerracesCliff(
					left, leftCell, right, rightCell, bottom, bottomCell, hexMesh
				);
			}
		}
		else
		{
			hexMesh.AddTriangle(bottom, left, right);
			Vector3 indices;
			indices.x = bottomCell.Index;
			indices.y = leftCell.Index;
			indices.z = rightCell.Index;
			hexMesh.AddTriangleCellData(indices, weights1, weights2, weights3);
		}
	}

	static void TriangulateCornerTerraces(
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell,
		HexMesh hexMesh
	)
	{
		Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
		Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
		Color w3 = HexMetrics.TerraceLerp(weights1, weights2, 1);
		Color w4 = HexMetrics.TerraceLerp(weights1, weights3, 1);
		Vector3 indices;
		indices.x = beginCell.Index;
		indices.y = leftCell.Index;
		indices.z = rightCell.Index;

		hexMesh.AddTriangle(begin, v3, v4);
		hexMesh.AddTriangleCellData(indices, weights1, w3, w4);

		for (int i = 2; i < HexMetrics.terraceSteps; i++)
		{
			Vector3 v1 = v3;
			Vector3 v2 = v4;
			Color w1 = w3;
			Color w2 = w4;
			v3 = HexMetrics.TerraceLerp(begin, left, i);
			v4 = HexMetrics.TerraceLerp(begin, right, i);
			w3 = HexMetrics.TerraceLerp(weights1, weights2, i);
			w4 = HexMetrics.TerraceLerp(weights1, weights3, i);
			hexMesh.AddQuad(v1, v2, v3, v4);
			hexMesh.AddQuadCellData(indices, w1, w2, w3, w4);
		}

		hexMesh.AddQuad(v3, v4, left, right);
		hexMesh.AddQuadCellData(indices, w3, w4, weights2, weights3);
	}

	static void TriangulateCornerTerracesCliff(
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell,
		HexMesh hexMesh
	)
	{
		float b = 1f / (rightCell.Elevation - beginCell.Elevation);
		if (b < 0)
		{
			b = -b;
		}
		Vector3 boundary = Vector3.Lerp(
			HexMetrics.Perturb(begin), HexMetrics.Perturb(right), b
		);
		Color boundaryWeights = Color.Lerp(weights1, weights3, b);
		Vector3 indices;
		indices.x = beginCell.Index;
		indices.y = leftCell.Index;
		indices.z = rightCell.Index;

		TriangulateBoundaryTriangle(
			begin, weights1, left, weights2, boundary, boundaryWeights, indices, hexMesh
		);

		if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
		{
			TriangulateBoundaryTriangle(
				left, weights2, right, weights3,
				boundary, boundaryWeights, indices, hexMesh
			);
		}
		else
		{
			hexMesh.AddTriangleUnperturbed(
				HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary
			);
			hexMesh.AddTriangleCellData(
				indices, weights2, weights3, boundaryWeights
			);
		}
	}

	static void TriangulateCornerCliffTerraces(
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell,
		HexMesh hexMesh
	)
	{
		float b = 1f / (leftCell.Elevation - beginCell.Elevation);
		if (b < 0)
		{
			b = -b;
		}
		Vector3 boundary = Vector3.Lerp(
			HexMetrics.Perturb(begin), HexMetrics.Perturb(left), b
		);
		Color boundaryWeights = Color.Lerp(weights1, weights2, b);
		Vector3 indices;
		indices.x = beginCell.Index;
		indices.y = leftCell.Index;
		indices.z = rightCell.Index;

		TriangulateBoundaryTriangle(
			right, weights3, begin, weights1, boundary, boundaryWeights, indices, hexMesh
		);

		if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
		{
			TriangulateBoundaryTriangle(
				left, weights2, right, weights3,
				boundary, boundaryWeights, indices, hexMesh
			);
		}
		else
		{
			hexMesh.AddTriangleUnperturbed(
				HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary
			);
			hexMesh.AddTriangleCellData(
				indices, weights2, weights3, boundaryWeights
			);
		}
	}

	static void TriangulateBoundaryTriangle(
	Vector3 begin, Color beginWeights,
	Vector3 left, Color leftWeights,
	Vector3 boundary, Color boundaryWeights, 
	Vector3 indices, HexMesh hexMesh
)
	{
		Vector3 v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, 1));
		Color w2 = HexMetrics.TerraceLerp(beginWeights, leftWeights, 1);

		hexMesh.AddTriangleUnperturbed(HexMetrics.Perturb(begin), v2, boundary);
		hexMesh.AddTriangleCellData(indices, beginWeights, w2, boundaryWeights);

		for (int i = 2; i < HexMetrics.terraceSteps; i++)
		{
			Vector3 v1 = v2;
			Color w1 = w2;
			v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, i));
			w2 = HexMetrics.TerraceLerp(beginWeights, leftWeights, i);
			hexMesh.AddTriangleUnperturbed(v1, v2, boundary);
			hexMesh.AddTriangleCellData(indices, w1, w2, boundaryWeights);
		}

		hexMesh.AddTriangleUnperturbed(v2, HexMetrics.Perturb(left), boundary);
		hexMesh.AddTriangleCellData(indices, w2, leftWeights, boundaryWeights);
	}
}
