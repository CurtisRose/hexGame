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
        if (generateMap)
        {
            generateMap = false;
            CmdCommand();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            generateMap = true;
            CmdSetSeed();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            CmdSetSeed();
        }
    }

    [Command]
    void CmdSetSeed()
    {
        mapGenerator.RpcSetSeed();
    }

    [Command]
    void CmdCommand()
    {
        if (mapGenerator == null)
        {
            mapGenerator = GameObject.FindGameObjectWithTag("HexMapGenerator").GetComponent<HexMapGenerator>();
        }
        if (mapGenerator != null)
        {
            mapGenerator.RpcGenerateMap(40, 30, true);
        }

        /*
        if (hexMapCamera == null)
        {
            if (hexMapCamera != null)
            {
                hexMapCamera.GetHexCamera().enabled = true;
                hexMapCamera.GetHexCamera().tag = "MainCamera";
                hexMapCamera.GetAudioListener().enabled = true;
            }
        }
        if (hexMapCamera != null)
        {
            HexMapCamera.ValidatePosition();
        }*/
    }
}
