using Unity.VisualScripting;
using UnityEngine;

public class MousePosManager : MonoBehaviour
{

    [SerializeField] private bool resetWorldOnScreen;

    public enum CursorState
    {
        Move, Place, Delete, Play
    }
    [SerializeField] private CursorState cursorState;

    [SerializeField] private Transform placeableMin;
    [SerializeField] private Transform placeableMax;

    [SerializeField] private Transform currentSelectionViewer;

    [Header("Set automatically")]
    [SerializeField] private Vector2 blockPlaceMin;
    [SerializeField] private Vector2 blockPlaceMax;

    [SerializeField] private Vector3 pos;

    [SerializeField] private Vector3 oldMovePos;
    [SerializeField] private Vector3 oldPlayerPos;

    [SerializeField] private PlaceableBlock selectedObj;
    [SerializeField] private GameObject selectedTextButton;

    [SerializeField] private Vector2 worldOnScreenMin;
    [SerializeField] private Vector2 worldOnScreenMax;


    public static MousePosManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetWorldOnScreen();
        oldPlayerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
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
        CheckInput(MTW);

        // Setting position
        SetMouseWorldPos(MTW);

    }

    private void CheckInput(Vector3 pMTW)
    {
        if (Input.GetMouseButtonDown(0))
        {
            switch (cursorState)
            {
                case CursorState.Move: CheckMoveStateInput(pMTW); break;
                case CursorState.Place: CheckPlaceStateInput(); break;
                case CursorState.Delete: CheckDeleteStateInput(pMTW); break;
            }
        }

    }


    private void CheckMoveStateInput(Vector3 pMTW)
    {
        // Already holding something: Place it down
        if (selectedObj != null)
        {
            // If at the view thing: Place at old pos
            if (selectedObj.transform.position == currentSelectionViewer.position)
            {
                selectedObj.transform.position = oldMovePos;
            }
            if (selectedObj.tag == "Player")
            {
                oldPlayerPos = selectedObj.transform.position;
            }
            selectedObj = null;
            return;
        }

        // Not holding something: Try to pick something up
        PlaceableBlock raycastHitPlaceable = DoRaycast(pMTW);
        if (raycastHitPlaceable != null)
        {
            selectedObj = raycastHitPlaceable;
            oldMovePos = selectedObj.transform.position;
        }

    }
    private void CheckPlaceStateInput()
    {
        // If PLACE and obj is not at the view thing: Place a copy
        if (selectedObj != null && selectedObj.transform.position != currentSelectionViewer.position)
        {
            Instantiate(selectedObj, pos, Quaternion.identity);
            return;
        }
    }
    private void CheckDeleteStateInput(Vector3 pMTW)
    {
        PlaceableBlock raycastHitPlaceable = DoRaycast(pMTW);
        if (raycastHitPlaceable != null && !raycastHitPlaceable.isIndestructible)
        {
            Destroy(raycastHitPlaceable.gameObject);
        }

    }
    private PlaceableBlock DoRaycast(Vector3 pPos)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(pPos - Vector3.forward, Vector3.forward, out hitInfo))
        {
            PlaceableBlock pb;
            if (hitInfo.transform != null && hitInfo.transform.TryGetComponent<PlaceableBlock>(out pb))
            {
                return pb;
                //return hitInfo.transform;
            }
        }

        return null;
    }

    private void SetMouseWorldPos(Vector3 pMTW)
    {
        pos.x = Mathf.Round(pMTW.x);
        pos.y = Mathf.Round(pMTW.y);


        if (pos.x < blockPlaceMin.x || pos.x > blockPlaceMax.x ||
            pos.y < blockPlaceMin.y || pos.y > blockPlaceMax.y)
        {
            pos = currentSelectionViewer.position;
        }


        if (selectedObj != null)
        {
            selectedObj.transform.position = pos;
        }
    }

    public CursorState GetCursorState()
    {
        return cursorState;
    }
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
    public void SetCursorStatePlay()
    {
        SetCursorState(CursorState.Play);
    }

    public void SetCursorState(CursorState pCursorState, GameObject pPlaceableObj)
    {
        if (selectedObj != null)
        {
            if (cursorState == CursorState.Move)
            {
                // Holding something: Place back at old position
                selectedObj.transform.position = oldMovePos;
                selectedObj = null;
            }
            else
            {
                // Holding a placeable block: Remove it
                Destroy(selectedObj.gameObject);
                selectedObj = null;
            }
        }

        if (selectedTextButton != null)
        {
            Destroy(selectedTextButton);
            selectedTextButton = null;
        }


        cursorState = pCursorState;

        if (pPlaceableObj != null)
        {
            if (pCursorState == CursorState.Place)
            {
                selectedObj = Instantiate(pPlaceableObj.gameObject, pos, Quaternion.identity).GetComponent<PlaceableBlock>();
            }
            else
            {
                selectedTextButton = Instantiate(pPlaceableObj.gameObject, pos, Quaternion.identity);
            }
        }

        if (pCursorState == CursorState.Play)
        {
            StartPlayMode();
        }
        else
        {
            ResetPlayerPosition();
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

    private void StartPlayMode()
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.transform.position = oldPlayerPos;
        player.GetComponent<Rigidbody>().useGravity = true;
        player.GetComponent<Rigidbody>().isKinematic = false;
        player.enabled = true;
    }

    private void ResetPlayerPosition()
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.transform.position = oldPlayerPos;
        player.GetComponent<Rigidbody>().useGravity = false;
        player.GetComponent<Rigidbody>().isKinematic = true;
        player.enabled = false;
    }
}
