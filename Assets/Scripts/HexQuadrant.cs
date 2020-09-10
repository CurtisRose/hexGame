using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexQuadrant
{
    static Color weights1 = new Color(1f, 0f, 0f);
    static Color weights2 = new Color(0f, 1f, 0f);
    static Color weights3 = new Color(0f, 0f, 1f);

    public static void Triangulate(HexDirection direction, int cellIndex, Vector3 center, HexMesh mesh)
    {
        EdgeVertices edgeVertice = new EdgeVertices(
            center + HexMetrics.GetFirstSolidCorner(direction),
            center + HexMetrics.GetSecondSolidCorner(direction)
        );
        HexQuadrant.TriangulateQuadrant(cellIndex, center, edgeVertice, mesh);
    }

    static void TriangulateQuadrant(
        int index, Vector3 center, EdgeVertices e, HexMesh mesh
    )
    {
        TriangulateEdgeFan(center, e, index, mesh);
    }

    static void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, float index, HexMesh mesh)
    {
        mesh.AddTriangle(center, edge.v1, edge.v2);
        mesh.AddTriangle(center, edge.v2, edge.v3);
        mesh.AddTriangle(center, edge.v3, edge.v4);
        mesh.AddTriangle(center, edge.v4, edge.v5);

        Vector3 indices;
        indices.x = indices.y = indices.z = index;
        mesh.AddTriangleCellData(indices, weights1);
        mesh.AddTriangleCellData(indices, weights1);
        mesh.AddTriangleCellData(indices, weights1);
        mesh.AddTriangleCellData(indices, weights1);
    }
}
