using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils {
    public enum PlayerNumber {
        ONE,
        TWO,
        NULL
    }
    public enum PlayerType {
        HUMAN,
        BOT
    }
    public enum PlayerPlatform {
        EDITOR,
        OCULUS,
        STEAMVR
    }
    public enum HandSide {
        UNSPECIFIED,
        LEFT,
        RIGHT
    }
    public enum HandObject {
        TOOL,
        SPAWNER
    }

    public static class Constants {
        public static class Colors {
            public static Color blue = new Color(0, 0.5f, 1);
            public static Color orange = new Color(0.937f, 0.447f, 0.0823f);
        }

        public static class FontSizes {
            public static int scoreBoardNormal = 25;
            public static int scoreBoardSmall = 12;
        }
    }

    public static class Math {
        // Modulo that handles negative numbers as well
        // (C#'s % is a remainder function, not a modulo)
        public static int Modulo(int a, int b) {
            return ((a %= b) < 0) ? a + b : a;
        }
    }
}
