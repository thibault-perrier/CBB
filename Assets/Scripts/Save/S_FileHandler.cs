using UnityEngine;

public class S_FileHandler: MonoBehaviour
{
    public static S_FileHandler instance;

    private void Awake()
    {
        instance = this;
    }
    public void SaveInventory()
    {
        string inventoryData = JsonUtility.ToJson(S_DataGame.instance.inventory);
        string filePath = Application.persistentDataPath + "/InventoryData.json";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, inventoryData);
        Debug.Log("Sauvegarde effectué");
    }

    public void LoadInventory()
    {
        string filePath = Application.persistentDataPath + "/InventoryData.json";
        string inventoryData = System.IO.File.ReadAllText(filePath);

        S_DataGame.instance.inventory = JsonUtility.FromJson<InventorySaver>(inventoryData);
        Debug.Log("Changement effectué");
    }

    public void SaveTournament()
    {
        string tournamentData = JsonUtility.ToJson(S_DataGame.instance.tournament);
        string filePath = Application.persistentDataPath + "/TournamentData.json";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, tournamentData);
        Debug.Log("Sauvegarde effectué");
    }

    public void LoadTournament()
    {
        string filePath = Application.persistentDataPath + "/TournamentData.json";
        string tournamentData = System.IO.File.ReadAllText(filePath);

        S_DataGame.instance.tournament = JsonUtility.FromJson<TournamentSaver>(tournamentData);
        Debug.Log("Changement effectué");
    }
}
