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
        System.IO.File.WriteAllText(filePath, inventoryData);
        Debug.Log("Sauvegarde effectué");
        Debug.Log(filePath);
    }

    public void LoadInventory()
    {
        string filePath = Application.persistentDataPath + "/InventoryData.json";
        string inventoryData = System.IO.File.ReadAllText(filePath);
        InventorySaver inventorySaver = JsonUtility.FromJson<InventorySaver>(inventoryData);
        S_DataGame.Instance.inventory = inventorySaver;
        Debug.Log("Changement effectué");
        Debug.Log(S_DataGame.Instance.inventory.CurrentMoney);
        Debug.Log(filePath);
    }

    public void SaveTournament()
    {
        string tournamentData = JsonUtility.ToJson(S_DataGame.Instance.tournament);
        string filePath = Application.persistentDataPath + "/TournamentData.json";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, tournamentData);
        Debug.Log("Sauvegarde effectué");
    }

    public void LoadTournament()
    {
        string filePath = Application.persistentDataPath + "/TournamentData.json";
        string tournamentData = System.IO.File.ReadAllText(filePath);

        S_DataGame.Instance.tournament = JsonUtility.FromJson<TournamentSaver>(tournamentData);
        Debug.Log("Changement effectué");
    }
}
