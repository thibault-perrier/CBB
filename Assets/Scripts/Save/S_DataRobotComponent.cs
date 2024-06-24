using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

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

        public Robot GetRandomRobot()
        {
            Frame frame = new Frame(_frameDatas[Random.Range(0, _frameDatas.Count)]);
            Robot robot = new Robot(frame);
            int nbmaxweapon = frame.GetFrameData().GetNbWeaponMax();
            int randomNbMaxWeapon = Random.Range(1, nbmaxweapon);
            int[] hookPoints = new int[nbmaxweapon];

            List<int> availableIndices = new List<int>();
            
            for (int i = 0; i < nbmaxweapon; i++)
            {
                availableIndices.Add(i);
            }

            
            for (int i = 0; i < randomNbMaxWeapon; i++)
            {
                int randomIndex = Random.Range(0, availableIndices.Count);
                int arrayIndex = availableIndices[randomIndex];
                Weapon weapon = new Weapon(_weaponDatas[Random.Range(0, _weaponDatas.Count)]);
                Debug.Log(weapon._name);
                robot._weapons.Add(new Robot.HookPoint(arrayIndex, weapon));

                
                availableIndices.RemoveAt(randomIndex);
            }

            return robot;
        }

    }
}