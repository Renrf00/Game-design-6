using UnityEngine;
using UnityEngine.Events;

public class WorldSpaceButton : MonoBehaviour
{

    [SerializeField] private MousePosManager.CursorState cursorState;
    [SerializeField] private GameObject buttonObj;

    [SerializeField] private GameObject selectedHighlight;

    public static UnityEvent onThingSelected = new UnityEvent();

    private void Start()
    {
        onThingSelected.AddListener(RemoveHighlight);

        if (buttonObj != null && isPrefab(buttonObj))
        {
            Instantiate(buttonObj, transform);
        }
    }

    private void RemoveHighlight()
    {
        selectedHighlight.SetActive(false);
    }

    private void OnMouseDown()
    {
        onThingSelected?.Invoke();
        selectedHighlight.SetActive(true);
        MousePosManager.Instance.SetCursorState(cursorState, buttonObj);
    }


    bool isPrefab(GameObject go)
    {
        return go.scene == null || go.scene.name == go.name
            || go.scene.name == null || go.scene.name == "";
    }
}
