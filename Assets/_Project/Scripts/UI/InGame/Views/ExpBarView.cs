using UnityEngine;
using UnityEngine.UI;


namespace SurvivorsLike
{
    public class ExpBarView : MonoBehaviour
    {
        [SerializeField] private Image _expBar;

        public void SetExpRatio(float ratio)
        {
            _expBar.fillAmount = ratio;
        }
    }
}
