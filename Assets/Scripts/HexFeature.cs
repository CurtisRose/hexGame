using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEditor.Experimental.GraphView;

public static class HexFeature
{
	static int numQuadrants = 6;
	static int numConnections = 3;
	static int numCorners = 3;

	public static void Triangulate(HexMesh mesh, HexCell cell)
    {
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
		{
			HexQuadrant.Triangulate(d, cell.Index, cell.Position, mesh);
			if (d <= HexDirection.SE)
			{
				if (cell.HasNeighbor(d))
                {
					HexConnection.Triangulate(d, cell);
					HexCorners.Triangulate(d, cell);
				}
			}
		}
	}
}