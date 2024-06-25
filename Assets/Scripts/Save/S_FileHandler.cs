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

        if (System.IO.File.Exists(filePath))
        {
            string inventoryData = System.IO.File.ReadAllText(filePath);
            InventorySaver inventorySaver = JsonUtility.FromJson<InventorySaver>(inventoryData);
            S_DataGame.Instance.inventory = inventorySaver;
            Debug.Log("Changement effectué");
            Debug.Log(S_DataGame.Instance.inventory.CurrentMoney);
            Debug.Log(filePath);
        }
        else
        {
            Debug.LogWarning("Le fichier d'inventaire n'a pas été trouvé, création d'un inventaire par défaut.");

            S_DataGame.Instance.inventory = CreateDefaultInventory();
        }
    }

    private InventorySaver CreateDefaultInventory()
    {
        InventorySaver defaultInventory = new InventorySaver();
        
        defaultInventory.CurrentMoney = 1000;
        defaultInventory.prefixIndex = 0;
        defaultInventory.suffixIndex = 0;
        defaultInventory.LoadPrefixName();
        defaultInventory.LoadSuffixName();
        
        return defaultInventory;
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
