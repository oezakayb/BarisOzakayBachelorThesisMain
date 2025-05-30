using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ARControllerWithoutAR : MonoBehaviour
{
    [Header("References")]
    public Transform target;                
    public Animator robodyAnimator;
    public AudioClip welcomeClip;
    public AudioSource ttsAudioSource;
    public AudioSource lipSyncAudioSource;
    public CanvasGroup buttonsUI;

    [Header("Orbit Settings")]
    public float xSpeed = 60f;               
    public float ySpeed = 60f;            

    [Header("Zoom Settings")]
    public float minDistance = 0.2f;          
    public float maxDistance = 2f;  
    public float zoomSpeed = 5f;   

    private Vector3 lastMousePos;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        
        if (buttonsUI != null) buttonsUI.alpha = 0;
        if (target != null) {
            target.parent.gameObject.SetActive(true);
            transform.LookAt(target);
        }
        
        UpdateNearClipPlane();
    }

    void Start()
    {
        StartCoroutine(BeginSequence());
    }

    IEnumerator BeginSequence()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        if (robodyAnimator    != null) robodyAnimator.Play("Waving");
        if (ttsAudioSource     != null) ttsAudioSource.PlayOneShot(welcomeClip);
        if (lipSyncAudioSource != null) lipSyncAudioSource.PlayOneShot(welcomeClip);

        yield return new WaitForSeconds(welcomeClip.length);

        if (buttonsUI != null) buttonsUI.alpha = 1;
    }

    void Update()
    {
        if (target == null) return;
        
    #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            lastMousePos = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            Orbit(delta.x, delta.y);
            lastMousePos = Input.mousePosition;
        }
    #else
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 d = Input.GetTouch(0).deltaPosition;
            Orbit(d.x, d.y);
        }
    #endif
        
    #if UNITY_EDITOR
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
            Zoom(scroll * zoomSpeed);
    #else
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0), t1 = Input.GetTouch(1);
            Vector2 prev0 = t0.position - t0.deltaPosition;
            Vector2 prev1 = t1.position - t1.deltaPosition;
            float prevDist = Vector2.Distance(prev0, prev1);
            float curDist  = Vector2.Distance(t0.position, t1.position);
            Zoom((curDist - prevDist) * (zoomSpeed * 0.01f));
        }
    #endif
    }

    private void Orbit(float deltaX, float deltaY)
    {
        transform.RotateAround(target.position, Vector3.up,
                               deltaX * xSpeed * Time.deltaTime);
        
        transform.RotateAround(target.position, transform.right,
                               -deltaY * ySpeed * Time.deltaTime);
        
        transform.LookAt(target);
    }

    private void Zoom(float increment)
    {
        Vector3 nextPos = transform.position + transform.forward * increment * Time.deltaTime;
        float nextDist = Vector3.Distance(nextPos, target.position);
        
        if (nextDist >= minDistance && nextDist <= maxDistance)
        {
            transform.position = nextPos;
            UpdateNearClipPlane();
        }
    }

    private void UpdateNearClipPlane()
    {
        float dist = Vector3.Distance(transform.position, target.position);
        
        float t = Mathf.InverseLerp(minDistance, maxDistance, dist);
        
        cam.nearClipPlane = Mathf.Lerp(0.01f, 1f, t);
    }
}
