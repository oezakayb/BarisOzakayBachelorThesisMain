using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TablePoint : MonoBehaviour
{
    private float _interactionPointRadius = 0.25f;
    [SerializeField] private LayerMask _interactableMask;
    private readonly Collider[] _colliders = new Collider[1];
    private int _numFound;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(transform.position, _interactionPointRadius, _colliders,
            _interactableMask);

        if (_numFound == 1)
        {
            var child = FindChildWithTag(_colliders[0].gameObject.transform.parent.gameObject, "Grabable");
            if (child != null)
            {
                child.transform.position = transform.position;
                            child.transform.parent = transform;
            }
            
        }
        
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _interactionPointRadius);
    }
    
    GameObject FindChildWithTag(GameObject parent, string tag) {
        GameObject child = null;
 
        foreach(Transform transform in parent.transform) {
            if(transform.CompareTag(tag)) {
                child = transform.gameObject;
                break;
            }
        }
 
        return child;
    }
}
