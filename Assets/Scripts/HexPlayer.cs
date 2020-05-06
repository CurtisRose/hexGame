using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

public class HexPlayer : NetworkBehaviour
{
    HexMapCamera hexMapCamera;
    HexMapGenerator mapGenerator;

    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            //CmdSetSeed();
        }
    }

    public override void OnStartLocalPlayer()
    {
        hexMapCamera = GetComponentInChildren<HexMapCamera>();
        if (hexMapCamera != null) {
            hexMapCamera.GetHexCamera().enabled = true;
            hexMapCamera.GetHexCamera().tag = "MainCamera";
            hexMapCamera.GetAudioListener().enabled = true;
            HexMapCamera.ValidatePosition();
        }
        
        if (GameObject.FindGameObjectWithTag("HexMapGenerator") != null)
        {
            mapGenerator = GameObject.FindGameObjectWithTag("HexMapGenerator").GetComponent<HexMapGenerator>();
        }
    }

    bool generateMap = false;
    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            int seed = Random.Range(0, int.MaxValue);
            seed ^= (int)System.DateTime.Now.Ticks;
            seed ^= (int)Time.unscaledTime;
            seed &= int.MaxValue;
            CmdGenerateMap(seed);
        }
    }

    [Command]
    void CmdGenerateMap(int seed)
    {
        if (mapGenerator == null)
        {
            mapGenerator = GameObject.FindGameObjectWithTag("HexMapGenerator").GetComponent<HexMapGenerator>();
        }
        if (mapGenerator != null)
        {
            mapGenerator.RpcSetSeed(seed);
            mapGenerator.RpcGenerateMap(40, 30, true);
        }
    }
}
