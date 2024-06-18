using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_RobotSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GenerateRobotAt(Robot robot,Transform transform)
    {
        S_FrameData frameData = robot._frame.GetFrameData();
        List<S_WeaponData> weaponsData = new List<S_WeaponData>();

        GameObject frame = Instantiate(frameData.Prefab);
        List<GameObject> weapons = new List<GameObject>();

        frame.transform.position = transform.position;
        frame.transform.rotation = transform.rotation;

        if (robot._weapons.Count() == 0)
            return frame;

        List<GameObject> hookPoits = frame.GetComponent<S_FrameManager>().WeaponHookPoints.ToList();

        for (int i = 0; i < robot._weapons.Count(); i++)
        {
            if (robot._weapons[i] != null)
            {
                GameObject weapon = Instantiate(robot._weapons[i].GetWeaponData().Prefab);
                weapon.transform.parent = hookPoits[i].transform;
                weapon.transform.localPosition = Vector3.zero;
                weapon.transform.localRotation = Quaternion.identity;
            }
        }

        return frame;
    }

}
