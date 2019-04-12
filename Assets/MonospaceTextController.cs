using UnityEngine;
using UnityEngine.UI;

public class MonospaceTextController : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField]
    private Text[] _Characters;

    [SerializeField]
    private string _Text;
    public string Text
    {
        get
        {
            return _Text;
        }
        set
        {
            SetCharacters(value ?? "");
            _Text = value;
        }
    }

    public int NumberOfCharacters
    {
        get { return _Characters.Length; }
    }

    private void SetCharacters(string value)
    {
        if (value.Length > NumberOfCharacters)
            Debug.LogWarning("Text length exceeds number of available characters.\nDisplayed text will be truncated.");
        var length = Mathf.Min(value.Length, NumberOfCharacters);
            int i;
        for (i = 0; i < length; i++)
            _Characters[i].text = value[i].ToString();
        for (; i < _Characters.Length; i++)
            _Characters[i].text = string.Empty;
    }

    public void OnBeforeSerialize()
    {
        _Characters = GetComponentsInChildren<Text>();
        Text = Text;
    }

    public void OnAfterDeserialize()
    {
    }
}
