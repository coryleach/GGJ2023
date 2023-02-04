using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathNode : MonoBehaviour
{
    [SerializeField]
    private PathNode[] nextNodes = new PathNode[0];

    private static Color SphereColor = new Color(1f,0,0, 0.5f);
    private static Color LineColor = Color.red;
    private static float GizmoNodeSize = 0.1f;

    public Vector3 Position => transform.position;

    private void Start()
    {
        //Purge any null nodes just in case
        nextNodes = nextNodes.Where(x => x != null).ToArray();
    }

    public PathNode NextNode()
    {
        return nextNodes[Random.Range(0,nextNodes.Length)];
    }

    private void OnDrawGizmos()
    {
        var pt = transform.position;

        Gizmos.color = SphereColor;
        Gizmos.DrawSphere(pt, GizmoNodeSize);

        for (var i = 0; i < nextNodes.Length; i++)
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
