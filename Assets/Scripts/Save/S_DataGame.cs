using System;
using System.Collections.Generic;
using System.Linq;
using Systems;
using Unity.VisualScripting;
using UnityEngine;

public class S_DataGame : MonoBehaviour
{
    public static S_DataGame Instance;
    [SerializeField] public S_TournamentManager _tournamentManager;

    [Serializable]
    public enum Load
    {
        Nothing,
        Inventory,
        Tournament,
        InventoryAndTournament
    }

    public Load OnSceneLoad;

    public InventorySaver inventory = new InventorySaver();
    public TournamentSaver tournament = new TournamentSaver();

    private void Awake()
    {
        Instance = this;
        if (OnSceneLoad == Load.Inventory || OnSceneLoad == Load.InventoryAndTournament)
        {
            LoadInventory();
        }
        if (OnSceneLoad == Load.Tournament || OnSceneLoad == Load.InventoryAndTournament)
        {
            LoadTournament();
        }
    }

    public void SaveInventory()
    {
        S_FileHandler.Instance.SaveInventory();
    }

    public void LoadInventory()
    {
        S_FileHandler.Instance.LoadInventory();
    }

    public void SaveTournament()
    {
        S_FileHandler.Instance.SaveTournament();
    }

    public void LoadTournament()
    {
        S_FileHandler.Instance.LoadTournament();
    }
}

[System.Serializable]
public class InventorySaver // Inventory 
{
    public int CurrentMoney;
    public List<Frame> Frames = new List<Frame>();
    public List<Weapon> Weapons = new List<Weapon>();
    public List<Robot> Robots = new List<Robot>();

    public string backgroundColorHex;
    public string overlayColorHex;
    public float backgroundAlpha = 255;

    public int prefixIndex;
    public int suffixIndex;

    public int overlayImageIndex;
    public Vector2 overlayImagePosition;

    #region Color Load/Save
    public void SaveOverlayColor(string hexColor)
    {
        overlayColorHex = hexColor;
        PlayerPrefs.SetString("CurrentOverlayColor", hexColor);
        PlayerPrefs.Save();
        Debug.Log("Overlay Color Saved: " + hexColor);
    }

    public void SaveBackgroundColor(string hexColor)
    {
        backgroundColorHex = hexColor;
        PlayerPrefs.SetString("CurrentBackgroundColor", hexColor);
        PlayerPrefs.Save();
        Debug.Log("Background Color Saved: " + hexColor);
    }

    public void SaveOverlayImage(int imageIndex)
    {
        overlayImageIndex = imageIndex;
        PlayerPrefs.SetInt("OverlayImageIndex", imageIndex);
        PlayerPrefs.Save();
    }

    // Charger les couleurs
    public void LoadColors()
    {
        backgroundColorHex = PlayerPrefs.GetString("CurrentBackgroundColor", backgroundColorHex);
        Debug.Log("Background Color Loaded: " + backgroundColorHex + " with Alpha: " + backgroundAlpha);
    }

    public void LoadOverlayColor()
    {
        overlayColorHex = PlayerPrefs.GetString("CurrentOverlayColor", overlayColorHex);
        Debug.Log("Overlay Color Loaded: " + overlayColorHex);
    }

    public void LoadOverlayImageIndex()
    {
        overlayImageIndex = PlayerPrefs.GetInt("OverlayImageIndex", overlayImageIndex);
    }
    #endregion
    #region Name Load/Save
    public void SavePrefixName(int prefix)
    {
        prefixIndex = prefix;
        PlayerPrefs.SetInt("CurrentPrefixName", prefix);
        PlayerPrefs.Save();
    }
    public void SaveSuffixname(int suffix)
    {
        suffixIndex = suffix;
        PlayerPrefs.SetInt("CurrentSuffixName", suffix);
        PlayerPrefs.Save();
    }

    public bool LoadPrefixName()
    {
        if (PlayerPrefs.HasKey("CurrentPrefixName"))
        {
            prefixIndex = PlayerPrefs.GetInt("CurrentPrefixName", prefixIndex);
            return true;
        }
        return false;
    }
    public bool LoadSuffixName()
    {
        if (PlayerPrefs.HasKey("CurrentSuffixName"))
        {
            suffixIndex = PlayerPrefs.GetInt("CurrentSuffixName", suffixIndex);
            return true;
        }

        return false;
    }

