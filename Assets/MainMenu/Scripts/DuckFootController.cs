using TMPro;
using UnityEngine;

public class DuckFootController : MonoBehaviour
{
    [SerializeField] private GameObject duckFoot;
    private TextMeshProUGUI _textMeshProUGUI;
    
    // Start is called before the first frame update
    private void Awake()
    {
        _textMeshProUGUI = transform.GetComponentInChildren<TextMeshProUGUI>();
    }
    public void EnableDuckFoot()
    {
        duckFoot.SetActive(!duckFoot.activeInHierarchy);
        _textMeshProUGUI.fontStyle = FontStyles.Bold;
        _textMeshProUGUI.color = Color.white;
        if (duckFoot.activeInHierarchy)
        {
            _textMeshProUGUI.fontStyle = FontStyles.Underline | FontStyles.Bold;
            _textMeshProUGUI.color = new Color(255.0f / 255.0f, 157.0f / 255.0f, 58.0f / 255.0f);
        }
    }
}
