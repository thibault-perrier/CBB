using UnityEngine;

public class S_FileHandler: MonoBehaviour
{
    public static S_FileHandler Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void SaveInventory()
    {
        string inventoryData = JsonUtility.ToJson(S_DataGame.Instance.inventory);
        string filePath = Application.persistentDataPath + "/InventoryData.json";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, inventoryData);
        Debug.Log("Sauvegarde effectu�");
    }

    public void LoadInventory()
    {
        string filePath = Application.persistentDataPath + "/InventoryData.json";
        string inventoryData = System.IO.File.ReadAllText(filePath);
        S_DataGame.Instance.inventory = JsonUtility.FromJson<InventorySaver>(inventoryData);
        Debug.Log("Changement effectu�");
    }

    public void SaveTournament()
    {
        string tournamentData = JsonUtility.ToJson(S_DataGame.Instance.tournament);
        string filePath = Application.persistentDataPath + "/TournamentData.json";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, tournamentData);
        Debug.Log("Sauvegarde effectu�");
    }

    public void LoadTournament()
    {
        string filePath = Application.persistentDataPath + "/TournamentData.json";
        string tournamentData = System.IO.File.ReadAllText(filePath);

        S_DataGame.Instance.tournament = JsonUtility.FromJson<TournamentSaver>(tournamentData);
        Debug.Log("Changement effectu�");
    }
}
