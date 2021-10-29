using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A2_FOV : MonoBehaviour
{
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] LayerMask agentMask;

    private int view_angle = 30;
    private int view_dist = 10;
    private int edge_iter = 4;

    private float edge_dist_thrs = 0.5f;

    public MeshFilter mesh_filter;
    private Mesh view_mesh;
    [SerializeField] private GameObject mesh;

    [SerializeField] private Material flashlight_material;

    public static bool detected;

    private struct ViewCastInfo
    {
        public bool hit;
        public Vector3 hit_point;
        public float ray_distance;
        public float ray_angle;

        public ViewCastInfo(bool _hit, Vector3 _hit_point, float _ray_distance, float _ray_angle)
        {
            hit = _hit;
            hit_point = _hit_point;
            ray_distance = _ray_distance;
            ray_angle = _ray_angle;
        }
    }

    private struct EdgeInfo
    {
        public Vector3 point_a;
        public Vector3 point_b;

        public EdgeInfo(Vector3 _point_a, Vector3 _point_b)
        {
            point_a = _point_a;
            point_b = _point_b;
        }
    }

    private void Start()
    {
        view_mesh = new Mesh();
        view_mesh.name = "View Mesh";
        mesh_filter.mesh = view_mesh;
        flashlight_material.color = new Color32(254, 224, 0, 140);
    }

    private void LateUpdate()
    {
        DrawFov();
        DetectAgent();
    }

    private void DrawFov()
    {
        List<Vector3> view_points = new List<Vector3>();
        ViewCastInfo old_viewcast = new ViewCastInfo();

        for (int i = 0; i <= view_angle; i++)
        {
            float angle = mesh.transform.eulerAngles.y - view_angle/2 + i;

            ViewCastInfo viewcast = ViewCast(angle);

            if (i > 0)
            {
                bool threshold_exceeded = Mathf.Abs(old_viewcast.ray_distance - viewcast.ray_distance) > edge_dist_thrs;

                if (old_viewcast.hit != viewcast.hit || (old_viewcast.hit && viewcast.hit && threshold_exceeded))
                {
                    EdgeInfo edge = GetEdge(old_viewcast, viewcast);
                    if(edge.point_a != Vector3.zero)
                    {
                        view_points.Add(edge.point_a);
                    }
                    if (edge.point_b != Vector3.zero)
                    {
                        view_points.Add(edge.point_b);
                    }
                }
            }

            view_points.Add(viewcast.hit_point);
            old_viewcast = viewcast;
        }

        int vertex_count = view_points.Count + 1;
        Vector3[] vertices = new Vector3[vertex_count];
        int[] triangles = new int[(vertex_count - 2) * 3];

        vertices[0] = Vector3.zero;
        for(int i = 0; i < vertex_count - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(view_points[i]);

            if (i < vertex_count - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        view_mesh.Clear();
        view_mesh.vertices = vertices;
        view_mesh.triangles = triangles;
        view_mesh.RecalculateNormals();
    }

    private void DetectAgent()
    {
        flashlight_material.color = new Color32(254, 224, 0, 140);
        Collider[] visibleTarget = Physics.OverlapSphere(transform.position, view_dist, agentMask);

        for(int i = 0; i < visibleTarget.Length; i++)
        {
            Transform target = visibleTarget[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if(Vector3.Angle(transform.forward, dirToTarget) < view_angle / 2)
            {
                if (!Physics.Raycast(mesh.transform.position, dirToTarget, view_dist, obstacleMask) && A3_WalkAway.isWalkingAway == false)
                {
                    detected = true;
                    flashlight_material.color = new Color32(255, 0, 0, 140);
                }
            }
        }
    }

    private Vector3 GetVectorFromAngle(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    private ViewCastInfo ViewCast(float global_angle)
    {
        Vector3 dir = GetVectorFromAngle(global_angle);
        RaycastHit hit;

        if (Physics.Raycast(mesh.transform.position, dir, out hit, view_dist, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, global_angle);
        }
        else
        {
            return new ViewCastInfo(false, mesh.transform.position + dir * view_dist, view_dist, global_angle);
        }
    }

    private EdgeInfo GetEdge(ViewCastInfo min_viewcast, ViewCastInfo max_viewcast)
    {
        float min_angle = min_viewcast.ray_angle;
        float max_angle = max_viewcast.ray_angle;
        Vector3 min_point = Vector3.zero;
        Vector3 max_point = Vector3.zero;

        for(int i = 0; i < edge_iter; i++)
        {
            float angle = (min_angle + max_angle) / 2;
            ViewCastInfo viewcast = ViewCast(angle);
            bool threshold_exceeded = Mathf.Abs(min_viewcast.ray_distance - viewcast.ray_distance) > edge_dist_thrs;

            if (viewcast.hit == min_viewcast.hit && !threshold_exceeded)
            {
                min_angle = angle;
                min_point = viewcast.hit_point;
            }
            else
            {
                max_angle = angle;
                max_point = viewcast.hit_point;
            }
        }

        return new EdgeInfo(min_point, max_point);
    }

}
