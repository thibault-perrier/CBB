using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CartoonFX;

public class S_FXTester : MonoBehaviour
{
    [SerializeField] private S_SFXPlayer _SFXPlayer;
    [SerializeField] private S_VFXPlayer _VFXPlayer;


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_VFXPlayer._VFXType == S_VFXPlayer.VFXType.Single)
            {

                if (!_VFXPlayer._effectsList[_VFXPlayer._singleVFXIndex].GetComponent<ParticleSystem>().isEmitting)
                {
                    _VFXPlayer.PlaySingleEffect();

                    _SFXPlayer.PlayEffect();
                }
                else
                {
                    _VFXPlayer.StopSingleEffect();
                }
            }
            else
            {
                if (!_VFXPlayer._effectsList[_VFXPlayer._flameThrowerIndex].GetComponent<ParticleSystem>().isEmitting)
                {
                    _VFXPlayer.PlayFlameThrowerEffect();

                    _SFXPlayer.PlayEffect();
                }
                else
                {
                    _VFXPlayer.StopFlameThrowerEffect();
                }
            }
        }
    }
}
