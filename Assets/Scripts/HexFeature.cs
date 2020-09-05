using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class HexFeature
{
	public List<HexQuadrant> quadrants = new List<HexQuadrant>();
	public List<HexConnection> connections = new List<HexConnection>();
	public HexCell cell;

	public HexFeature(HexCell cell, Vector3 center)
	{
		this.cell = cell;
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
		{
			quadrants.Add(new HexQuadrant(this, d));
			if (d == HexDirection.NE)
            {
				if (cell.HasNeighbor(d))
				{

				}
            }
        }
	}

	public void Triangulate()
    {
		HexMesh terrain = cell.chunk.terrain;
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
		{
			quadrants[(int)d].Triangulate();
			if (d <= HexDirection.SE)
			{
				if ((int)d < connections.Count)
				{
					connections[(int)d].Triangulate();
				}
			}
		}
	}
}