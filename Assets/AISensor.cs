using System.Collections;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

//[ExecuteInEditMode]
public class AISensor : MonoBehaviour
{
  private Enemy enemy;
  public float distance = 10f;
  public float angle = 30f;
  public float height = 1.0f;
  public Color meshColor = Color.red;

  int count;
  Collider[] colliders = new Collider[20];
  public LayerMask layers;
  public LayerMask obstructionMask;

  private bool canSeePlayer = false;
  Mesh mesh;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      enemy = gameObject.GetComponentInParent<Enemy>();
      StartCoroutine(ScanRoutine());
    }

    private IEnumerator ScanRoutine() {
      WaitForSeconds wait = new WaitForSeconds(0.2f);

      while (true) {
        yield return wait;
        Scan();
      }
    }

    private void Scan() {
      count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);
      for(int i = 0; i < count; ++i) {
        GameObject target = colliders[i].gameObject;
        if(IsInSight(target) && target.layer == LayerMask.NameToLayer("Player")) {
          canSeePlayer = true;
          enemy.PlayerSpotted(target.transform);
        } else {
          canSeePlayer = false;
          enemy.NoPlayerFound();
        }
      }
    }

    private bool IsInSight(GameObject target) {
      
      Vector3 origin = transform.position;
      Vector3 dest = target.transform.position;
      Vector3 direction = dest - origin;
      if(direction.y < 0 || direction.y > height) {
        return false;
      }

      direction.y = 0;
      float deltaAngle = Vector3.Angle(direction, transform.forward);
      if(deltaAngle > angle) {
        return false;
      }

      Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
      float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
      if(Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask)) {
        return false;
      }

      return true;
    }

    Mesh CreateWedgeMesh() {
      Mesh mesh = new Mesh();

      int segments = 10;
      int numTriangles = (segments * 4) + 2 + 2;
      int numVertices = numTriangles * 3;

      Vector3[] vertices = new Vector3[numVertices];
      int[] triangles = new int[numVertices];

      Vector3 bottomCenter = Vector3.zero;
      Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
      Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

      Vector3 topCenter = bottomCenter + Vector3.up * height;
      Vector3 topRight = bottomRight + Vector3.up * height;
      Vector3 topLeft = bottomLeft + Vector3.up * height;

      int vert = 0;

      //left side
      vertices[vert++] = bottomCenter;
      vertices[vert++] = bottomLeft;
      vertices[vert++] = topLeft;

      vertices[vert++] = topLeft;
      vertices[vert++] = topCenter;
      vertices[vert++] = bottomCenter;

      //right side
      vertices[vert++] = bottomCenter;
      vertices[vert++] = topCenter;
      vertices[vert++] = topRight;

      vertices[vert++] = topRight;
      vertices[vert++] = bottomRight;
      vertices[vert++] = bottomCenter;

      float currentAngle = -angle;
      float deltaAngle = (angle * 2) / segments;
      for(int i = 0; i < segments; ++i) {
        
        bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
        bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

        topRight = bottomRight + Vector3.up * height;
        topLeft = bottomLeft + Vector3.up * height;

        //far side
        vertices[vert++] = bottomLeft;
        vertices[vert++] = bottomRight;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = topLeft;
        vertices[vert++] = bottomLeft;

        //top
        vertices[vert++] = topCenter;
        vertices[vert++] = topLeft;
        vertices[vert++] = topRight;

        //bottom
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomLeft;

        
        currentAngle += deltaAngle;
      }

     
      for(int i = 0; i < numVertices; ++i) {
        triangles[i] = i;
      }

      mesh.vertices = vertices;
      mesh.triangles = triangles;
      mesh.RecalculateNormals();

      return mesh;
    }

    

  private void OnValidate()
  {
    mesh = CreateWedgeMesh();
  }

  private void OnDrawGizmos()
  {
      if(mesh) {
        Gizmos.color = meshColor;
        Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
      }
    Gizmos.DrawWireSphere(transform.position, distance);
    if(canSeePlayer) {
      Gizmos.color = new Color(0,1,0,0.5f);
    } else {
      Gizmos.color = new Color(0,0,1,0.5f);
    }
    for(int i =0; i < count; ++i) {
      Gizmos.DrawSphere(colliders[i].transform.position, 1f);
    }
  }
}
