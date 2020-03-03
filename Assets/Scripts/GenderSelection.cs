using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenderSelection : MonoBehaviour {
    private bool isPlayerMale = false;

    public bool IsPlayerMale() {
        return isPlayerMale;
    }

    public void SetPlayerMale(bool isMale) {
        isPlayerMale = isMale;
    }
}
