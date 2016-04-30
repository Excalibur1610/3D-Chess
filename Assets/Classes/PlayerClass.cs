using System.Collections.Generic;
using UnityEngine;

public class PlayerClass {
    private List<PieceClass> Pieces;
    private bool movedKing;
    private List<GameObject> InGamePieces;

    public PlayerClass(bool isP1) {
        Pieces = new List<PieceClass>();
        movedKing = false;
        InGamePieces = new List<GameObject>();
    }

    private void AddPiece(string Name, int[] Location) {
        Pieces.Add(new PieceClass(Name, Location));
    }

    public void AddPiece(GameObject Piece, string Name, int[] Location) {
        AddPiece(Name, Location );
        InGamePieces.Add(Piece);
    }

    public void RemovePiece(GameObject Piece)
    {
        
    }

    public void Move(GameObject Piece, int[] newPosition) {
        int i = 0;
        GameObject[] pieces = InGamePieces.ToArray();
        while (i < InGamePieces.Count)
            if (pieces[i] == Piece)
                break;
        int[] PiecePosition = Pieces.ToArray()[i].GetLocation();
        i = 0;
        while (i < Pieces.Count)
        {
            PieceClass piece = Pieces.ToArray()[i];
            if (piece.GetLocation() == PiecePosition)
                piece.SetPiece(newPosition);
        }
    }

    public List<int[]> AvailableMoves(int[] PiecePosition) {
        List<int[]> AvailableMoves = new List<int[]>();
        string type = GetPieceType(PiecePosition);
        if (type.Equals("PAWN"))
        {
            AvailableMoves.Add(new int[] { PiecePosition[0] + 1, PiecePosition[1] });
            if (PiecePosition[1] == 1)
                AvailableMoves.Add(new int[] { PiecePosition[0] + 2, PiecePosition[1] });
        }
        return AvailableMoves;
    }

    public List<int[]> GetPiecePositions() {
        List<int[]> Positions = new List<int[]>();
        foreach (PieceClass Piece in Pieces)
            Positions.Add(Piece.GetLocation());
        return Positions;
    }

    public string GetPieceType(int[] PiecePosition) {
        int i = 0;
        while (i < Pieces.Count)
        {
            PieceClass piece = Pieces.ToArray()[i];
            if (piece.GetLocation() == PiecePosition)
                return piece.GetName();
        }
        return null;
    }

    public bool KingMoved()
    {
        return movedKing;
    }

    public void MoveKing()
    {
        movedKing = true;
    }

    public List<GameObject> GetPieces()
    {
        return InGamePieces;
    }

    public GameObject GetKing()
    {
        foreach(GameObject piece in InGamePieces)
        {
            if (GetPieceType(new int[] { (int)(piece.transform.position.x + 3.5f), (int)(piece.transform.position.z + 3.5f) }).Equals("KING"))
            {
                return piece;
            }
        }
        return null;
    }
}
