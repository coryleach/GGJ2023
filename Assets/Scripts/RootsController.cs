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

    private Targetable currentTarget = null;

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

    private void Start()
    {
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
        }

        weapon.Tick();
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
