using UnityEngine;
using UnityEngine.UI;

public class ScoreAmmeter : MonoBehaviour
{
    [SerializeField] private Image ammeterFillImage;
    public float value;
    private void OnEnable()
    {
        value = 0;
        ammeterFillImage.fillAmount = value;
    }

    public void UpdateCooldown(float Speed)
    {
        value = Mathf.Clamp01(value - Speed * Time.deltaTime);
        ammeterFillImage.fillAmount = value;
    }

    public void Boost(float boostAmount)
    {
        value = Mathf.Clamp01(value + (boostAmount / 100));
        ammeterFillImage.fillAmount = value;
    }
}