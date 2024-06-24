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

        if (robot._weapons == null || robot._weapons.Count() == 0)
            return frame;

        List<GameObject> hookPoits = frame.GetComponent<S_FrameManager>().WeaponHookPoints.ToList();

        for (int i = 0; i < hookPoits.Count(); i++)
        {
            Weapon weapon = robot.GetHookPointWeapon(i);
            if (weapon != null)
            {
                GameObject objWeapon = Instantiate(weapon.GetWeaponData().Prefab);
                objWeapon.transform.parent = hookPoits[i].transform;
                objWeapon.transform.localPosition = Vector3.zero;
                objWeapon.transform.localRotation = Quaternion.identity;
            }
        }

        return frame;
    }

}
