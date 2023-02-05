using System;
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

    public int Slot { get; set; }

    [SyncVar(hook = nameof(OnOwnerChanged)), SerializeField]
    private string owner;
    public string Owner => owner;

    [SerializeField] private TMP_Text ownerLabel;


    [Server]
    public void SetPlayer(PlayerController player)
    {
        owner = player.Data.username;
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
        else if(isServer)
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
