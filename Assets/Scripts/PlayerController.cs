using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class PlayerController : NetworkBehaviour
{
    HexMapCamera hexMapCamera;
    HexMapEditor hexMapEditor;
    bool allowUserInput = true;

    // Start is called before the first frame update
    /*void Start()
    {
        Debug.Log("Start");
        hexMapCamera = GetComponentInChildren<HexMapCamera>();
        hexMapEditor = GameObject.FindGameObjectWithTag("HexMapEditor").GetComponent<HexMapEditor>();
        if (!isLocalPlayer)
        {
            Debug.Log("Not the local player");
            return;
        }
        Debug.Log("IS the local player");
        hexMapCamera.GetCamera().enabled = true;
        hexMapCamera.GetCamera().tag = "MainCamera";
        hexMapCamera.GetAudioListener().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        CameraControl();
        ColorMap();
    }

    private void ColorMap()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButton(0))
            {
                hexMapEditor.HandleInput();
                return;
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                hexMapEditor.CmdCreateUnit(UnitType.Swordsman);
                return;
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                hexMapEditor.CmdCreateUnit(UnitType.Archer);
                return;
            }
        }
        hexMapEditor.SetPreviousCellNull();
    }

    private void CameraControl()
    {
        if (allowUserInput)
        {
            //Debug.Log("TEST");
            float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
            if (zoomDelta != 0f)
            {
                hexMapCamera.AdjustZoom(zoomDelta);
            }

            float rotationDelta = Input.GetAxis("Rotation");
            if (rotationDelta != 0f)
            {
                hexMapCamera.AdjustRotation(rotationDelta);
            }

            float xDelta = Input.GetAxis("Horizontal");
            float zDelta = Input.GetAxis("Vertical");
            if (xDelta != 0f || zDelta != 0f)
            {
                hexMapCamera.AdjustPosition(xDelta, zDelta);
            }
        }
    }*/
}
