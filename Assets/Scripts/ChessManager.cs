using UnityEngine;
using System.Collections.Generic;

public class ChessManager : MonoBehaviour {
    public GameObject[,] Spaces;   //tracks the game board spaces
    public PlayerClass Player1, Player2;
    public bool P1Turn;    //tracks the turns
    private bool P1InCheck, P2InCheck;  //checks if someone is in or will be in check
    public bool P1Checkmate, P2Checkmate;  //checks for win. like above bools, P1.. = true is bad for P1 & P2.. = true is bad for P2
    private int P1Wins = 0, P2Wins = 0;
    private float ApplicationX, ApplicationY;   //window dimensions of game
    private float zoom, deltaZoom;
    private Vector3 lastPosition = new Vector3(0, 0, 0);    //tracks mouse position for camera pan
    public GameObject previousSelection, selectedPiece;
    public List<Color> originalColors = new List<Color>();
    public List<int[]> highlightedSpaces = new List<int[]>();
    private const float CAMERA_RADIUS = 8.6023252670426267717294735350497f;
    public bool SpaceSelectable;

    // Use this for initialization
    void Start () {
        Spaces = new GameObject[8,8];
        Player1 = new PlayerClass(true);
        Player2 = new PlayerClass(false);
        GenerateBoard();
        P1Turn = true;
        SetCamera();
        zoom = 60f;
        deltaZoom = 0;
        ApplicationX = Screen.width;
        ApplicationY = Screen.height;
        SpaceSelectable = false;
        previousSelection = null;
        Debug.Log("Game Start");
    }
	
    void Update () {
        ManagePerspective();
    }

