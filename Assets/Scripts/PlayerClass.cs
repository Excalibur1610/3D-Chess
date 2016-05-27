using System.Collections.Generic;
using UnityEngine;

public class PlayerClass {
    private List<PieceClass> Pieces;
    private List<GameObject> InGamePieces;

    public PlayerClass() {
        Pieces = new List<PieceClass>();
        InGamePieces = new List<GameObject>();
    }

    private void AddPiece(string Name, int[] Location) {
        Pieces.Add(new PieceClass(Name, Location));
    }

    public void AddPiece(GameObject Piece, string Name, int[] Location) {
        AddPiece(Name, Location);
        InGamePieces.Add(Piece);
    }

    public void RemovePiece(GameObject Piece)
    {
        int i = 0;
        while (i < InGamePieces.Count)
        {
            if (Piece.Equals(InGamePieces.ToArray()[i]))
            {
                Pieces.RemoveAt(i);
                InGamePieces.RemoveAt(i);
                break;
            }
            i++;
        }
    }

    public void RemovePiece(int index)
    {
        Pieces.RemoveAt(index);
        InGamePieces.RemoveAt(index);
    }

    public void Move(GameObject Piece, int[] newPosition) {
        int i = 0;
        GameObject[] pieces = InGamePieces.ToArray();
        while (i < InGamePieces.Count)
        {
            if (pieces[i] == Piece)
                break;
            i++;
        }
        Pieces.ToArray()[i].SetPiece(newPosition);
    }

    public List<int[]> GetPiecePositions() {
        List<int[]> Positions = new List<int[]>();
        foreach (PieceClass Piece in Pieces)
            Positions.Add(Piece.GetLocation());
        return Positions;
    }

    public int[] GetPiecePosition(GameObject piece)
    {
        int count = 0;
        foreach (GameObject Piece in InGamePieces) {
            if (Piece.Equals(piece))
                return Pieces.ToArray()[count].GetLocation();
            count++;
        }
        return null;
    }

    public string GetPieceType(int[] PiecePosition) {
       foreach (PieceClass piece in Pieces)
        {
            int[] pos = piece.GetLocation();
            if (pos[0] == PiecePosition[0] && pos[1] == PiecePosition[1])
                return piece.GetName();
        }
        return null;
    }

    public List<GameObject> GetPieces()
    {
        return InGamePieces;
    }
}
