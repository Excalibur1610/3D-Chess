using UnityEngine;

public class SpaceScript : MonoBehaviour {
    private ChessManager main;
    private bool pawnAtEnd;
    private Vector3 Target;
    private Rigidbody pieceRB;

    void Start()
    {
        main = GameObject.Find("Main Camera").GetComponent<ChessManager>();
        pawnAtEnd = false;
        Target = new Vector3(-100f, -100f, -100f);
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && main.SpaceSelectable)
        {
            bool spaceFound = false;
            int i = 0, j = 0;
            for (i = 0; i < 8 && !spaceFound; i++)
            {
                for (j = 0; j < 8 && !spaceFound; j++)
                {
                    if (main.Spaces[i, j].Equals(gameObject))
                        spaceFound = true;
                }
            }
            if (main.P1Turn)
                main.Player1.Move(main.selectedPiece, new int[] { --i, --j });
            else
                main.Player2.Move(main.selectedPiece, new int[] { --i, --j });
            Target = gameObject.transform.position;
            if (main.P1Turn)
            {
                if (main.Player1.GetPieceType(main.Player1.GetPiecePosition(main.selectedPiece)).Equals("PAWN"))
                {
                    if (Target.z == 3.5f)
                        pawnAtEnd = true;
                }
            }
            else
            {
                if (main.Player2.GetPieceType(main.Player2.GetPiecePosition(main.selectedPiece)).Equals("PAWN"))
                {
                    if (Target.z == -3.5f)
                        pawnAtEnd = true;
                }
            }
            main.pieceSelectable = false;
            Target.y = main.selectedPiece.transform.position.y;
            pieceRB = main.selectedPiece.GetComponent<Rigidbody>();
            main.CheckPieceTake(Target);
            MeshRenderer renderer = main.selectedPiece.transform.GetChild(0).GetComponent<MeshRenderer>();
            renderer.material = main.previousColor;
            main.Move(pieceRB, main.selectedPiece, Target);
            main.previousSelection = null;
        }
    }

    void Update()
    {
        if (Target != new Vector3(-100f, -100f, -100f))
        {
            if (Mathf.Abs(main.selectedPiece.transform.position.x - Target.x) < .2f && Mathf.Abs(main.selectedPiece.transform.position.z - Target.z) < .2f)
            {
                pieceRB.isKinematic = true;
                pieceRB.velocity = new Vector3(0f, 0f, 0f);
                main.selectedPiece.transform.position = Target;
                if (pawnAtEnd)
                {
                    GameObject newPiece;
                    if (main.P1Turn)
                        newPiece = (GameObject)Instantiate(Resources.Load("P1Queen"));
                    else
                        newPiece = (GameObject)Instantiate(Resources.Load("P2Queen"));
                    newPiece.transform.position = new Vector3(main.selectedPiece.transform.position.x, newPiece.transform.position.y, main.selectedPiece.transform.position.z);
                    if (main.P1Turn)
                    {
                        main.Player1.RemovePiece(main.selectedPiece);
                        main.Player1.AddPiece(newPiece, "queen", new int[] { (int)(Target.z + 3.5f), (int)(Target.x + 3.5f) });
                    }
                    else
                    {
                        main.Player2.RemovePiece(main.selectedPiece);
                        main.Player2.AddPiece(newPiece, "queen", new int[] { (int)(Target.z + 3.5f), (int)(Target.x + 3.5f) });
                    }
                    Destroy(main.selectedPiece);
                    pawnAtEnd = false;
                }
                main.selectedPiece = null;
                main.SpaceSelectable = false;
                main.P1Turn = !main.P1Turn;
                main.pieceSelectable = true;
                main.SetCamera();
                Target = new Vector3(-100f, -100f, -100f);
            }
        }
    }
}
