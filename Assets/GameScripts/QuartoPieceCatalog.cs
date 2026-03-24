using System;
using System.Collections.Generic;

public static class QuartoPieceCatalog {
    public static readonly string[] Names ={
        "TallHollowWhiteCylinder", "TallHollowBlackCylinder", "TallSolidWhiteCylinder", "TallSolidBlackCylinder",
        "ShortHollowWhiteCylinder", "ShortHollowBlackCylinder", "ShortSolidWhiteCylinder", "ShortSolidBlackCylinder",
        "TallHollowWhitePrism", "TallHollowBlackPrism", "TallSolidWhitePrism", "TallSolidBlackPrism",
        "ShortHollowWhitePrism", "ShortHollowBlackPrism", "ShortSolidWhitePrism", "ShortSolidBlackPrism"
    };

    static readonly Dictionary<string, byte> IndexByName = BuildIndex();

    static Dictionary<string, byte> BuildIndex(){
        var d = new Dictionary<string, byte>(StringComparer.Ordinal);
        for (int i = 0; i < Names.Length; i++)
            d[Names[i]] = (byte)i;
        return d;
    }

    public static bool TryGetIndex(string pieceObjectName, out byte index){
        return IndexByName.TryGetValue(pieceObjectName, out index);
    }

    public static string GetName(byte index){
        if (index >= Names.Length) return null;
        return Names[index];
    }
}