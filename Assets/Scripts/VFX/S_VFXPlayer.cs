using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CartoonFX 
{
    public class S_VFXPlayer : MonoBehaviour
    {


        [SerializeField] private GameObject parent; 
        [SerializeField] GameObject[] effectsList;
        public GameObject currentEffect;
        int index = 0;
        private GameObject currentGOVFX;

        void Awake()
        {
            // var list = new List<GameObject>();
            // for (int i = 0; i < this.transform.childCount; i++)
            // {
            //     var effect = this.transform.GetChild(i).gameObject;
            //     list.Add(effect);

            //     var cfxrEffect= effect.GetComponent<CFXR_Effect>();
            //     if (cfxrEffect != null) cfxrEffect.clearBehavior = CFXR_Effect.ClearBehavior.Disable;
            // }
            // effectsList = list.ToArray();

            PlayAtIndex();
        }

        public void PlayAtIndex()
		{
			if (currentGOVFX != null)
			{
                Destroy(currentGOVFX);
			}

			currentEffect = effectsList[index];
			// currentEffect.SetActive(true);
            currentGOVFX = Instantiate(currentEffect, parent.transform);
		}

        void WrapIndex()
            {
                if (index < 0) index = effectsList.Length - 1;
                if (index >= effectsList.Length) index = 0;
            }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("SPACE IS DOWN");
                if (currentEffect != null)
                {
                    var ps = currentGOVFX.GetComponent<ParticleSystem>();
                    Debug.Log(ps.isEmitting);
                    if (ps.isEmitting)
                    {
                        Debug.Log("STOP");
                        ps.Stop(true);
                    }
                    else
                    {
                        Debug.Log("Is not emitting");
                        if (!currentGOVFX.gameObject.activeSelf)
                        {
                            Debug.Log("currentEffect is not active");
                            currentGOVFX.SetActive(true);
                        }
                        else
                        {
                            ps.Play(true);
                            var cfxrEffects = currentEffect.GetComponentsInChildren<CFXR_Effect>();
                            Debug.Log(cfxrEffects);
                            foreach (var cfxr in cfxrEffects)
                            {
                                cfxr.ResetState();
                            }
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (currentGOVFX != null)
                {
                    Destroy(currentGOVFX);
                }
                else 
                {
                    PlayAtIndex();
                }
            }

            if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
            {
                if (currentEffect != null)
                {
                    currentEffect.SetActive(false);
                    currentEffect.SetActive(true);
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PreviousEffect();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                NextEffect();
            }
        }

        public void NextEffect()
        {
            index++;
            WrapIndex();
            PlayAtIndex();
        }

        public void PreviousEffect()
        {
            index--;
            WrapIndex();
            PlayAtIndex();
        }
    }
}