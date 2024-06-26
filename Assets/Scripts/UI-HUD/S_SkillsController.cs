using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class S_SkillsController : MonoBehaviour
{
    [System.Serializable]
    public class S_Skill
    {
        [HideInInspector]
        public S_WeaponManager Weapon;
        public Image WeaponIcon;
        public Image WeaponMask;
    }

    [Header("Skills")]
    [SerializeField]
    private S_Skill skillUp;
    [SerializeField]
    private S_Skill skillDown;
    [SerializeField]
    private S_Skill skillLeft;
    [SerializeField]
    private S_Skill skillRight;

    private S_FrameManager _frameManager;
    private bool _isBind = false;

    private void OnDisable()
    {
        StopAllCoroutines();
        DisableAllListener();
    }

    private void DisableAllListener()
    {
        if (!_isBind)
            return;

        List<S_Skill> skills = new()
        {
            skillLeft,
            skillUp,
            skillRight,
            skillDown,
        };
        List<UnityAction> actionCooldown = new()
        {
            ListenerCooldownMashLeft,
            ListenerCooldownMashUp,
            ListenerCooldownMashRight,
            ListenerCooldownMashDown,
        };
        List<UnityAction> actionDestroy = new()
        {
            ListenerDestroyMashLeft,
            ListenerDestroyMashUp,
            ListenerDestroyMashRight,
            ListenerDestroyMashDown
        };

        foreach (var skill in skills.Select((value, index) => new { index, value }))
        {
            var currentSkill = skill.value;

            if (skill.index <= _frameManager.Weapons.Count - 1)
            {
                currentSkill.Weapon = _frameManager.Weapons[skill.index];
                currentSkill.WeaponIcon.sprite = currentSkill.Weapon.Data.WeaponSrite;

                currentSkill.Weapon.AttackingEnd.RemoveListener(actionCooldown[skill.index]);
                currentSkill.Weapon.AttackingStart.RemoveListener(actionDestroy[skill.index]);
                currentSkill.Weapon.WeaponUnuseable.RemoveListener(actionDestroy[skill.index]);
            }
            else
            {
                currentSkill.WeaponIcon.gameObject.SetActive(false);
            }
        }

        _isBind = false;
    }
    public void InitializeSkills(S_FrameManager frameManager)
    {
        if (_isBind)
            return;

        _frameManager = frameManager;

        List<S_Skill> skills = new()
        {
            skillLeft,
            skillUp,
            skillRight,
            skillDown,
        };
        List<UnityAction> actionCooldown = new()
        {
            ListenerCooldownMashLeft,
            ListenerCooldownMashUp,
            ListenerCooldownMashRight,
            ListenerCooldownMashDown,
        };
        List<UnityAction> actionDestroy = new()
        {
            ListenerDestroyMashLeft,
            ListenerDestroyMashUp,
            ListenerDestroyMashRight,
            ListenerDestroyMashDown
        };

        foreach (var skill in skills.Select((value, index) => new { index, value }))
        {
            var currentSkill = skill.value;

            if (skill.index <= frameManager.Weapons.Count - 1)
            {
                currentSkill.Weapon = frameManager.Weapons[skill.index];
                currentSkill.WeaponIcon.sprite = currentSkill.Weapon.Data.WeaponSrite;
                currentSkill.Weapon.AttackingEnd.AddListener(actionCooldown[skill.index]);
                currentSkill.Weapon.AttackingStart.AddListener(actionDestroy[skill.index]);
                currentSkill.Weapon.WeaponUnuseable.AddListener(actionDestroy[skill.index]);
            }
            else
            {
                currentSkill.WeaponIcon.gameObject.SetActive(false);
            }
        }
        _isBind = true;
    }
    public void ResetSkills()
    {
        List<S_Skill> skills = new()
        {
            skillLeft,
            skillUp,
            skillRight,
            skillDown,
        };

        foreach (var skill in skills)
        {
            skill.WeaponMask.fillAmount = 0f;
        }
    }

    private void ListenerCooldownMashUp()
    {
        StartCoroutine(CooldownMask(skillUp));
    }
    private void ListenerCooldownMashDown()
    {
        StartCoroutine(CooldownMask(skillDown));
    }
    private void ListenerCooldownMashRight()
    {
        StartCoroutine(CooldownMask(skillRight));
    }
    private void ListenerCooldownMashLeft()
    {
        StartCoroutine(CooldownMask(skillLeft));
    }

    private void ListenerDestroyMashUp()
    {
        StartCoroutine(DestroyMask(skillUp));
    }
    private void ListenerDestroyMashDown()
    {
        StartCoroutine(DestroyMask(skillDown));
    }
    private void ListenerDestroyMashRight()
    {
        StartCoroutine(DestroyMask(skillRight));
    }
    private void ListenerDestroyMashLeft()
    {
        StartCoroutine(DestroyMask(skillLeft));
    }

    private IEnumerator CooldownMask(S_Skill skill)
    {
        float timeCooldown = skill.Weapon.Data.AttackCooldown;
        Image weaponMask = skill.WeaponMask;
        float timer = timeCooldown;
        weaponMask.fillAmount = 1f;

        while (timer > 0f)
        {
            weaponMask.fillAmount = timer / timeCooldown;
            timer -= Time.deltaTime;
            yield return null;
        }

        weaponMask.fillAmount = 0f;
        yield return null;
    }
    private IEnumerator DestroyMask(S_Skill skill)
    {
        Image weaponMask = skill.WeaponMask;
        weaponMask.fillAmount = 1f;

        yield return null;
    }

    private bool IsEventAlreadyRegistered(UnityEvent unityEvent, MonoBehaviour target, string method)
    {
        int eventCount = unityEvent.GetPersistentEventCount();

        for (int i = 0; i < eventCount; i++)
        {
            if (unityEvent.GetPersistentTarget(i) == target &&
                unityEvent.GetPersistentMethodName(i) == method)
            {
                return true;
            }
        }

        return false;
    }
}
