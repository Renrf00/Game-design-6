using Unity.VisualScripting;
using UnityEngine;

public class MousePosManager : MonoBehaviour
{

    [SerializeField] private bool resetWorldOnScreen;

    [SerializeField] private Transform placeableMin;
    [SerializeField] private Transform placeableMax;

    public enum CursorState
    {
        Move, Place, Delete
    }

    [SerializeField] private CursorState cursorState;

    [Header("Set automatically")]
    [SerializeField] private Vector2 blockPlaceMin;
    [SerializeField] private Vector2 blockPlaceMax;

    [SerializeField] private Vector3 pos;

    [SerializeField] private Vector3 oldMovePos;

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
        CheckInput();

        /*if (Input.GetMouseButton(0))
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
            
        }*/


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

    private void CheckInput()
    {
        Vector3 MTW = GetMouseToWorld();

        if (Input.GetMouseButtonDown(0))
        {
            // If MOVE and Holding obj: Place it 
            if (cursorState == CursorState.Move && selectedObj != null)
            {
                // If at the view thing: Place at old pos
                if (selectedObj.position == currentSelectionViewer.position)
                {
                    selectedObj.position = oldMovePos;
                }
                selectedObj = null;
                return;
            }

            // If PLACE and obj is not at the view thing: Place a copy
            if (cursorState == CursorState.Place && selectedObj.position != currentSelectionViewer.position)
            {
                Instantiate(selectedObj, pos, Quaternion.identity);
                return;
            }

            RaycastHit hitInfo;
            if (Physics.Raycast(MTW - Vector3.forward, Vector3.forward, out hitInfo))
            {
                if (hitInfo.transform != null && hitInfo.transform.GetComponent<PlaceableBlock>() != null)
                {

                    if (cursorState == CursorState.Move)
                    {
                        selectedObj = hitInfo.transform;
                        oldMovePos = selectedObj.position;
                    }
                    if (cursorState == CursorState.Delete)
                    {
                        Destroy(hitInfo.transform.gameObject);
                    }

                }
            }
        }

    }

    /*
    private void CheckMoveStateInput()
    {
        Vector3 MTW = GetMouseToWorld();

        if (selectedObj != null)
        {
            // If at the view thing: Place at old pos
            if (selectedObj.position == currentSelectionViewer.position)
            {
                selectedObj.position = oldMovePos;
            }
            selectedObj = null;
            return;
        }

        RaycastHit hitInfo;
        if (Physics.Raycast(MTW - Vector3.forward, Vector3.forward, out hitInfo))
        {
            if (hitInfo.transform != null && hitInfo.transform.GetComponent<PlaceableBlock>() != null)
            {
                selectedObj = hitInfo.transform;
                oldMovePos = selectedObj.position;
                
            }
        }
    }

    private Transform DoRaycast(Vector3 pPos)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(pPos - Vector3.forward, Vector3.forward, out hitInfo))
        {
            if (hitInfo.transform != null && hitInfo.transform.GetComponent<PlaceableBlock>() != null)
            {
                selectedObj = hitInfo.transform;
                oldMovePos = selectedObj.position;

            }
        }

        return null;
    }
    /**/

    public void SetCursorState(CursorState pCursorState)
    {
        SetCursorState(pCursorState, null);
    }

    public void SetCursorStateMove()
    {
        SetCursorState(CursorState.Move);
    }
    public void SetCursorStatePlace(GameObject pPlaceableObj)
    {
        SetCursorState(CursorState.Place, pPlaceableObj);
    }
    public void SetCursorStateDelete()
    {
        SetCursorState(CursorState.Delete);
    }

    public void SetCursorState(CursorState pCursorState, GameObject pPlaceableObj)
    {
        if (selectedObj != null)
        {
            if (cursorState == CursorState.Move)
            {
                selectedObj.position = oldMovePos;
                selectedObj = null;
            }
            else
            {
                Destroy(selectedObj.gameObject);
            }
        }
        

        cursorState = pCursorState;

        if (pPlaceableObj != null) 
        {
            selectedObj = Instantiate(pPlaceableObj, pos, Quaternion.identity).transform;
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
