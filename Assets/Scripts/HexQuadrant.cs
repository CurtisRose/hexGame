using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexQuadrant
{
    HexFeature hexFeature;
    public EdgeVertices edgeVertice;
    public Vector3 center, v1, v2, v3, v4;

    public HexQuadrant(HexFeature hexFeature, HexDirection direction)
    {
        this.hexFeature = hexFeature;
        this.center = hexFeature.cell.Position;
        v1 = Vector3.Lerp(center, v3, 0.5f);
        v2 = Vector3.Lerp(center, v4, 0.5f);
        v3 = center + HexMetrics.GetFirstSolidCorner(direction);
        v4 = center + HexMetrics.GetSecondSolidCorner(direction);
        edgeVertice = new EdgeVertices(v3, v4);
    }

    public void Triangulate()
    {
        HexMesh hexMesh = hexFeature.cell.chunk.terrain;
        hexMesh.AddTriangle(center, v3, v4);

        Vector3 indices;
        indices.x = indices.y = indices.z = hexFeature.cell.Index;
        hexMesh.AddTriangleCellData(indices, Color.red);
    }
}
