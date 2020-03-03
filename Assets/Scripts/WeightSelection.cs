using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeightSelection : MonoBehaviour {
    public Text weightDisplay;
    private int playerWeight;
    private string input;
    private int maxWeight = 999;

    // Start is called before the first frame update
    void Start() {
        input = "";
        weightDisplay.text = "00";
    }

    public int GetPlayerWeight() {
        return playerWeight;
    }

    public void ClearWeight() {
        playerWeight = 0;
        input = "";
        weightDisplay.text = "00";
    }

    public void KeypadInput(int num) {
        Debug.Log("Keypad input: " + num);
        input = input + num.ToString();
        playerWeight = int.Parse(input);

        if (playerWeight > maxWeight) {
            playerWeight = num;
            input = num.ToString();
        }
        weightDisplay.text = input;
    }

    public void ConfirmWeight() {
        playerWeight = int.Parse(input);
        Debug.Log("Player is " + playerWeight + " kg!");
    }
}
