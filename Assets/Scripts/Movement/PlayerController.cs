using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private CustomActions input;
    
    [Header("Movement")]
    private float lookRotationSpeed = 40f;
    public Vector3 destination;
    private bool turning = false;

    [Header("Interaction")] [SerializeField]
    private Transform _interactionPoint;
    private float _interactionPointRadius = 0.25f;
    [SerializeField] private LayerMask _interactableMask;
    private readonly Collider[] _colliders = new Collider[1];
    private int _numFound;

    [Header("Door")] public GameObject glassDoorPoint;
    public GameObject roomDoorPoint;



    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        input = new CustomActions();
        AssignInputs();
        agent.updateRotation = false;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != agent.destination)
        {
            if (FaceTarget(agent.steeringTarget) != 0f)
            {
                turning = true;
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
                turning = false;
            }
        }
        
        else if(transform.position != destination)
        {
            if (FaceTarget(destination) != 0f)
            {
                turning = true;
            }
            else
            {
                turning = false;
            }
        }

        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders,
            _interactableMask);

        if (_numFound == 1 && !turning && !agent.hasPath)
        {
            _colliders[0].gameObject.transform.parent = transform;
        }

    }

    void AssignInputs()
    {
        input.Main.Move.performed += _ => ClickToMove();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }

    void ClickToMove()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
        {
            
            
            destination = hit.point;
            GameObject hitObject = hit.collider.gameObject;
            Debug.Log(hitObject.tag);
            if (hitObject.CompareTag("Surface")) 
            {
                agent.destination = hit.point;
                Debug.Log(hit.point);
                
            }
            
            else if (hitObject.CompareTag("Grabable"))
            {
                destination = hitObject.transform.position;
                if (NavMesh.SamplePosition(hit.point, out var navMeshHit, 2 * agent.height,NavMesh.AllAreas))
                {
                    
                    agent.destination = navMeshHit.position;
                }
            }

            else if (hitObject.CompareTag("GlassDoor"))
            {
                agent.destination = glassDoorPoint.transform.position;
                destination = glassDoorPoint.transform.position;
            }
            
            else if (hitObject.CompareTag("RoomDoor"))
            {
                agent.destination = roomDoorPoint.transform.position;
                destination = roomDoorPoint.transform.position;
            }
            
            else if (hitObject.CompareTag("Table"))
            {
                destination = hitObject.transform.position;
                if (NavMesh.SamplePosition(hit.point, out var navMeshHit, 2 * agent.height,NavMesh.AllAreas))
                {
                    
                    agent.destination = navMeshHit.position;
                }
            }
            
        }
    }

    float FaceTarget(Vector3 target)
    {
        float rotationLeft = 0f;
        Vector3 pos = transform.position;

        Vector3 direction = (target - transform.position).normalized;
        Vector3 lookRotationViewingVector = new Vector3(direction.x, 0, direction.z);
        if(lookRotationViewingVector != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookRotationViewingVector);
            var rotation =
                Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
            transform.rotation = rotation;
            rotationLeft = Quaternion.Angle(rotation, lookRotation);
        }

        transform.position = pos;
        
        return rotationLeft;
    }
    
}
