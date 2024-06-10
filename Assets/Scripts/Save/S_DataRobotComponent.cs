using UnityEngine;
using System.Collections.Generic;


namespace Systems
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Robot/ComponentData")]
    public class S_DataRobotComponent : ScriptableObject
    {
        private static S_DataRobotComponent _instance;
        public List<S_WeaponData> _weaponDatas;
        public List<S_FrameData> _frameDatas;

        public static S_DataRobotComponent Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<S_DataRobotComponent>("RobotComponentData"); 
                }

                return _instance;
            }
        }

       
    }
}