    #endregion

    public Weapon GetWeapon(S_WeaponData weaponData)
    {
        foreach (Weapon weapon in Weapons)
        {
            S_WeaponData data = weapon.GetWeaponData();
            if (data == weaponData)
            {
                return weapon;
            }
        }
        return null;
    }

    public void AddWeapon(S_WeaponData weaponData)
    {
        Weapon weapon = GetWeapon(weaponData);
        if(weapon == null)
        {
            weapon = new Weapon(weaponData);
            Weapons.Add(weapon);
        }
        weapon._number++;
    }

    public void RemoveWeapon(S_WeaponData weaponData)
    {
        Weapon weapon = GetWeapon(weaponData);
        if (weapon != null)
        {
            weapon._number--;
            if (weapon._number <= 0)
                Weapons.Remove(weapon);
        } 
    }

    public Frame GetFrame(S_FrameData frameData)
    {
        foreach (Frame frame in Frames)
        {
            if (frame.GetFrameData() == frameData)
            {
                return frame;
            }
        }
        return null;
    }

    public void AddFrame(S_FrameData frameData)
    {
        Frame frame = GetFrame(frameData);
        if (frame == null)
        {
            frame = new Frame(frameData);
            Frames.Add(frame);
        }
        frame._number++;
    }

    public void RemoveFrame(S_FrameData frameData)
    {
        Frame frame = GetFrame(frameData);
        if (frame != null)
        {
            frame._number--;
            if (frame._number <= 0)
                Frames.Remove(frame);
        }
    }

    public void RemoveRobot(int index)
    {
        Robots.RemoveAt(index);
    }

    public void UpdateUseItem()
    {

        foreach (Weapon weapon in Weapons)
        {
            weapon._useNumber = 0;
        }

        foreach (Robot robot in Robots)
        {
            robot._frame._useNumber++;
            foreach(Weapon rbWeapon in robot._weapons)
            {
                if(rbWeapon != null)
                {
                    foreach(Weapon weapon in Weapons)
                    {
                        if(weapon._id == rbWeapon._id)
                        {
                            weapon._useNumber++;
                        }
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class Weapon
{
    public int _id;
    public string _name;
    public int _number;
    public int _useNumber;

    public S_WeaponData GetWeaponData()
    {
        return S_DataRobotComponent.Instance._weaponDatas[_id];
    }

    public Weapon(S_WeaponData weaponData)
    {
        _id = S_DataRobotComponent.Instance._weaponDatas.IndexOf(weaponData);
        _name = weaponData.name;
        _number = 1;
        _useNumber = 0;
    }

}

[System.Serializable]
public class Frame
{
    public int _id;
    public string _name;
    public int _number;
    public int _useNumber;

    public S_FrameData GetFrameData()
    {
        return S_DataRobotComponent.Instance._frameDatas[_id];
    }

    public Frame(S_FrameData frameData)
    {
        _id = S_DataRobotComponent.Instance._frameDatas.IndexOf(frameData);
        _name = frameData.name;
        _number = 1;
        _useNumber = 0;
    }
}

[System.Serializable]
public class Robot
{
    public Frame _frame;
    public List<Weapon> _weapons = new List<Weapon>();

    public Robot(Frame frame)
    {
        _frame = frame;
        for(int i = 0; i < frame.GetFrameData().GetNbWeaponMax(); i++)
        {
            _weapons.Add(null);
        }
    }

    public void UpdateWeaponMaxList()
    {
        List<Weapon> updateWeapons = new List<Weapon>();
        for (int i = 0; i < _frame.GetFrameData().GetNbWeaponMax(); i++)
        {
            updateWeapons.Add(_weapons[i]);
        }
        _weapons = updateWeapons.ToList();
    }
}

[System.Serializable]
public class TournamentSaver // Tournament
{
    public bool _isRunning;
    public List<S_TournamentManager.Participant> _participants;
    public int _currentMatch;
    public int _currentLevel;
    public S_TournamentManager.Tournament _tournamentInfo;
    public List<S_TournamentManager.Participant> _roundWinners;
}

