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

    public static class Constants {
        public static Color blue = new Color(0, 0.5f, 1);
        public static Color orange = new Color(0.937f, 0.447f, 0.0823f);
    }
}
