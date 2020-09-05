using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexConnection
{
    EdgeVertices edge;
    EdgeVertices neighborEdge;
    HexFeature hexFeature;

    public HexConnection(HexFeature hexFeature, EdgeVertices thisEdge, EdgeVertices neighborEdge)
    {
        this.hexFeature = hexFeature;
        this.edge = thisEdge;
        this.neighborEdge = neighborEdge;
    }

    public void Triangulate()
    {
        HexMesh hexMesh = hexFeature.cell.chunk.terrain;
        hexMesh.AddTriangle(edge.v1, neighborEdge.v5, edge.v5);
        hexMesh.AddTriangle(edge.v5, neighborEdge.v5, neighborEdge.v1);

        Vector3 indices;
        indices.x = indices.y = indices.z = hexFeature.cell.Index;
        hexMesh.AddTriangleCellData(indices, Color.red);
        hexMesh.AddTriangleCellData(indices, Color.red);
    }
}
