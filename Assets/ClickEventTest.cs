using UnityEngine;
using UnityEngine.EventSystems;

public class ClickEventTest : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        print("M1s");
    }
}
