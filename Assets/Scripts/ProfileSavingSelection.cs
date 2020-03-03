using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileSavingSelection : MonoBehaviour {
    private bool isSaveProfile = true;

    public bool IsSaveProfile() {
        return isSaveProfile;
    }

    public void SetIsSaveProfile(bool shouldSave) {
        isSaveProfile = shouldSave;
    }

    public void SaveProfile() {
        PlayerPrefs.SetInt("isPlayerMale", GetComponent<GenderSelection>().IsPlayerMale() ? 1 : 0);
        PlayerPrefs.SetInt("playerAge", GetComponent<AgeSelection>().GetPlayerAge());
        PlayerPrefs.SetInt("playerWeight", GetComponent<WeightSelection>().GetPlayerWeight());
        Debug.Log("Caloric profile saved successfully!");
    }
}
