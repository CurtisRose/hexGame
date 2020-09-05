using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEditor.Experimental.GraphView;

public class HexFeature
{
	public HexQuadrant[] quadrants = new HexQuadrant[6];
	public HexConnection[] connections = new HexConnection[3];
	public HexCorners[] corners = new HexCorners[3];
	public HexCell cell;

	public HexFeature(HexCell cell)
	{
		this.cell = cell;
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
		{
			quadrants[(int)d] = (new HexQuadrant(this, d));
        }
	}

	public void AddConnections()
    {
		for (HexDirection d = HexDirection.NE; d <= HexDirection.SE; d++)
		{
			if (cell.GetNeighbor(d) != null)
			{
				connections[(int)d] = new HexConnection(this, d);
			}
		}
		AddCorners();
	}

	public void AddCorners()
    {
		for (HexDirection d = HexDirection.NE; d <= HexDirection.SE; d++)
		{
			if (cell.GetNeighbor(d) != null)
			{
				corners[(int)d] = new HexCorners(this, d, d.Previous());
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
				if ((int)d < connections.Length)
				{
					if (connections[(int)d] != null)
					{
						connections[(int)d].Triangulate();
					}
				}
				if (corners[(int)d] != null)
				{
					corners[(int)d].Triangulate();
				}
			}
		}
	}
}