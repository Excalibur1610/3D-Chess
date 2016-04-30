using System.Collections.Generic;

public class PieceClass {
    private string Name;
    private int[] Location;

    public PieceClass(string name, int[] location)
    {
        Name = name;
        Location = location;
    }

    public int[] GetLocation() {
        return Location;
    }

    public string GetName() {
        return Name;
    }

    public void Move(int[] NewLocation) {
        Location = NewLocation;
    }

    public void SetPiece(int[] Location) {
        this.Location = Location;
    }
}
