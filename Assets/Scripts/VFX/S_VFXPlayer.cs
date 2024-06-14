using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CartoonFX
{
    public class S_VFXPlayer : MonoBehaviour
    {
        private GameObject[] _effectsList;
        private int _singleVFXIndex = 0;
        private int _flameThrowerIndex = 1;


        public enum VFXType
        {
            Single,
            FlameThrower
        }

        [SerializeField] private VFXType _VFXType;

        void Awake()
        {
            GetVFXInChildren();

            if (_VFXType == VFXType.FlameThrower)
                PlayStopEffectAtIndex(_singleVFXIndex, true);
        }

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
        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                int index = (_VFXType == VFXType.Single) ? _singleVFXIndex : _flameThrowerIndex;
                PlayStopEffectAtIndex(index, !_effectsList[index].GetComponent<ParticleSystem>().isEmitting);
            }


            if (Input.GetKeyDown(KeyCode.Return))
            {
                int index = (_VFXType == VFXType.Single) ? _singleVFXIndex : _flameThrowerIndex;
                PlayStopEffectAtIndex(index, true);
            }
            if (Input.GetKeyUp(KeyCode.Return))
            {
                Debug.Log("Key is released");
                int index = (_VFXType == VFXType.Single) ? _singleVFXIndex : _flameThrowerIndex;
                PlayStopEffectAtIndex(index, false);
            }
        }

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


        public void PlayFlameThrowerEffect()
        {
            PlayStopEffectAtIndex(_flameThrowerIndex, true);
        }

        public void StopFlameThrowerEffect()
        {
            PlayStopEffectAtIndex(_flameThrowerIndex, false);
        }

        public void PlaySingleEffect()
        {
            PlayStopEffectAtIndex(_singleVFXIndex, true);
        }

        public void StopSingleEffect()
        {
            PlayStopEffectAtIndex(_singleVFXIndex, false);
        }

    }
}