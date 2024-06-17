using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using CartoonFX;


public class S_VFXPlayer : MonoBehaviour
{

    public GameObject[] _effectsList;
    public int _singleVFXIndex = 0; //Index used for playing the single VFX in case of type == Single and playing the little flame in case of type == FlameThrower
    public int _flameThrowerIndex = 1; //Index used for playing the large flame in case of type == FlameThrower


    public enum VFXType
    {
        Single,
        FlameThrower
    }

    [SerializeField] public VFXType _VFXType;

    void Awake()
    {
        GetVFXInChildren();

        if (_VFXType == VFXType.FlameThrower)
            PlayStopEffectAtIndex(_singleVFXIndex, true);
    }


    /// <summary> 
    /// Get every children of the current GameObject and put it in _effectList
    /// </summary> 
    private void GetVFXInChildren()
    {
        var list = new List<GameObject>();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            var effect = this.transform.GetChild(i).gameObject;
            list.Add(effect);

            var cfxrEffect = effect.GetComponent<CFXR_Effect>();
            if (cfxrEffect != null) cfxrEffect.clearBehavior = CFXR_Effect.ClearBehavior.Disable;
        }
        _effectsList = list.ToArray();
    }

    /// <summary> 
    /// Plays or stop the correct effect based on the parameters
    /// </summary> 
    /// <param name="index"> The index of the wanted VFX, either _singleVFXIndex or _flameThrowerIndex </param>
    /// <param name="toPlay"> Tell if the chosen VFX should start playing or stop </param>
    private void PlayStopEffectAtIndex(int index, bool toPlay)
    {
        GameObject _currentEffect = _effectsList[index];
        if (_currentEffect == null)
            return;

        var ps = _currentEffect.GetComponent<ParticleSystem>();

        if (toPlay)
        {
            if (!_currentEffect.gameObject.activeSelf)
            {
                _currentEffect.SetActive(true);
            }
            else
            {
                ps.Play(true);
                var cfxrEffects = _currentEffect.GetComponentsInChildren<CFXR_Effect>();
                foreach (var cfxr in cfxrEffects)
                {
                    cfxr.ResetState();
                }
            }
        }
        else
        {
            if (ps.isEmitting)
            {
                ps.Stop(true);

            }
        }

        if (index == _flameThrowerIndex)
            PlayStopEffectAtIndex(_singleVFXIndex, !toPlay);
    }

    /// <summary> 
    /// Plays the flame thrower VFX
    /// </summary> 
    public void PlayFlameThrowerEffect()
    {
        PlayStopEffectAtIndex(_flameThrowerIndex, true);
    }

    /// <summary> 
    /// Stops the flame thrower VFX
    /// </summary> 
    public void StopFlameThrowerEffect()
    {
        PlayStopEffectAtIndex(_flameThrowerIndex, false);
    }

    /// <summary> 
    /// Plays the single VFX
    /// </summary> 
    public void PlaySingleEffect()
    {
        PlayStopEffectAtIndex(_singleVFXIndex, true);
    }

    /// <summary> 
    /// Stops the single VFX
    /// </summary> 
    public void StopSingleEffect()
    {
        PlayStopEffectAtIndex(_singleVFXIndex, false);
    }

}