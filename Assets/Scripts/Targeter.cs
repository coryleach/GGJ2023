using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TargetEvent : UnityEvent<Targetable> { }

public class Targeter : MonoBehaviour
{
    public float range = 1;

    public TargetEvent OnTargetAdded { get; } = new TargetEvent();
    public TargetEvent OnTargetLost { get; } = new TargetEvent();

    private List<Targetable> _targets = new List<Targetable>();

    private void OnTriggerEnter2D(Collider2D col)
    {
        var targetable = col.gameObject.GetComponent<Targetable>();
        if (targetable == null)
        {
            return;
        }
        AddTarget(targetable);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var targetable = other.gameObject.GetComponent<Targetable>();
        if (targetable == null)
        {
            return;
        }
        RemoveTarget(targetable);
    }

    private void AddTarget(Targetable target)
    {
        OnTargetAdded.Invoke(target);
        _targets.Add(target);
    }

    private void RemoveTarget(Targetable target)
    {
        OnTargetLost.Invoke(target);
        _targets.Remove(target);
    }

    public Targetable GetClosestTarget(Vector3 pt)
    {
        float minDistance = float.MaxValue;
        Targetable closest = null;
        foreach (var target in _targets)
        {
            var dist = (target.transform.position - pt).sqrMagnitude;
            if (dist < minDistance)
            {
                closest = target;
                minDistance = dist;
            }
        }
        return closest;
    }

}
