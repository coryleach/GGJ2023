using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathNode : MonoBehaviour
{
    [SerializeField]
    private List<PathNode>  nextNodes = new List<PathNode>();

    [SerializeField]
    private List<PathNode> previousNodes = new List<PathNode>();

    private static Color SphereColor = new Color(1f,0,0, 0.5f);
    private static Color LineColor = Color.red;
    private static float GizmoNodeSize = 0.1f;

    public Vector3 Position => transform.position;

    private void Start()
    {
        //Purge any null nodes just in case
        nextNodes = nextNodes.Where(x => x != null).ToList();
    }

    public PathNode NextNode()
    {
        return nextNodes[Random.Range(0,nextNodes.Count)];
    }

    [ContextMenu("SetupPrevNodes")]
    public void AddPreviousNodes()
    {
        var hashSet = new HashSet<PathNode> {this};
        AddSelfAsPreviousToNext();
        _AddPreviousNodes(this, hashSet);
    }

    public void GetAllNodesInPath(HashSet<PathNode> visited)
    {
        foreach (var nextNode in nextNodes)
        {
            if (visited.Contains(nextNode))
            {
                continue;
            }
            visited.Add(nextNode);
            nextNode.GetAllNodesInPath(visited);
        }
    }

    [ContextMenu("SwapDirection")]
    public void SwapAllDirections()
    {
        var hashSet = new HashSet<PathNode>();
        GetAllNodesInPath(hashSet);
        foreach (var node in hashSet)
        {
            node.SwapDirection();
        }
    }

    public void SwapDirection()
    {
        (previousNodes, nextNodes) = (nextNodes, previousNodes);
    }

    private void _AddPreviousNodes(PathNode node, HashSet<PathNode> visited)
    {
        foreach (var nextNode in node.nextNodes)
        {
            if (visited.Contains(nextNode))
            {
                continue;
            }
            visited.Add(nextNode);
            nextNode.AddSelfAsPreviousToNext();
            _AddPreviousNodes(nextNode, visited);
        }
    }

    public void AddSelfAsPreviousToNext()
    {
        foreach (var next in nextNodes)
        {
            if (next != null && !next.previousNodes.Contains(this))
            {
                next.previousNodes.Add(this);
            }
        }
    }

    private void OnDrawGizmos()
    {
        var pt = transform.position;

        Gizmos.color = SphereColor;
        Gizmos.DrawSphere(pt, GizmoNodeSize);

        for (var i = 0; i < nextNodes.Count; i++)
        {
            if (nextNodes[i] == null)
            {
                continue;
            }

            var nextPt = nextNodes[i].transform.position;
            Gizmos.color = LineColor;
            Gizmos.DrawLine(pt,nextPt);
        }
    }

}
