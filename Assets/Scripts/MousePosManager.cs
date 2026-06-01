using UnityEngine;

public class MousePosManager : MonoBehaviour
{

    [SerializeField] private bool resetWorldOnScreen;

    [SerializeField] private Transform placeableMin;
    [SerializeField] private Transform placeableMax;

    [Header("Set automatically")]
    [SerializeField] private Vector2 blockPlaceMin;
    [SerializeField] private Vector2 blockPlaceMax;

    [SerializeField] private Vector3 pos;

    [SerializeField] private Transform selectedObj;

    [SerializeField] private Transform currentSelectionViewer;


    [SerializeField] private Vector2 worldOnScreenMin;
    [SerializeField] private Vector2 worldOnScreenMax;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetWorldOnScreen();
    }

    // Update is called once per frame
    void Update()
    {
        if (resetWorldOnScreen)
        {
            ResetWorldOnScreen();
        }
        Vector3 MTW = GetMouseToWorld();

        // Inputs
        if (Input.GetMouseButton(0))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(MTW - Vector3.forward, Vector3.forward, out hitInfo))
            {
                if (hitInfo.transform != null && hitInfo.transform.GetComponent<PlaceableBlock>() != null)
                {
                    selectedObj = hitInfo.transform;
                }
            }
        }
        if (Input.GetMouseButton(1))
        {
            selectedObj = null;
            
        }


        // Setting position
        pos.x = Mathf.Round(MTW.x);
        pos.y = Mathf.Round(MTW.y);


        if (pos.x < blockPlaceMin.x || pos.x > blockPlaceMax.x ||
            pos.y < blockPlaceMin.y || pos.y > blockPlaceMax.y)
        {
            pos = currentSelectionViewer.position;
        }


        if (selectedObj != null) 
        {
            selectedObj.position = pos;
        }

    }

    // Recalibrate the camera to make the mouse position match world position
    private void ResetWorldOnScreen()
    {
        Vector3 camPos = Camera.main.transform.position;
        float heightOrthoSize = Camera.main.orthographicSize;
        float widthOrthoSize = heightOrthoSize * Screen.width / Screen.height;
        worldOnScreenMin = new Vector2(camPos.x - widthOrthoSize, camPos.y - heightOrthoSize);
        worldOnScreenMax = new Vector2(camPos.x + widthOrthoSize, camPos.y + heightOrthoSize);

        if (placeableMin == null || placeableMax == null)
        {
            Debug.LogError("Min and/or Max placement position isn't defined");
            return;
        }
        blockPlaceMin = new Vector2(placeableMin.position.x, placeableMin.position.y);
        blockPlaceMax = new Vector2(placeableMax.position.x, placeableMax.position.y);
    }

    private Vector3 GetMouseToWorld()
    {
        float xPos = Mathf.Lerp(worldOnScreenMin.x, worldOnScreenMax.x, Input.mousePosition.x / Screen.width);
        float yPos = Mathf.Lerp(worldOnScreenMin.y, worldOnScreenMax.y, Input.mousePosition.y / Screen.height);
        return new Vector3(xPos, yPos, 0);
    }
}
