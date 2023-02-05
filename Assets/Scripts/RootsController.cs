using System;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

public class RootsController : NetworkBehaviour
{
    [SerializeField]
    private Targeter targeter;

    [SerializeField]
    private ProjectileWeapon weapon;

    [SerializeField]
    private Animator anim = null;

    [SerializeField]
    private Targetable currentTarget = null;

    [SerializeField]
    private AudioSource Audio;

    [SerializeField]
    private List<AudioClip> shootClips = new List<AudioClip>();

    [SerializeField]
    private AudioClip growSound;

    [SyncVar(hook = nameof(OnSlotChanged)), SerializeField]
    private int slot = -1;

    public int Slot => slot;

    [SyncVar(hook = nameof(OnOwnerChanged)), SerializeField]
    private string owner;
    public string Owner => owner;

    public UnityEvent OnValueChanged { get; } = new UnityEvent();

    [SyncVar(hook = nameof(OnKillsChanged))]
    public int Kills = 0;

    [SyncVar]
    private int level = 0;

    [SerializeField]
    private TMP_Text ownerLabel;

    [Server]
    public void SetPlayer(PlayerController player)
    {
        owner = player.Data.username;
    }

    [Server]
    public void SetSlot(int slot)
    {
        this.slot = slot;
    }

    private void OnKillsChanged(int oldValue, int newValue)
    {
        OnValueChanged.Invoke();
    }

    private async void OnSlotChanged(int oldValue, int newValue)
    {
        //Wait a frame to ensure the instance list has had a chance to populate
        await Task.Yield();

        var treeContainer = TreeContainer.GetSlot(newValue);
        if (treeContainer != null)
        {
            treeContainer.Current = this;
        }
        else
        {
            Debug.LogError($"Failed to get tree container for slot {newValue}");
        }
    }

    private void OnOwnerChanged(string oldValue, string newValue)
    {
        ownerLabel.text = newValue;
    }

    [SyncVar]
    private float sproutTimer = 1.5f;

    [SyncVar]
    private float plantTime = 0f;

    private void Start()
    {
        plantTime = Time.time;
        targeter.OnTargetLost.AddListener(TargetRemoved);
        targeter.OnTargetAdded.AddListener(TargetAdded);

        //Check if we have a non-negative slot number on start
        //There is an edge case on instantiate where we might not get the slot number changed hook
        if (isClient && slot != -1)
        {
            var container = TreeContainer.GetSlot(slot);
            if (container.Current == null)
            {
                container.Current = this;
            }
        }
    }

    private void OnDestroy()
    {
        targeter.OnTargetLost.RemoveListener(TargetRemoved);
        targeter.OnTargetAdded.RemoveListener(TargetAdded);
    }

    private void Update()
    {
        if (level < 1 && Time.time > plantTime + sproutTimer)
        {
            level++;
            RPCSetAnimInt("Level", level);
            RPCPlayGrowSound();
        }
        else if(level == 1 && Kills >= 20)
        {
            level++;
            RPCSetAnimInt("Level", level);
            RPCPlayGrowSound();
            weapon.cooldown *= 0.7f;
        }

        if (weapon == null)
        {
            return;
        }

        //Look for a target if we don't have one
        if (currentTarget == null)
        {
            currentTarget = FindTarget();
        }

        //Fire our weapon if it is ready and we have a target
        if (isServer && weapon.IsReady && currentTarget != null)
        {
            weapon.Fire(currentTarget.transform);
            RPCSetAnimBool("Attacking", true);
            RPCPlayAttackSound();
        }
        else if (isServer && anim.GetBool("Attacking"))
        {
            RPCSetAnimBool("Attacking", false);
        }

        weapon.Tick();
        if (isClient)
        {
            anim.SetInteger("Level", level);
        }
    }

    [ClientRpc]
    public void RPCPlayAttackSound()
    {
        if (shootClips.Count > level)
        {
            Audio.PlayOneShot(shootClips[level]);
        }
    }

    [ClientRpc]
    public void RPCPlayGrowSound()
    {
        if (shootClips.Count > level)
        {
            Audio.PlayOneShot(growSound);
        }
    }

    [ClientRpc]
    public void RPCSetAnimBool(string tag, bool value)
    {
        anim.SetBool(tag, value);
    }

    [ClientRpc]
    public void RPCSetAnimInt(string tag, int value)
    {
        anim.SetInteger(tag, value);
    }

    private Targetable FindTarget()
    {
        var target = targeter.GetClosestTarget(transform.position);
        return target == null ? null : target;
    }

    private void TargetAdded(Targetable target)
    {
        if (currentTarget == null)
        {
            currentTarget = FindTarget();
        }
    }

    private void TargetRemoved(Targetable target)
    {
        if (currentTarget == target)
        {
            currentTarget = FindTarget();
        }
    }

    public void OnEnemyDestroyed(EnemyController enemy)
    {
        Kills++;
    }

}
