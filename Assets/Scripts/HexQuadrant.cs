using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexQuadrant
{
    HexCell cell;
    HexFeature hexFeature;
    HexDirection direction;

    static Color weights1 = new Color(1f, 0f, 0f);
    static Color weights2 = new Color(0f, 1f, 0f);
    static Color weights3 = new Color(0f, 0f, 1f);

    public HexQuadrant(HexFeature hexFeature, HexDirection direction)
    {
        this.hexFeature = hexFeature;
        this.direction = direction;
        cell = hexFeature.cell;
    }

    public void Triangulate()
    {
        Vector3 center = cell.Position;
        EdgeVertices edgeVertice = new EdgeVertices(
            center + HexMetrics.GetFirstSolidCorner(direction),
            center + HexMetrics.GetSecondSolidCorner(direction)
        );
        TriangulateQuadrant(direction, cell, center, edgeVertice);
    }

    void TriangulateQuadrant(
        HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
    )
    {
        TriangulateEdgeFan(center, e, cell.Index);
    }

    void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, float index)
    {
        HexMesh hexMesh = cell.chunk.terrain;
        hexMesh.AddTriangle(center, edge.v1, edge.v2);
        hexMesh.AddTriangle(center, edge.v2, edge.v3);
        hexMesh.AddTriangle(center, edge.v3, edge.v4);
        hexMesh.AddTriangle(center, edge.v4, edge.v5);

        Vector3 indices;
        indices.x = indices.y = indices.z = index;
        hexMesh.AddTriangleCellData(indices, weights1);
        hexMesh.AddTriangleCellData(indices, weights1);
        hexMesh.AddTriangleCellData(indices, weights1);
        hexMesh.AddTriangleCellData(indices, weights1);
    }
}