    void OnGUI() {
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(ApplicationX - 300, 0, 300, 20), "Player1: " + P1Wins + "\tPlayer2: " + P2Wins);
        if (GUI.Button(new Rect(ApplicationX - 60, ApplicationY - 30, 60, 30), "Quit"))
            Application.Quit();
        if (GUI.Button(new Rect(0, ApplicationY - 30, 100, 30), "Reset Camera"))
            SetCamera();
        if (P1Checkmate)
        {
            Clear();
            GUI.Label(new Rect(ApplicationX / 2 - 30, ApplicationY - 10, 60, 20), "Player 2 wins!!!!");
            P2Wins++;
        }
        else if (P2Checkmate)
        {
            Clear();
            GUI.Label(new Rect(ApplicationX / 2 - 30, ApplicationY - 10, 60, 20), "Player 1 wins!!!!");
            P1Wins++;
        }
        if (GUI.Button(new Rect(ApplicationX / 2 - 80, ApplicationY + 15, 160, 30), "New Game"))
            Start();
    }

    void ManagePerspective() {
        //move camera around
        if (Input.GetMouseButtonDown(1))
            lastPosition = Input.mousePosition;
        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastPosition;
            if ((transform.position.x < -4.5f && delta.x > 0) || (transform.position.x > 4.5f && delta.x < 0))
                delta.x = 0;
            if ((transform.position.z < -7.5f && delta.y > 0) || (transform.position.z > 7.5f && delta.y < 0))
                delta.y = 0;
            GameObject.Find("CameraCenter").transform.Translate(new Vector3(-delta.x * 2f, 0, -delta.y * 2f) * Time.deltaTime);
            //transform.position = new Vector3(transform.position.x, cameraY, transform.position.z);
            lastPosition = Input.mousePosition;
        }
        //rotate view of game board
        if (Input.GetMouseButtonDown(2))
            lastPosition = Input.mousePosition;
        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastPosition;
            Vector3 angle = new Vector3(Mathf.PI - (2f * Mathf.Acos(delta.x / (2f * CAMERA_RADIUS))), 
                Mathf.PI - (2f * Mathf.Acos(delta.y / (2f * CAMERA_RADIUS))), 0f) * -300f;
            if (transform.position.y < 1)
            {
                if (transform.position.z < GameObject.Find("CameraCenter").transform.position.z && angle.y < 0)
                {
                    angle.y = 0;
                }
                else if (transform.position.z > GameObject.Find("CameraCenter").transform.position.z && angle.y > 0)
                {
                    angle.y = 0;
                }
            }
            GameObject.Find("CameraCenter").transform.Rotate(new Vector3(angle.y, -angle.x, 0f) * Time.deltaTime);
            lastPosition = Input.mousePosition;
        }
        //zoom in/out
        deltaZoom = -1f * Input.GetAxis("Mouse ScrollWheel") * 20f;
        zoom += deltaZoom;
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
                            Player1.AddPiece((GameObject)Instantiate(Resources.Load("P1Rook")), "rook", new int[] { i, j });
                            break;
                        case 1:
                            Player1.AddPiece((GameObject)Instantiate(Resources.Load("P1Knight")), "knight", new int[] { i, j });
                            break;
                        case 2:
                            Player1.AddPiece((GameObject)Instantiate(Resources.Load("P1Bishop")), "bishop", new int[] { i, j });
                            break;
                        case 3:
                            Player1.AddPiece((GameObject)Instantiate(Resources.Load("P1Queen")), "queen", new int[] { i, j });
                            break;
                        case 4:
                            Player1.AddPiece((GameObject)Instantiate(Resources.Load("P1King")), "king", new int[] { i, j });
                            break;
                        case 5:
                            newPiece = (GameObject)Instantiate(Resources.Load("P1Bishop"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player1.AddPiece(newPiece, "bishop", new int[] { i, j });
                            break;
                        case 6:
                            newPiece = (GameObject)Instantiate(Resources.Load("P1Knight"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player1.AddPiece(newPiece, "knight", new int[] { i, j });
                            break;
                        case 7:
                            newPiece = (GameObject)Instantiate(Resources.Load("P1Rook"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player1.AddPiece(newPiece, "rook", new int[] { i, j });
                            break;
                    }
                }
                else
                {
                    newPiece = (GameObject)Instantiate(Resources.Load("P1Pawn"));
                    newPiece.transform.position = new Vector3(
                        newPiece.transform.position.x + j, newPiece.transform.position.y, newPiece.transform.position.z
                        );
                    Player1.AddPiece(newPiece, "pawn", new int[] { i, j });
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
                            Player2.AddPiece((GameObject)Instantiate(Resources.Load("P2Rook")), "rook", new int[] { i, j });
                            break;
                        case 1:
                            Player2.AddPiece((GameObject)Instantiate(Resources.Load("P2Knight")), "knight", new int[] { i, j });
                            break;
                        case 2:
                            Player2.AddPiece((GameObject)Instantiate(Resources.Load("P2Bishop")), "bishop", new int[] { i, j });
                            break;
                        case 3:
                            Player2.AddPiece((GameObject)Instantiate(Resources.Load("P2Queen")), "queen", new int[] { i, j });
                            break;
                        case 4:
                            Player2.AddPiece((GameObject)Instantiate(Resources.Load("P2King")), "king", new int[] { i, j });
                            break;
                        case 5:
                            newPiece = (GameObject)Instantiate(Resources.Load("P2Bishop"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player2.AddPiece(newPiece, "bishop", new int[] { i, j });
                            break;
                        case 6:
                            newPiece = (GameObject)Instantiate(Resources.Load("P2Knight"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player2.AddPiece(newPiece, "knight", new int[] { i, j });
                            break;
                        case 7:
                            newPiece = (GameObject)Instantiate(Resources.Load("P2Rook"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player2.AddPiece(newPiece, "rook", new int[] { i, j });
                            break;
                    }
                }
                else
                {
                    newPiece = (GameObject)Instantiate(Resources.Load("P2Pawn"));
                    newPiece.transform.position = new Vector3(
                        newPiece.transform.position.x + j, newPiece.transform.position.y, newPiece.transform.position.z
                        );
                    Player2.AddPiece(newPiece, "pawn", new int[] { i, j });
                }
            }
        }
    }

    void SetCamera () {
        if (P1Turn)
        {
            GameObject.Find("CameraCenter").transform.position = new Vector3(0f, 0f, 0f);
            GameObject.Find("CameraCenter").transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else
        {
            GameObject.Find("CameraCenter").transform.position = new Vector3(0f, 0f, 0f);
            GameObject.Find("CameraCenter").transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        Camera.main.fieldOfView = 60f;
    }

    public List<int[]> GetAvailableMoves (GameObject SelectedPiece) {
        List<int[]> InvalidSpaces;  //spaces blocked by pieces
        List<int[]> ValidSpaces;  //spaces to which SelectedPiece can move
        int[] Position;
        string type;
        if (P1Turn) //Player1
        {
            Position = Player1.GetPiecePosition(SelectedPiece);
            InvalidSpaces = new List<int[]>(Player1.GetPiecePositions());
            ValidSpaces = new List<int[]>(Player1.AvailableMoves(Position));
            type = Player1.GetPieceType(Position);
            int i;
            if (type.Equals("KNIGHT"))
            {

                foreach (int[] space in InvalidSpaces)
                {
                    if (ValidSpaces.Contains(space))
                        ValidSpaces.Remove(space);
                }
            }
            else   //only knights can jump, so check for obstructions in move pattern
            {
                foreach (int[] PlayerPiece in Player2.GetPiecePositions())
                    InvalidSpaces.Add(PlayerPiece);
                if (type.Equals("QUEEN") || type.Equals("ROOK"))
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
                if (type.Equals("QUEEN") || type.Equals("Bishop"))
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
                if (type.Equals("KING"))  //SPECIAL CASE: king cannot castle when in check
                {
                    if (P1InCheck)
                    {
                        i = 0;
                        while (i < ValidSpaces.Count)
                        {
                            int[] space = ValidSpaces.ToArray()[i];
                            bool removedOne = false;
                            if (space[0] == Position[0])
                            {
                                if (space[1] == Position[1] + 2 || space[1] == Position[1] - 2)
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
                    if (!Player1.CanCastleLeft())
                    {
                        foreach (int[] space in ValidSpaces)
                        {
                            if (space[1] == Position[1] - 2)
                            {
                                ValidSpaces.Remove(space);
                                break;
                            }
                        }
                    }
                    if (!Player1.CanCastleRight())
                    {
                        foreach (int[] space in ValidSpaces)
                        {
                            if (space[1] == Position[1] + 2)
                            {
                                ValidSpaces.Remove(space);
                                break;
                            }
                        }
                    }
                }
                if (!type.Equals("PAWN"))
                {
                    foreach (int[] space in Player1.GetPiecePositions())
                    {
                        if (ValidSpaces.Contains(space))
                            ValidSpaces.Remove(space);
                    }
                }
                if (type.Equals("PAWN"))  //SPECIAL CASES: pawns attack diagonally, pawns can move two instead of one on first move.  OPT SPECIAL CASE: en passant rule
                {
                    if (ValidSpaces.Count > 1)  //pawn hasn't moved yet
                    {
                        foreach (int[] occSpace in InvalidSpaces)
                        {
                            if (occSpace[1] == Position[1])
                            {
                                if (occSpace[0] == Position[0] + 1)
                                {
                                    ValidSpaces.Clear();
                                    break;
                                }
                                if (occSpace[0] == Position[0] + 2)
                                    ValidSpaces.Remove(new int[] { Position[0] + 2, Position[1] });
                            }
                        }
                    }
                    foreach (int[] occSpace in InvalidSpaces)   //diagonal attack opportunity
                    {
                        if (occSpace[0] == Position[0] + 1)
                        {
                            if (occSpace[1] == Position[1] + 1 || occSpace[1] == Position[1] - 1)
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
            Position = Player2.GetPiecePosition(SelectedPiece);
            InvalidSpaces = new List<int[]>(Player2.GetPiecePositions());
            ValidSpaces = new List<int[]>(Player2.AvailableMoves(Position));
            type = Player2.GetPieceType(Position);
            int i;
            if (type.Equals("KNIGHT"))
            {

                foreach (int[] space in InvalidSpaces)
                {
                    if (ValidSpaces.Contains(space))
                        ValidSpaces.Remove(space);
                }
            }
            else   //only knights can jump, so check for obstructions in move pattern
            {
                if (!type.Equals("PAWN"))
                    foreach (int[] PlayerPiece in Player2.GetPiecePositions())
                        InvalidSpaces.Add(PlayerPiece);
                if (type.Equals("QUEEN") || type.Equals("ROOK"))
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
                if (type.Equals("QUEEN") || type.Equals("Bishop"))
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
                if (type.Equals("KING"))  //SPECIAL CASE: king cannot castle when in check
                {
                    if (P2InCheck)
                    {
                        i = 0;
                        while (i < ValidSpaces.Count)
                        {
                            int[] space = ValidSpaces.ToArray()[i];
                            bool removedOne = false;
                            if (space[0] == Position[0])
                            {
                                if (space[1] == Position[1] + 2 || space[1] == Position[1] - 2)
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
                    if (!Player2.CanCastleLeft())
                    {
                        foreach(int[] space in ValidSpaces)
                        {
                            if (space[1] == Position[1] - 2)
                            {
                                ValidSpaces.Remove(space);
                                break;
                            }
                        }
                    }
                    if (!Player2.CanCastleRight())
                    {
                        foreach (int[] space in ValidSpaces)
                        {
                            if (space[1] == Position[1] + 2)
                            {
                                ValidSpaces.Remove(space);
                                break;
                            }
                        }
                    }
                }
                if (!type.Equals("PAWN"))
                {
                    foreach (int[] space in Player2.GetPiecePositions())
                    {
                        if (ValidSpaces.Contains(space))
                            ValidSpaces.Remove(space);
                    }
                }
                if (type.Equals("PAWN"))  //SPECIAL CASES: pawns attack diagonally, pawns can move two instead of one on first move.  OPT SPECIAL CASE: en passant rule
                {
                    if (ValidSpaces.Count > 1)  //pawn hasn't moved yet
                    {
                        foreach (int[] occSpace in InvalidSpaces)
                        {
                            if (occSpace[1] == Position[1])
                            {
                                if (occSpace[0] == Position[0] - 1)
                                {
                                    ValidSpaces.Clear();
                                    break;
                                }
                                if (occSpace[0] == Position[0] - 2)
                                    ValidSpaces.Remove(new int[] { Position[0] - 2, Position[1] });
                            }
                        }
                    }
                    foreach (int[] occSpace in InvalidSpaces)   //diagonal attack opportunity
                    {
                        if (occSpace[0] == Position[0] - 1)
                        {
                            if (occSpace[1] == Position[1] + 1 || occSpace[1] == Position[1] - 1)
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

    public void Move(Rigidbody pieceRB, GameObject Piece, Vector3 Target, bool Castle)
    {
        pieceRB.isKinematic = false;
        pieceRB.AddForce(ForceCalculation(pieceRB, Target, Piece.transform.position), ForceMode.Acceleration);
    }

    public void CheckPieceTake(Vector3 Target) {
        List<int[]> opposingPieces = new List<int[]>();
        if (P1Turn)
            opposingPieces = Player2.GetPiecePositions();
        else
            opposingPieces = Player1.GetPiecePositions();
        int i = 0;
        foreach (int[] space in opposingPieces)
        {
            if (Spaces[space[0], space[1]].transform.position.x == Target.x && 
                Spaces[space[0], space[1]].transform.position.z == Target.z)
            {
                GameObject[] pieces;
                if (P1Turn)
                {
                    pieces = Player2.GetPieces().ToArray();
                    Player2.RemovePiece(pieces[i]);
                }
                else
                {
                    pieces = Player1.GetPieces().ToArray();
                    Player1.RemovePiece(pieces[i]);
                }
                Destroy(pieces[i]);
                break;
            }
            i++;
        }
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
        int[] kingPos;
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
                kingPos = Player2.GetPiecePosition(opposingKing);
                if (move[0] == kingPos[0] && move[1] == kingPos[1])
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
                kingPos = Player1.GetPiecePosition(opposingKing);
                if (move[0] == kingPos[0] && move[1] == kingPos[1])
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
        int[] kingPos;
        if (P1Turn) //?Player1 put self in check?
        {
            ownKing = Player1.GetKing();
            foreach (GameObject piece in Player2.GetPieces())
                opposingMoves.AddRange(GetAvailableMoves(piece, movingPiece, targetPosition));

            foreach (int[] move in opposingMoves)
            {
                kingPos = Player1.GetPiecePosition(ownKing);
                if (move[0] == kingPos[0] && move[1] == kingPos[1])
                    return true;
            }
        }
        else    //?Player2 put self in check?
        {
            ownKing = Player2.GetKing();
            foreach (GameObject piece in Player1.GetPieces())
                opposingMoves.AddRange(GetAvailableMoves(piece, movingPiece, targetPosition));

            foreach (int[] move in opposingMoves)
            {
                kingPos = Player2.GetPiecePosition(ownKing);
                if (move[0] == kingPos[0] && move[1] == kingPos[1])
                    return true;
            }
        }
        return false;
    }

    public List<Color> HighlightSpaces(List<int[]> availMoves)
    {
        Material highlight = (Material)Instantiate(Resources.Load("Highlight"));
        List<Color> originalColors = new List<Color>();
        foreach(int[] space in availMoves)
        {
            MeshRenderer renderer = Spaces[space[0], space[1]].transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
            originalColors.Add(renderer.material.color);
            renderer.material.color = highlight.color;
        }
        return originalColors;
    }

    public void UndoHighlights(List<int[]> highlightedSpaces, List<Color> originalColors)
    {
        for (int i = 0; i < highlightedSpaces.Count; i++)
        {
            int[] space = highlightedSpaces.ToArray()[i];
            MeshRenderer renderer = Spaces[space[0], space[1]].transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
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

    private List<int[]> GetAvailableMoves(GameObject SelectedPiece, GameObject movingPiece, int[] targetPosition)
    {
        List<int[]> InvalidSpaces;  //spaces blocked by pieces
        List<int[]> ValidSpaces;  //spaces to which SelectedPiece can move
        int[] Position;
        string type;
        if (!P1Turn) //Player2  for <boolean> checkcheck method
        {
            Position = Player2.GetPiecePosition(SelectedPiece);
            InvalidSpaces = new List<int[]>(Player1.GetPiecePositions());
            ValidSpaces = new List<int[]>(Player1.AvailableMoves(Position));
            int i;
            foreach (int[] space in ValidSpaces)
            {
                if (InvalidSpaces.Contains(space))
                    ValidSpaces.Remove(space);
            }
            type = Player2.GetPieceType(Position);
            if (!type.Equals("KNIGHT"))   //only knights can jump, so check for obstructions in move pattern
            {
                foreach (int[] PlayerPiece in Player2.GetPiecePositions())
                    InvalidSpaces.Add(PlayerPiece);
                InvalidSpaces.Remove(Player2.GetPiecePosition(movingPiece));
                InvalidSpaces.Add(targetPosition);
                if (type.Equals("QUEEN") || type.Equals("ROOK"))
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
                if (type.Equals("QUEEN") || type.Equals("Bishop"))
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
                if (type.Equals("KING"))  //SPECIAL CASE: king cannot castle when in check
                {
                    if (P2InCheck)
                    {
                        i = 0;
                        while (i < ValidSpaces.Count)
                        {
                            int[] space = ValidSpaces.ToArray()[i];
                            bool removedOne = false;
                            if (space[0] == Position[0])
                            {
                                if (space[1] == Position[1] + 2 || space[1] == Position[1] - 2)
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
                    if (!Player2.CanCastleLeft())
                    {
                        foreach (int[] space in ValidSpaces)
                        {
                            if (space[1] == Position[1] - 2)
                            {
                                ValidSpaces.Remove(space);
                                break;
                            }
                        }
                    }
                    if (!Player2.CanCastleRight())
                    {
                        foreach (int[] space in ValidSpaces)
                        {
                            if (space[1] == Position[1] + 2)
                            {
                                ValidSpaces.Remove(space);
                                break;
                            }
                        }
                    }
                }
                if (type.Equals("PAWN"))  //SPECIAL CASES: pawns attack diagonally, pawns can move two instead of one on first move.  OPT SPECIAL CASE: en passant rule
                {
                    InvalidSpaces.Clear();
                    foreach (int[] PlayerPiece in Player2.GetPiecePositions())
                        InvalidSpaces.Add(PlayerPiece);
                    InvalidSpaces.Remove(Player2.GetPiecePosition(movingPiece));
                    InvalidSpaces.Add(targetPosition);
                    if (ValidSpaces.Count > 1)  //pawn hasn't moved yet
                    {
                        foreach (int[] occSpace in InvalidSpaces)
                        {
                            if (occSpace[1] == Position[1])
                            {
                                if (occSpace[0] == Position[0] + 1)
                                {
                                    ValidSpaces.Clear();
                                    break;
                                }
                                if (occSpace[0] == Position[0] + 2)
                                    ValidSpaces.Remove(new int[] { Position[0] + 2, Position[1] });
                            }
                        }
                    }
                    foreach (int[] occSpace in InvalidSpaces)   //diagonal attack opportunity
                    {
                        if (occSpace[0] == Position[0] + 1)
                        {
                            if (occSpace[1] == Position[1] + 1 || occSpace[1] == Position[1] - 1)
                                ValidSpaces.Add(occSpace);
                        }
                    }
                }
            }
        }
        else    //Player1 for <boolean> checkcheck
        {
            Position = Player2.GetPiecePosition(SelectedPiece);
            InvalidSpaces = new List<int[]>(Player2.GetPiecePositions());
            ValidSpaces = new List<int[]>(Player2.AvailableMoves(Position));
            type = Player2.GetPieceType(Position);
            int i;
            if (type.Equals("KNIGHT"))
            {

                foreach (int[] space in InvalidSpaces)
                {
                    if (ValidSpaces.Contains(space))
                        ValidSpaces.Remove(space);
                }
            }
            else   //only knights can jump, so check for obstructions in move pattern
            {
                foreach (int[] PlayerPiece in Player1.GetPiecePositions())
                    InvalidSpaces.Add(PlayerPiece);
                InvalidSpaces.Remove(Player1.GetPiecePosition(movingPiece));
                InvalidSpaces.Add(targetPosition);
                if (type.Equals("QUEEN") || type.Equals("ROOK"))
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
                if (type.Equals("QUEEN") || type.Equals("Bishop"))
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
                if (type.Equals("KING"))  //SPECIAL CASE: king cannot castle when in check
                {
                    if (P2InCheck)
                    {
                        i = 0;
                        while (i < ValidSpaces.Count)
                        {
                            int[] space = ValidSpaces.ToArray()[i];
                            bool removedOne = false;
                            if (space[0] == Position[0])
                            {
                                if (space[1] == Position[1] + 2 || space[1] == Position[1] - 2)
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
                    if (!Player2.CanCastleLeft())
                    {
                        foreach (int[] space in ValidSpaces)
                        {
                            if (space[1] == Position[1] - 2)
                            {
                                ValidSpaces.Remove(space);
                                break;
                            }
                        }
                    }
                    if (!Player2.CanCastleRight())
                    {
                        foreach (int[] space in ValidSpaces)
                        {
                            if (space[1] == Position[1] + 2)
                            {
                                ValidSpaces.Remove(space);
                                break;
                            }
                        }
                    }
                }
                if (!type.Equals("PAWN"))
                {
                    foreach(int[] space in Player2.GetPiecePositions())
                    {
                        if (ValidSpaces.Contains(space))
                            ValidSpaces.Remove(space);
                    }
                }
                if (type.Equals("PAWN"))  //SPECIAL CASES: pawns attack diagonally, pawns can move two instead of one on first move.  OPT SPECIAL CASE: en passant rule
                {
                    if (ValidSpaces.Count > 1)  //pawn hasn't moved yet
                    {
                        foreach (int[] occSpace in InvalidSpaces)
                        {
                            if (occSpace[1] == Position[1])
                            {
                                if (occSpace[0] == Position[0] - 1)
                                {
                                    ValidSpaces.Clear();
                                    break;
                                }
                                if (occSpace[0] == Position[0] - 2)
                                    ValidSpaces.Remove(new int[] { Position[0] - 2, Position[1] });
                            }
                        }
                    }
                    if (InvalidSpaces.Contains(ValidSpaces.ToArray()[0]))
                        ValidSpaces.Clear();
                    foreach (int[] occSpace in InvalidSpaces)   //diagonal attack opportunity
                    {
                        if (occSpace[0] == Position[0] - 1)
                        {
                            if (occSpace[1] == Position[1] + 1 || occSpace[1] == Position[1] - 1)
                                ValidSpaces.Add(occSpace);
                        }
                    }
                }
            }
        }
        return ValidSpaces;
    }
}
