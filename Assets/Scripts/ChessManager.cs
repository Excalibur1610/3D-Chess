using UnityEngine;
using System.Collections.Generic;

public class ChessManager : MonoBehaviour {
    private GameObject[,] Spaces;   //tracks the game board spaces
    private PlayerClass Player1, Player2;
    private bool P1Turn;    //tracks the turns
    private bool P1InCheck, P2InCheck;  //checks if someone is in or will be in check
    private bool P1Checkmate, P2Checkmate;  //checks for win. like above bools, P1.. = true is bad for P1 & P2.. = true is bad for P2
    private int P1Wins = 0, P2Wins = 0;
    private float ApplicationX, ApplicationY;   //window dimensions of game

	// Use this for initialization
	void Start () {
        Clear();
        Spaces = new GameObject[8,8];
        Player1 = new PlayerClass();
        Player2 = new PlayerClass();
        GenerateBoard();
        P1Turn = true;
        SetCamera();
        ApplicationX = Screen.width;
        ApplicationY = Screen.height;
	}
	
	// Update is called once per frame
	void Update () {
        ManagePerspective();
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(ApplicationX - 300, 0, 300, 20), "Player1: " + P1Wins + "\tPlayer2: " + P2Wins);
        if (GUI.Button(new Rect(ApplicationX - 60, ApplicationY - 30, 60, 30), "Quit"))
            Application.Quit();
        if (!P1Checkmate && !P2Checkmate)
        {
            List<Color> originalColors = new List<Color>();
            List<int[]> highlightedSpaces = new List<int[]>();
            GameObject SelectedPiece = SelectPiece();
            if (SelectedPiece != null)
            {
                highlightedSpaces = GetAvailableMoves(SelectedPiece);
                originalColors = HighlightSpaces(highlightedSpaces);
                SelectSpace(SelectedPiece);
                UndoHighlights(highlightedSpaces, originalColors);
                P1Turn = !P1Turn;
            }
        }
        else
        {
            if (P1Checkmate)
            {
                GUI.Label(new Rect(ApplicationX/2 - 30, ApplicationY - 10, 60, 20), "Player 2 wins!!!!");
                P2Wins++;
            }
            else
            {
                GUI.Label(new Rect(ApplicationX / 2 - 30, ApplicationY - 10, 60, 20), "Player 1 wins!!!!");
                P1Wins++;
            }
            if (GUI.Button(new Rect(ApplicationX / 2 - 80, ApplicationY + 15, 160, 30), "New Game"))
                Start();
        }
    }

    void ManagePerspective() {
        //move camera around
        while (Input.GetMouseButtonDown(1))
        {
            float deltaX, deltaZ;
            if (transform.position.x < 4.5f && transform.position.x > -4.5f)
                deltaX = Input.GetAxis("Mouse X") * 10f;
            else
                deltaX = 0f;
            if (transform.position.z < 4.5f && transform.position.z > -4.5f)
                deltaZ = Input.GetAxis("Mouse Y") * 10f;
            else
                deltaZ = 0f;
            transform.Translate(deltaX, 0f, deltaZ);
        }
        //rotate view of game board
        while (Input.GetMouseButtonDown(2))
        {
            float deltaY, deltaZ;
            if (transform.position.y > 0f)
                deltaY = Input.GetAxis("Mouse X") * 10f;
            else
                deltaY = 0f;
            if (transform.position.z > 0f)
                deltaZ = Input.GetAxis("Mouse Y") * 10f;
            else
                deltaZ = 0f;
            transform.RotateAround(new Vector3(transform.position.x, 0f, 0f), new Vector3(0, deltaY, deltaZ), 10f);
        }
        //zoom in/out
        float zoom = Input.GetAxis("Mouse ScrollWheel") * 10f;
        Camera.main.fieldOfView = Mathf.Clamp(zoom, 25f, 80f);
        //track window size
        ApplicationX = Screen.width;
        ApplicationY = Screen.height;
    }

    void GenerateBoard () {
        //sets board
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (i%2 == j%2)
                    Spaces[i, j] = (GameObject)Instantiate(Resources.Load("BrownSpace"));
                else
                    Spaces[i, j] = (GameObject)Instantiate(Resources.Load("TanSpace"));
                Spaces[i, j].transform.position = new Vector3(
                    Spaces[i, j].transform.position.x + j, Spaces[i, j].transform.position.y, Spaces[i, j].transform.position.z + i
                    );
            }
        }
        //sets Players pieces
        GameObject newPiece;
        for (int i = 0; i < 2; i++) //Player1
        {
            for (int j = 0; j < 8; j++)
            {
                if (i == 0)
                {
                    switch (j)
                    {
                        case 0:
                            Player1.AddPiece((GameObject)Instantiate(Resources.Load("P1Rook")), "rook");
                            break;
                        case 1:
                            Player1.AddPiece((GameObject)Instantiate(Resources.Load("P1Knight")), "knight");
                            break;
                        case 2:
                            Player1.AddPiece((GameObject)Instantiate(Resources.Load("P1Bishop")), "bishop");
                            break;
                        case 3:
                            Player1.AddPiece((GameObject)Instantiate(Resources.Load("P1Queen")), "queen");
                            break;
                        case 4:
                            Player1.AddPiece((GameObject)Instantiate(Resources.Load("P1King")), "king");
                            break;
                        case 5:
                            newPiece = (GameObject)Instantiate(Resources.Load("P1Bishop"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player1.AddPiece(newPiece, "bishop");
                            break;
                        case 6:
                            newPiece = (GameObject)Instantiate(Resources.Load("P1Knight"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player1.AddPiece(newPiece, "knight");
                            break;
                        case 7:
                            newPiece = (GameObject)Instantiate(Resources.Load("P1Rook"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player1.AddPiece(newPiece, "rook");
                            break;
                    }
                }
                else
                {
                    newPiece = (GameObject)Instantiate(Resources.Load("P1Pawn"));
                    newPiece.transform.position = new Vector3(
                        newPiece.transform.position.x + i, newPiece.transform.position.y, newPiece.transform.position.z
                        );
                    Player1.AddPiece(newPiece, "pawn");
                }
            }
        }

        for (int i = 6; i < 8; i++) //Player2
        {
            for (int j = 0; j < 8; j++)
            {
                if (i == 7)
                {
                    switch (j)
                    {
                        case 0:
                            Player2.AddPiece((GameObject)Instantiate(Resources.Load("P2Rook")), "rook");
                            break;
                        case 1:
                            Player2.AddPiece((GameObject)Instantiate(Resources.Load("P2Knight")), "knight");
                            break;
                        case 2:
                            Player2.AddPiece((GameObject)Instantiate(Resources.Load("P2Bishop")), "bishop");
                            break;
                        case 3:
                            Player2.AddPiece((GameObject)Instantiate(Resources.Load("P2Queen")), "queen");
                            break;
                        case 4:
                            Player2.AddPiece((GameObject)Instantiate(Resources.Load("P2King")), "king");
                            break;
                        case 5:
                            newPiece = (GameObject)Instantiate(Resources.Load("P2Bishop"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player2.AddPiece(newPiece, "bishop");
                            break;
                        case 6:
                            newPiece = (GameObject)Instantiate(Resources.Load("P2Knight"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player2.AddPiece(newPiece, "knight");
                            break;
                        case 7:
                            newPiece = (GameObject)Instantiate(Resources.Load("P2Rook"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player2.AddPiece(newPiece, "rook");
                            break;
                    }
                }
                else
                {
                    newPiece = (GameObject)Instantiate(Resources.Load("P2Pawn"));
                    newPiece.transform.position = new Vector3(
                        newPiece.transform.position.x + i, newPiece.transform.position.y, newPiece.transform.position.z
                        );
                    Player2.AddPiece(newPiece, "pawn");
                }
            }
        }
    }

    void SetCamera () {
        if (P1Turn)
        {
            transform.position = new Vector3(0f, 5f, -7f);
            transform.eulerAngles = new Vector3(40f, 0f, 0f);
        }
        else
        {
            transform.position = new Vector3(0f, 5f, 7f);
            transform.eulerAngles = new Vector3(40f, 180f, 0f);
        }
        Camera.main.fieldOfView = 60f;
    }

    List<int[]> GetAvailableMoves (GameObject SelectedPiece) {
        List<int[]> InvalidSpaces;  //spaces blocked by pieces
        List<int[]> ValidSpaces;  //spaces to which SelectedPiece can move
        int[] Position = new int[] { (int)(SelectedPiece.transform.position.x + 3.5f), (int)(SelectedPiece.transform.position.z + 3.5f) };
        if (P1Turn) //Player1
        {
            InvalidSpaces = new List<int[]>(Player1.GetPiecePositions());
            ValidSpaces = new List<int[]>(Player1.AvailableMoves(Position));
            int i = 0;
            while (i < ValidSpaces.Count)   //remove spaces occupied by player's pieces first
            {
                int[] space = ValidSpaces.ToArray()[i];
                bool removedOne = false;
                foreach (int[] occSpace in InvalidSpaces)
                {
                    if (space == occSpace)
                    {
                        ValidSpaces.Remove(space);
                        removedOne = true;
                        break;
                    }
                }
                if (!removedOne)
                    i++;
            }
            if (!Player1.GetPieceType(Position).Equals("KNIGHT"))   //only knights can jump, so check for obstructions in move pattern
            {
                if (!Player1.GetPieceType(Position).Equals("PAWN"))
                    foreach (int[] PlayerPiece in Player2.GetPiecePositions())
                        InvalidSpaces.Add(PlayerPiece);
                if (Player1.GetPieceType(Position).Equals("QUEEN") || Player1.GetPieceType(Position).Equals("ROOK"))
                {
                    i = 0;
                    while (i < ValidSpaces.Count)
                    {
                        int[] space = ValidSpaces.ToArray()[i];
                        bool removedOne = false;
                        bool equalX, equalZ;    //neither have to be true, but never both true
                        foreach (int[] occSpace in InvalidSpaces)
                        {
                            equalX = (occSpace[0] == Position[0]);
                            equalZ = (occSpace[1] == Position[1]);
                            if (equalX && space[0] == Position[0])
                            {
                                if (occSpace[1] > Position[1])
                                {
                                    if (space[1] > occSpace[1])
                                    {
                                        ValidSpaces.Remove(space);
                                        removedOne = true;
                                        break;
                                    }
                                }
                                else if (space[1] < occSpace[1])
                                {
                                    ValidSpaces.Remove(space);
                                    removedOne = true;
                                    break;
                                }
                            }
                            else if (equalZ && space[1] == Position[1])
                            {
                                if (occSpace[0] > Position[0])
                                {
                                    if (space[0] > occSpace[0])
                                    {
                                        ValidSpaces.Remove(space);
                                        removedOne = true;
                                        break;
                                    }
                                }
                                else if (space[0] < occSpace[0])
                                {
                                    ValidSpaces.Remove(space);
                                    removedOne = true;
                                    break;
                                }
                            }
                        }
                        if (!removedOne)
                            i++;
                    }
                }
                if (Player1.GetPieceType(Position).Equals("QUEEN") || Player1.GetPieceType(Position).Equals("Bishop"))
                {
                    i = 0;
                    while (i < ValidSpaces.Count)
                    {
                        int[] space = ValidSpaces.ToArray()[i];
                        bool removedOne = false;
                        bool inDiagonal, sameDiagonal, Left, Up;
                        Left = ((space[0] - Position[0]) < 0);
                        Up = ((space[1] - Position[1]) > 0);
                        foreach (int[] occSpace in InvalidSpaces)
                        {
                            inDiagonal = (Mathf.Abs(occSpace[0] - Position[0]) == Mathf.Abs(occSpace[1] - Position[1]));
                            if (inDiagonal)
                            {
                                sameDiagonal = (((space[0] - Position[0]) > 0 && (occSpace[0] - Position[0]) > 0) && 
                                    ((space[1] - Position[1]) > 0 && (occSpace[1] - Position[1]) > 0));
                                sameDiagonal = sameDiagonal || (((space[0] - Position[0]) > 0 && (occSpace[0] - Position[0]) > 0) &&
                                    ((space[1] - Position[1]) < 0 && (occSpace[1] - Position[1]) < 0));
                                sameDiagonal = sameDiagonal || (((space[0] - Position[0]) < 0 && (occSpace[0] - Position[0]) < 0) &&
                                    ((space[1] - Position[1]) < 0 && (occSpace[1] - Position[1]) < 0));
                                sameDiagonal = sameDiagonal || (((space[0] - Position[0]) < 0 && (occSpace[0] - Position[0]) < 0) &&
                                    ((space[1] - Position[1]) > 0 && (occSpace[1] - Position[1]) > 0));
                                if (sameDiagonal)
                                {
                                    if (Left)
                                    {
                                        if (space[0] < occSpace[0])
                                        {
                                            if (Up)
                                            {
                                                if (space[1] > occSpace[1])
                                                {
                                                    ValidSpaces.Remove(space);
                                                    removedOne = true;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (space[1] < occSpace[1])
                                                {
                                                    ValidSpaces.Remove(space);
                                                    removedOne = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (space[0] > occSpace[0])
                                        {
                                            if (Up)
                                            {
                                                if (space[1] > occSpace[1])
                                                {
                                                    ValidSpaces.Remove(space);
                                                    removedOne = true;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (space[1] < occSpace[1])
                                                {
                                                    ValidSpaces.Remove(space);
                                                    removedOne = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (!removedOne)
                            i++;
                    }
                }
                if (Player1.GetPieceType(Position).Equals("KING"))  //SPECIAL CASE: king cannot castle when in check
                {
                    if (P1InCheck)
                    {
                        i = 0;
                        while (i < ValidSpaces.Count)
                        {
                            int[] space = ValidSpaces.ToArray()[i];
                            bool removedOne = false;
                            if (space[1] == Position[1])
                            {
                                if (space[0] == Position[0] + 2 || space[0] == Position[0] - 2)
                                {
                                    ValidSpaces.Remove(space);
                                    removedOne = true;
                                    break;
                                }
                            }
                            if (!removedOne)
                                i++;
                        }
                    }
                }
                if (Player1.GetPieceType(Position).Equals("PAWN"))  //SPECIAL CASES: pawns attack diagonally, pawns can move two instead of one on first move.  OPT SPECIAL CASE: en passant rule
                {
                    InvalidSpaces.Clear();
                    foreach (int[] PlayerPiece in Player2.GetPiecePositions())
                        InvalidSpaces.Add(PlayerPiece);
                    if (ValidSpaces.Count > 1)  //pawn hasn't moved yet
                    {
                        foreach (int[] occSpace in InvalidSpaces)
                        {
                            if (occSpace[0] == Position[0])
                            {
                                if (occSpace[1] == Position[1] + 1)
                                {
                                    ValidSpaces.Clear();
                                    break;
                                }
                                if (occSpace[1] == Position[1] + 2)
                                    ValidSpaces.Remove(new int[] { Position[0], Position[1] + 2 });
                            }
                        }
                    }
                    foreach (int[] occSpace in InvalidSpaces)   //diagonal attack opportunity
                    {
                        if (occSpace[1] == Position[1] + 1)
                        {
                            if (occSpace[0] == Position[0] + 1 || occSpace[0] == Position[0] - 1)
                                ValidSpaces.Add(occSpace);
                        }
                    }
                }
            }
            i = 0;
            while (i < ValidSpaces.Count)   //final check: can't leave self in check.
            {
                int[] space = ValidSpaces.ToArray()[i];
                bool removedOne = false;
                if (CheckCheck(SelectedPiece, space))
                {
                    ValidSpaces.Remove(space);
                    removedOne = true;
                    break;
                }
                if (!removedOne)
                    i++;
            }
        }
        else    //Player2
        {
            InvalidSpaces = new List<int[]>(Player2.GetPiecePositions());
            ValidSpaces = new List<int[]>(Player2.AvailableMoves(Position));
            int i = 0;
            while (i < ValidSpaces.Count)   //remove spaces occupied by player's pieces first
            {
                int[] space = ValidSpaces.ToArray()[i];
                bool removedOne = false;
                foreach (int[] occSpace in InvalidSpaces)
                {
                    if (space == occSpace)
                    {
                        ValidSpaces.Remove(space);
                        removedOne = true;
                        break;
                    }
                }
                if (!removedOne)
                    i++;
            }
            if (!Player2.GetPieceType(Position).Equals("KNIGHT"))   //only knights can jump, so check for obstructions in move pattern
            {
                if (!Player2.GetPieceType(Position).Equals("PAWN"))
                    foreach (int[] PlayerPiece in Player2.GetPiecePositions())
                        InvalidSpaces.Add(PlayerPiece);
                if (Player2.GetPieceType(Position).Equals("QUEEN") || Player2.GetPieceType(Position).Equals("ROOK"))
                {
                    i = 0;
                    while (i < ValidSpaces.Count)
                    {
                        int[] space = ValidSpaces.ToArray()[i];
                        bool removedOne = false;
                        bool equalX, equalZ;    //neither have to be true, but never both true
                        foreach (int[] occSpace in InvalidSpaces)
                        {
                            equalX = (occSpace[0] == Position[0]);
                            equalZ = (occSpace[1] == Position[1]);
                            if (equalX && space[0] == Position[0])
                            {
                                if (occSpace[1] > Position[1])
                                {
                                    if (space[1] > occSpace[1])
                                    {
                                        ValidSpaces.Remove(space);
                                        removedOne = true;
                                        break;
                                    }
                                }
                                else if (space[1] < occSpace[1])
                                {
                                    ValidSpaces.Remove(space);
                                    removedOne = true;
                                    break;
                                }
                            }
                            else if (equalZ && space[1] == Position[1])
                            {
                                if (occSpace[0] > Position[0])
                                {
                                    if (space[0] > occSpace[0])
                                    {
                                        ValidSpaces.Remove(space);
                                        removedOne = true;
                                        break;
                                    }
                                }
                                else if (space[0] < occSpace[0])
                                {
                                    ValidSpaces.Remove(space);
                                    removedOne = true;
                                    break;
                                }
                            }
                        }
                        if (!removedOne)
                            i++;
                    }
                }
                if (Player2.GetPieceType(Position).Equals("QUEEN") || Player2.GetPieceType(Position).Equals("Bishop"))
                {
                    i = 0;
                    while (i < ValidSpaces.Count)
                    {
                        int[] space = ValidSpaces.ToArray()[i];
                        bool removedOne = false;
                        bool inDiagonal, sameDiagonal, Left, Up;
                        Left = ((space[0] - Position[0]) < 0);
                        Up = ((space[1] - Position[1]) > 0);
                        foreach (int[] occSpace in InvalidSpaces)
                        {
                            inDiagonal = (Mathf.Abs(occSpace[0] - Position[0]) == Mathf.Abs(occSpace[1] - Position[1]));
                            if (inDiagonal)
                            {
                                sameDiagonal = (((space[0] - Position[0]) > 0 && (occSpace[0] - Position[0]) > 0) &&
                                    ((space[1] - Position[1]) > 0 && (occSpace[1] - Position[1]) > 0));
                                sameDiagonal = sameDiagonal || (((space[0] - Position[0]) > 0 && (occSpace[0] - Position[0]) > 0) &&
                                    ((space[1] - Position[1]) < 0 && (occSpace[1] - Position[1]) < 0));
                                sameDiagonal = sameDiagonal || (((space[0] - Position[0]) < 0 && (occSpace[0] - Position[0]) < 0) &&
                                    ((space[1] - Position[1]) < 0 && (occSpace[1] - Position[1]) < 0));
                                sameDiagonal = sameDiagonal || (((space[0] - Position[0]) < 0 && (occSpace[0] - Position[0]) < 0) &&
                                    ((space[1] - Position[1]) > 0 && (occSpace[1] - Position[1]) > 0));
                                if (sameDiagonal)
                                {
                                    if (Left)
                                    {
                                        if (space[0] < occSpace[0])
                                        {
                                            if (Up)
                                            {
                                                if (space[1] > occSpace[1])
                                                {
                                                    ValidSpaces.Remove(space);
                                                    removedOne = true;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (space[1] < occSpace[1])
                                                {
                                                    ValidSpaces.Remove(space);
                                                    removedOne = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (space[0] > occSpace[0])
                                        {
                                            if (Up)
                                            {
                                                if (space[1] > occSpace[1])
                                                {
                                                    ValidSpaces.Remove(space);
                                                    removedOne = true;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (space[1] < occSpace[1])
                                                {
                                                    ValidSpaces.Remove(space);
                                                    removedOne = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (!removedOne)
                            i++;
                    }
                }
                if (Player2.GetPieceType(Position).Equals("KING"))  //SPECIAL CASE: king cannot castle when in check
                {
                    if (P1InCheck)
                    {
                        i = 0;
                        while (i < ValidSpaces.Count)
                        {
                            int[] space = ValidSpaces.ToArray()[i];
                            bool removedOne = false;
                            if (space[1] == Position[1])
                            {
                                if (space[0] == Position[0] + 2 || space[0] == Position[0] - 2)
                                {
                                    ValidSpaces.Remove(space);
                                    removedOne = true;
                                    break;
                                }
                            }
                            if (!removedOne)
                                i++;
                        }
                    }
                }
                if (Player2.GetPieceType(Position).Equals("PAWN"))  //SPECIAL CASES: pawns attack diagonally, pawns can move two instead of one on first move.  OPT SPECIAL CASE: en passant rule
                {
                    InvalidSpaces.Clear();
                    foreach (int[] PlayerPiece in Player2.GetPiecePositions())
                        InvalidSpaces.Add(PlayerPiece);
                    if (ValidSpaces.Count > 1)  //pawn hasn't moved yet
                    {
                        foreach (int[] occSpace in InvalidSpaces)
                        {
                            if (occSpace[0] == Position[0])
                            {
                                if (occSpace[1] == Position[1] - 1)
                                {
                                    ValidSpaces.Clear();
                                    break;
                                }
                                if (occSpace[1] == Position[1] - 2)
                                    ValidSpaces.Remove(new int[] { Position[0], Position[1] - 2 });
                            }
                        }
                    }
                    foreach (int[] occSpace in InvalidSpaces)   //diagonal attack opportunity
                    {
                        if (occSpace[1] == Position[1] - 1)
                        {
                            if (occSpace[0] == Position[0] + 1 || occSpace[0] == Position[0] - 1)
                                ValidSpaces.Add(occSpace);
                        }
                    }
                }
            }
            i = 0;
            while (i < ValidSpaces.Count)   //final check: can't leave self in check.
            {
                int[] space = ValidSpaces.ToArray()[i];
                bool removedOne = false;
                if (CheckCheck(SelectedPiece, space))
                {
                    ValidSpaces.Remove(space);
                    removedOne = true;
                    break;
                }
                if (!removedOne)
                    i++;
            }
        }
        return ValidSpaces;
    }

    GameObject SelectPiece()
    {
        if (Input.GetMouseButton(0))
        {
            //start: code gathered from outside source (Unity Answers)
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {   //end: code gathered from outside source
                if (P1Turn)
                {
                    if (hitInfo.transform.gameObject.name.Contains("P1"))
                    {
                        return hitInfo.transform.gameObject;
                    }
                }
                else
                {
                    if (hitInfo.transform.gameObject.name.Contains("P2"))
                    {
                        return hitInfo.transform.gameObject;
                    }
                }
            }
        }
        return null;
    }

    void SelectSpace(GameObject SelectedPiece)
    {
        if (Input.GetMouseButton(0))
        {
            //start: code gathered from outside source (Unity Answers)
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {   //end: code gathered from outside source
                if (hitInfo.transform.gameObject.name.Contains("Space"))
                {
                    MeshRenderer renderer = hitInfo.transform.gameObject.GetComponent<MeshRenderer>();
                    Material highlight = (Material)Instantiate(Resources.Load("Highlight"));
                    if (renderer.material.color == highlight.color)
                    {
                        Vector3 Target = hitInfo.transform.gameObject.transform.position;
                        Target.y = SelectedPiece.transform.position.y;
                        bool Castle = (Mathf.Abs(SelectedPiece.transform.position.x - Target.x) == 2f);
                        Rigidbody pieceRB = SelectedPiece.GetComponent<Rigidbody>();
                        Move(pieceRB, SelectedPiece, Target, Castle);
                        while (Mathf.Abs(SelectedPiece.transform.position.x - Target.x) > 0.1f || Mathf.Abs(SelectedPiece.transform.position.z - Target.z) > 0.1f)
                        {
                        }
                        pieceRB.isKinematic = true;
                        pieceRB.velocity = new Vector3(0f, 0f, 0f);
                    }
                }
            }
        }
    }

    void Move(Rigidbody pieceRB, GameObject Piece, Vector3 Target, bool Castle)
    {
        pieceRB.isKinematic = false;
        pieceRB.AddForce(ForceCalculation(pieceRB, Target, Piece.transform.position), ForceMode.Acceleration);
    }

    Vector3 ForceCalculation(Rigidbody piece, Vector3 Target, Vector3 startPos)
    {
        Vector3 initVelocity = new Vector3(XVelocity(Target, startPos), YVelocity(), ZVelocity(Target, startPos));
        Vector3 force = piece.mass * initVelocity / Time.fixedDeltaTime;
        return force;
    }

    float XVelocity(Vector3 endPos, Vector3 startPos)
    {
        float distance = endPos.x - startPos.x;
        return distance / (80f * Time.deltaTime);
    }

    float YVelocity()
    {
        return Physics.gravity.magnitude * (40f * Time.deltaTime);
    }

    float ZVelocity(Vector3 endPos, Vector3 startPos)
    {
        float distance = endPos.z - startPos.z;
        return distance / (80f * Time.deltaTime);
    }

    void CheckCheck()
    {
        List<int[]> newMoves = new List<int[]>();
        GameObject opposingKing;
        P2InCheck = false;
        P1InCheck = false;
        if (P1Turn) //?Player1 put Player2 in check?
        {
            opposingKing = Player2.GetKing();
            foreach(GameObject piece in Player1.GetPieces())
                foreach(int[] move in GetAvailableMoves(piece))
                    newMoves.Add(move);

            foreach(int[] move in newMoves)
            {
                if (move == new int[] { (int)(opposingKing.transform.position.x + 3.5f), (int)(opposingKing.transform.position.z + 3.5f) })
                {
                    P2InCheck = true;
                    break;
                }
            }
        }
        else    //?Player2 put Player1 in check?
        {
            opposingKing = Player1.GetKing();
            foreach (GameObject piece in Player2.GetPieces())
                foreach (int[] move in GetAvailableMoves(piece))
                    newMoves.Add(move);

            foreach (int[] move in newMoves)
            {
                if (move == new int[] { (int)(opposingKing.transform.position.x + 3.5f), (int)(opposingKing.transform.position.z + 3.5f) })
                {
                    P1InCheck = true;
                    break;
                }
            }
        }
    }

    bool CheckCheck(GameObject movingPiece, int[] targetPosition)
    {
        List<int[]> opposingMoves = new List<int[]>();
        GameObject ownKing;
        if (P1Turn) //?Player1 put self in check?
        {
            ownKing = Player1.GetKing();
            foreach (GameObject piece in Player2.GetPieces())
                foreach (int[] move in GetAvailableMoves(piece))
                    opposingMoves.Add(move);

            foreach (int[] move in opposingMoves)
            {
                if (move == new int[] { (int)(ownKing.transform.position.x + 3.5f), (int)(ownKing.transform.position.z + 3.5f) })
                    return true;
            }
        }
        else    //?Player2 put self in check?
        {
            ownKing = Player2.GetKing();
            foreach (GameObject piece in Player1.GetPieces())
                foreach (int[] move in GetAvailableMoves(piece))
                    opposingMoves.Add(move);

            foreach (int[] move in opposingMoves)
            {
                if (move == new int[] { (int)(ownKing.transform.position.x + 3.5f), (int)(ownKing.transform.position.z + 3.5f) })
                    return true;
            }
        }
        return false;
    }

    List<Color> HighlightSpaces(List<int[]> availMoves)
    {
        Material highlight = (Material)Instantiate(Resources.Load("Highlight"));
        List<Color> originalColors = new List<Color>();
        foreach(int[] space in availMoves)
        {
            MeshRenderer renderer = Spaces[space[0], space[1]].GetComponent<MeshRenderer>();
            originalColors.Add(renderer.material.color);
            renderer.material.color = highlight.color;
        }
        return originalColors;
    }

    void UndoHighlights(List<int[]> highlightedSpaces, List<Color> originalColors)
    {
        for (int i = 0; i < highlightedSpaces.Count; i++)
        {
            int[] space = highlightedSpaces.ToArray()[i];
            MeshRenderer renderer = Spaces[space[0], space[1]].GetComponent<MeshRenderer>();
            renderer.material.color = originalColors.ToArray()[i];
        }
    }

    void CheckmateCheck()
    {
        if (P1InCheck)
        {
            P1Checkmate = true;
            foreach(GameObject piece in Player1.GetPieces())
            {
                if (GetAvailableMoves(piece).Count != 0)
                {
                    P1Checkmate = false;
                    break;
                }
            }
        }
        if (P2InCheck)
        {
            P2Checkmate = true;
            foreach (GameObject piece in Player2.GetPieces())
            {
                if (GetAvailableMoves(piece).Count != 0)
                {
                    P2Checkmate = false;
                    break;
                }
            }
        }
    }

    void Clear()
    {
        if (Spaces != null)
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Destroy(Spaces[i, j]);
                }
            }
        if (Player1 != null)
            foreach (GameObject piece in Player1.GetPieces())
                Destroy(piece);
        if (Player2 != null)
            foreach (GameObject piece in Player2.GetPieces())
                Destroy(piece);
    }
}
