using UnityEngine;

public class MousePosManager : MonoBehaviour
{
    [SerializeField] private Vector2 blockPlaceMin;
    [SerializeField] private Vector2 blockPlaceMax;

    [SerializeField] private Vector3 pos;

    [SerializeField] private Transform selectedObj;

    [SerializeField] private Transform currentSelectionViewer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.mousePosition.x/Screen.width + " " + Input.mousePosition.y/Screen.height);
        if (Input.GetMouseButton(0))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(GetMouseToWorld() - Vector3.forward, Vector3.forward, out hitInfo))
            {
                if (hitInfo.transform != null)
                {
                    selectedObj = hitInfo.transform;
                }
            }
        }
        if (Input.GetMouseButton(1))
        {
            selectedObj = null;
            
        }
        pos.x = Mathf.Round(Input.mousePosition.x / Screen.width * 18);
        pos.y = Mathf.Round(Input.mousePosition.y / Screen.height * 10);

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

    private Vector3 GetMouseToWorld()
    {
        return new Vector3(Input.mousePosition.x / Screen.width * 18, Input.mousePosition.y / Screen.height * 10, 0);
    }
}
