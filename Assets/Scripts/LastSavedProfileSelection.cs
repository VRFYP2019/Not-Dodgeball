using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastSavedProfileSelection : MonoBehaviour {
    private bool isUseLastSaved;

    public bool IsUseLastSaved() {
        return isUseLastSaved;
    }

    public void SetUseLastSaved(bool useLastSaved) {
        isUseLastSaved = useLastSaved;
        if (useLastSaved) {
            GetComponent<GenderSelection>().SetPlayerMale(PlayerPrefs.GetInt("isPlayerMale") == 1 ? true : false);
            GetComponent<AgeSelection>().SetPlayerAge(PlayerPrefs.GetInt("playerAge"));
            GetComponent<WeightSelection>().SetPlayerWeight(PlayerPrefs.GetInt("playerWeight"));
        }
    }

    public void DeleteLastSaved() {
        PlayerPrefs.DeleteKey("isPlayerMale");
        PlayerPrefs.DeleteKey("playerAge");
        PlayerPrefs.DeleteKey("playerWeight");
    }
}
