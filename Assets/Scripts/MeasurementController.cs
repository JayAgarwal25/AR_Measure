using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(ARRaycastManager))]
public class MeasurementController : MonoBehaviour
{
    [SerializeField]
    private float measurementFactor = 1f;

    [SerializeField]
    private GameObject measurementPointPrefab;

    [SerializeField]
    private Vector3 offsetMeasurement = Vector3.zero;

    [SerializeField]
    private GameObject startPanel;

    [SerializeField]
    private Button startButton;

    [SerializeField]
    private TextMeshPro distanceText;

    [SerializeField]
    private ARCameraManager arCameraManager;
    
    private LineRenderer measureLine;

    private ARRaycastManager arRaycastManager;

    private GameObject startPoint;

    private GameObject endPoint;

    private Vector2 touchPosition = default;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public TMP_Text distance;
    public TMP_Text startPointC;
    public TMP_Text endPointC;

    void Awake() 
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        
        startPoint = Instantiate(measurementPointPrefab, Vector3.zero, Quaternion.identity);
        endPoint = Instantiate(measurementPointPrefab, Vector3.zero, Quaternion.identity);
   
        measureLine = GetComponent<LineRenderer>();
        
        startPoint.SetActive(true);
        endPoint.SetActive(false);
        startPanel.SetActive(true);
        startButton.onClick.AddListener(Startbtn);
    

    }

    private void Startbtn() 
    {
        startPanel.SetActive(false);
        Debug.Log("The button was pressed");
    }

    private void OnEnable() 
    {
        if(measurementPointPrefab == null)
        {
            Debug.LogError("measurementPointPrefab must be set");
            enabled = false;
        }    
    }
    
    void Update()
    { 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
           

        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                touchPosition = touch.position;

                if(arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    startPoint.SetActive(true);

                    Pose hitPose = hits[0].pose;
                    startPoint.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
                }
            }

            if(touch.phase == TouchPhase.Moved)
            {
                touchPosition = touch.position;
                
                if(arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    measureLine.gameObject.SetActive(true);
                    endPoint.SetActive(true);

                    Pose hitPose = hits[0].pose;
                    endPoint.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
                }
            }
        }

        if(startPoint.activeSelf && endPoint.activeSelf)
        {
            distanceText.transform.position = endPoint.transform.position + offsetMeasurement;
            distanceText.transform.rotation = endPoint.transform.rotation;
            measureLine.SetPosition(0, startPoint.transform.position);
            measureLine.SetPosition(1, endPoint.transform.position);
         
            startPointC.text = $"Start Point: {(startPoint.transform.position + new Vector3(0f, 0f, 0f)).ToString("F2")}";
            endPointC.text = $"End Point: {(endPoint.transform.position + new Vector3(0f, 0f, 0f)).ToString("F2")}";
            distanceText.text = $"Distance: {(Vector3.Distance(startPoint.transform.position, endPoint.transform.position) * measurementFactor).ToString("F2")} m";
            distance.text = $"Distance: {(Vector3.Distance(startPoint.transform.position, endPoint.transform.position) * measurementFactor).ToString("F2")} m";
           

        }
    }
}
