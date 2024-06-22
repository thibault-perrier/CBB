using System;
using System.Collections.Generic;
using System.Linq;
using Systems;
using UnityEngine;
using static Robot;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.EventSystems.EventTrigger;

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
    }

    private void Start()
    {
        if (OnSceneLoad == Load.Inventory || OnSceneLoad == Load.InventoryAndTournament)
        {
            LoadInventory();
        }
        else if (OnSceneLoad == Load.Tournament || OnSceneLoad == Load.InventoryAndTournament)
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
    public int SelectedRobot;

    public string backgroundColorHex;
    public string overlayColorHex;
    public float backgroundAlpha = 255;

    public int prefixIndex;
    public string prefixString;
    public int suffixIndex;
    public string suffixString;

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

    public void SavePrefixString(string prefix)
    {
        prefixString = prefix;
        PlayerPrefs.SetString("CurrentPrefixString", prefix);
        PlayerPrefs.Save();
    }
    public void SaveSuffixString(string suffix)
    {
        suffixString = suffix;
        PlayerPrefs.SetString("CurrentSuffixString", suffix);
        PlayerPrefs.Save();
    }

    public bool LoadPrefixString()
    {
        if (PlayerPrefs.HasKey("CurrentPrefixString"))
        {
            prefixString = PlayerPrefs.GetString("CurrentPrefixString", prefixString);
            return true;
        }
        return false;
    }
    public bool LoadSuffixString()
    {
        if (PlayerPrefs.HasKey("CurrentSuffixString"))
        {
            suffixString = PlayerPrefs.GetString("CurrentSuffixString", suffixString);
            return true;
        }

        return false;
    }

    public string GetPlayerName()
    {
        return prefixString + " " + suffixString;
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
        SelectedRobot = 0;
    }

    public void UpdateUseItem()
    {

        foreach (Weapon weapon in Weapons)
        {
            weapon._useNumber = 0;
        }
        foreach (Frame frame in Frames)
        {
            frame._useNumber = 0;
        }

        foreach (Robot robot in Robots)
        {
            foreach (Frame frame in Frames)
            {
                if (frame._id == robot._frame._id)
                    frame._useNumber++;
            }
            if(robot._weapons != null)
            {
                foreach (HookPoint hookPoint in robot._weapons)
                {
                    foreach (Weapon weapon in Weapons)
                    {
                        if (weapon._id == hookPoint._weapon._id)
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

    public List<HookPoint> _weapons = new List<HookPoint>();

    public Robot(Frame frame)
    {
        _frame = frame;
    }

    public void UpdateWeaponMaxList()
    {
        //int max = _frame.GetFrameData().GetNbWeaponMax();
        //List<HookPoint> updateWeapons = new List<HookPoint>();

        //for (int i = 0; i < max; i++)
        //{
        //    foreach (HookPoint weapon in _weapons)
        //    {
        //        if (weapon._hookPointIndex == i)
        //        {
        //            updateWeapons.Add(weapon);
        //        }
        //    }
        //}
        //_weapons = updateWeapons.ToList();

        _weapons.Clear();
    }

    public void AddWeapon(Weapon weapon, int index)
    {
        int replaceIndex = -1;

        for (int i = 0; i < _weapons.Count(); i++)
        {
            if (_weapons[i]._hookPointIndex == index)
            {
                replaceIndex = i;
            }
        }

        if(replaceIndex >= 0)
        {
            this.RemoveWeapon(replaceIndex);
        }

        _weapons.Add(new HookPoint(index, weapon));
    }

    public void RemoveWeapon(int hookPointIndex)
    {
        List<HookPoint> updateWeapons = new List<HookPoint>();
        foreach (HookPoint hookPoint in _weapons)
        {
            if (hookPoint._hookPointIndex != hookPointIndex)
            {
                updateWeapons.Add(hookPoint);
            }
        }
        _weapons = updateWeapons.ToList();
    }

    public Weapon GetHookPointWeapon(int hookPointIndex)
    {
        foreach (HookPoint hookPoint in _weapons)
        {
            if (hookPoint._hookPointIndex == hookPointIndex)
            {
                return hookPoint._weapon;
            }
        }
        return null;
    }

    [System.Serializable]
    public struct HookPoint
    {
        public int _hookPointIndex;
        public Weapon _weapon;

        public HookPoint(int index, Weapon weapon)
        {
            _hookPointIndex = index;
            _weapon = weapon;
        }
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
    public Dictionary<S_TournamentManager.Participant ,Robot> _participantsRobot;
    public S_TournamentManager.Participant _player;
    public List<float> _playerLife;

    public void InitRobot()
    {
        foreach(S_TournamentManager.Participant participant in _participants)
        {
            if (participant.isPlayer)
            {
                _participantsRobot.Add(participant, S_DataGame.Instance.inventory.Robots[S_DataGame.Instance.inventory.SelectedRobot]);
            }
            else
            {
                _participantsRobot.Add(participant, S_DataRobotComponent.Instance.GetRandomRobot());
            }
        }
    }

    public void SavePlayerLife(S_FrameManager frameManager)
    {
        _playerLife.Add(frameManager._life);
        foreach(GameObject hookPoint in frameManager.WeaponHookPoints)
        {
            S_WeaponManager weaponManager = hookPoint.GetComponentInChildren<S_WeaponManager>();
            if (weaponManager != null)
            {
                _playerLife.Add(weaponManager._life);
            }
            else
            {
                _playerLife.Add(0);
            }
        }
    }

    public void SetPlayerLife(S_FrameManager frameManager)
    {
        frameManager._life = _playerLife[0];
        for(int i=0; i < frameManager.WeaponHookPoints.Count();i++)
        {
            GameObject hookPoint = frameManager.WeaponHookPoints[i];
            S_WeaponManager weaponManager = hookPoint.GetComponentInChildren<S_WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager._life = _playerLife[i + 1];
            }
        }
    }
}

