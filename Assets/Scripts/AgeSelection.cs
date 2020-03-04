using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgeSelection : MonoBehaviour {
    public Text ageDisplay;
    private int playerAge;
    private string input;
    private int maxAge = 99;

    // Start is called before the first frame update
    void Start() {
        input = "";
        ageDisplay.text = "00";
    }

    public int GetPlayerAge() {
        return playerAge;
    }

    public void ClearAge() {
        playerAge = 0;
        input = "";
        ageDisplay.text = "00";
    }

    public void KeypadInput(int num) {
        Debug.Log("Keypad input: " + num);
        input = input + num.ToString();
        playerAge = int.Parse(input);

        if (playerAge > maxAge) {
            playerAge = num;
            input = num.ToString();
        }
        ageDisplay.text = input;
    }

    public void ConfirmAge() {
        playerAge = int.Parse(input);
        Debug.Log("Player is " + playerAge + " years old!");
    }

    // For setting age via script
    public void SetPlayerAge(int age) {
        playerAge = age;
    }
}
