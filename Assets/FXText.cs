using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FXText : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField]
    private string _Value;

    public string Value
    {
        get { return _Value; }
        set
        {
            _Value = value;
            foreach (var text in _Texts)
                text.text = value ?? "";
        }
    }

    [SerializeField]
    private Sprite _FillSprite;

    public Sprite FillSprite
    {
        get { return _FillSprite; }
        set
        {
            _FillSprite = value;
            if (_Fill != null)
                _Fill.sprite = value;
        }
    }

    [SerializeField]
    private Text[] _Texts = new Text[] { };

    [SerializeField]
    private Image _Fill;
    void Start()
    {

    }

    public void OnBeforeSerialize()
    {
        Value = Value;
        FillSprite = FillSprite;
    }

    public void OnAfterDeserialize()
    {
    }
}
