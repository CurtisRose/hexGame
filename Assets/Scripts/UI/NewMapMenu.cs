using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Networking;

public class NewMapMenu : NetworkBehaviour {
	[SerializeField]
	HexPlayer hexPlayer;

	HexGrid hexGrid;

	HexMapGenerator mapGenerator;

	bool generateMaps = true;

	bool wrapping = true;

	private void Start()
	{
		if (GameObject.FindGameObjectWithTag("HexGrid") != null)
		{
			hexGrid = GameObject.FindGameObjectWithTag("HexGrid").GetComponent<HexGrid>();
		}
		if (GameObject.FindGameObjectWithTag("HexMapGenerator") != null)
		{
			mapGenerator = GameObject.FindGameObjectWithTag("HexMapGenerator").GetComponent<HexMapGenerator>();
		}
	}

	public void ToggleMapGeneration (bool toggle) {
		generateMaps = toggle;
	}

	public void ToggleWrapping (bool toggle) {
		wrapping = toggle;
	}

	public void Open () {
		gameObject.SetActive(true);
		HexMapCamera.Locked = true;
	}

	public void Close () {
		gameObject.SetActive(false);
		HexMapCamera.Locked = false;
	}

	public void CreateSmallMap () {
		CreateMap(20, 15);
	}

	public void CreateMediumMap () {
		CreateMap(40, 30);
	}

	public void CreateLargeMap () {
		CreateMap(80, 60);
	}

	void CreateMap (int x, int z) {
		if (generateMaps) {
			hexPlayer.CmdGenerateMap(30, 40);
		}
		else {
			hexGrid.CreateMap(x, z, wrapping);
		}
		HexMapCamera.ValidatePosition();
		Close();
	}
}