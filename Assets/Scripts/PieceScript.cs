using UnityEngine;
using System.Collections.Generic;

public class PieceScript : MonoBehaviour {
    private ChessManager main;

    void Start()
    {
        main = GameObject.Find("Main Camera").GetComponent<ChessManager>();
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (main.pieceSelectable)
            {
                if (main.P1Turn)
                {
                    if (gameObject.name.Contains("P1"))
                    {
                        main.selectedPiece = gameObject;
                    }
                }
                else
                {
                    if (gameObject.name.Contains("P2"))
                    {
                        main.selectedPiece = gameObject;
                    }
                }
                if (main.previousSelection != null)
                {
                    MeshRenderer renderer = main.previousSelection.transform.GetChild(0).GetComponent<MeshRenderer>();
                    renderer.material = main.previousColor;
                }
                if (main.selectedPiece != null)
                {
                    Material highlight = (Material)Instantiate(Resources.Load("Highlight"));
                    MeshRenderer renderer = main.selectedPiece.transform.GetChild(0).GetComponent<MeshRenderer>();
                    main.previousColor = renderer.material;
                    renderer.material = highlight;
                    main.previousSelection = main.selectedPiece;
                    main.SpaceSelectable = true;
                }
                else
                    main.SpaceSelectable = false;
            }
        }
    }
}
