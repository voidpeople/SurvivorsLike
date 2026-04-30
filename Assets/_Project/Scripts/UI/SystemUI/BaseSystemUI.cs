using UnityEngine;

public class BaseSystemUI : MonoBehaviour
{
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected GameObject root;

    public virtual void Show(int order)
    {
        //overrideSorting = true
        //이 Canvas는 독립적인 렌더링 그룹
        //부모 Canvas 영향 무시
        //sortingOrder가 직접 적용됨
        canvas.overrideSorting = true;
        canvas.sortingOrder = order;
        root.SetActive(true);
    }

    public virtual void Hide()
    {
        root.SetActive(false);
    }
}
