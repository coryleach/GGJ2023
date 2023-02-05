using System;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;
using TMPro;

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

    [SyncVar(hook = nameof(OnSlotChanged)), SerializeField]
    private int slot;

    public int Slot
    {
        get => slot;
    }

    [SyncVar(hook = nameof(OnOwnerChanged)), SerializeField]
    private string owner;
    public string Owner => owner;

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

    private float sproutTimer = 1.5f;
    private float plantTime = 0f;

    [SyncVar]
    private int level = 0;

    private void Start()
    {
        plantTime = Time.time;
        targeter.OnTargetLost.AddListener(TargetRemoved);
        targeter.OnTargetAdded.AddListener(TargetAdded);
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
        }
        else if (isServer && anim.GetBool("Attacking"))
        {

            RPCSetAnimBool("Attacking", false);
        }

        weapon.Tick();
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

}
