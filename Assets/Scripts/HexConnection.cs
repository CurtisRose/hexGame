using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexConnection
{
    static Color weights1 = new Color(1f, 0f, 0f);
    static Color weights2 = new Color(0f, 1f, 0f);
    static Color weights3 = new Color(0f, 0f, 1f);

    public static void Triangulate(HexDirection direction, HexCell cell)
    {
        EdgeVertices edge = new EdgeVertices(
            cell.Position + HexMetrics.GetFirstSolidCorner(direction),
            cell.Position + HexMetrics.GetSecondSolidCorner(direction)
        );
        HexCell neighbor = cell.GetNeighbor(direction);
        Vector3 bridge = HexMetrics.GetBridge(direction);
        bridge.y = neighbor.Position.y - cell.Position.y;
        EdgeVertices neighborEdge = new EdgeVertices(
            edge.v1 + bridge,
            edge.v5 + bridge
        );

        if (cell.GetEdgeType(direction) == HexEdgeType.Slope)
        {
            TriangulateEdgeTerraces(edge, cell, neighborEdge, neighbor, cell.chunk.terrain, false);
        }
        else
        {
            TriangulateEdgeStrip(
                edge, weights1, cell.Index,
                neighborEdge, weights2, neighbor.Index, cell.chunk.terrain, false
            );
        }
    }

    static void TriangulateEdgeStrip(
        EdgeVertices e1, Color w1, float index1,
        EdgeVertices e2, Color w2, float index2,
        HexMesh hexMesh, bool hasRoad = false
    )
    {
        hexMesh.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        hexMesh.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        hexMesh.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        hexMesh.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);

        Vector3 indices;
        indices.x = indices.z = index1;
        indices.y = index2;
        hexMesh.AddQuadCellData(indices, w1, w2);
        hexMesh.AddQuadCellData(indices, w1, w2);
        hexMesh.AddQuadCellData(indices, w1, w2);
        hexMesh.AddQuadCellData(indices, w1, w2);
    }

    static void TriangulateEdgeTerraces(
        EdgeVertices begin, HexCell beginCell,
        EdgeVertices end, HexCell endCell,
        HexMesh hexMesh, bool hasRoad
    )
    {
        EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
        Color w2 = HexMetrics.TerraceLerp(weights1, weights2, 1);
        float i1 = beginCell.Index;
        float i2 = endCell.Index;

        TriangulateEdgeStrip(begin, weights1, i1, e2, w2, i2, hexMesh, hasRoad);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            EdgeVertices e1 = e2;
            Color w1 = w2;
            e2 = EdgeVertices.TerraceLerp(begin, end, i);
            w2 = HexMetrics.TerraceLerp(weights1, weights2, i);
            TriangulateEdgeStrip(e1, w1, i1, e2, w2, i2, hexMesh, hasRoad);
        }

        TriangulateEdgeStrip(e2, w2, i1, end, weights2, i2, hexMesh, hasRoad);
    }
}
