using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameplayScoreController : MonoBehaviour, ISerializationCallbackReceiver
{
    [Serializable]
    public class ScoreEvent : UnityEvent<int> { }

    [SerializeField]
    private GameplayHUDController _HUD;
    public GameplayHUDController HUD
    {
        get { return _HUD; }
        set { _HUD = value; }
    }

    [SerializeField]
    private int _TotalPoints;
    public int TotalPoints
    {
        get { return _TotalPoints; }
        set
        {
            _TotalPoints = value;
            if (HUD != null)
                HUD.Score = value;
        }
    }

    [SerializeField]
    private SpooderMotion _SpooderMotion;

    public SpooderMotion SpooderMotion
    {
        get { return _SpooderMotion; }
        set { _SpooderMotion = value; }
    }

    private float GetAngle()
    {
        return SpooderMotion.Rigidbody.rotation;
    }

    float _AirAngle = 0f;
    float _AngleBonus = 0f;
    float _OnBoardTimeout = 0;

    [SerializeField]
    private ScoreEvent _TotalPointsChanged;

    public event UnityAction<int> TotalPointsChanged
    {
        add { _TotalPointsChanged.AddListener(value); }
        remove { _TotalPointsChanged.RemoveListener(value); }
    }

    [SerializeField]
    private ScoreEvent _ScoredPoints;

    public event UnityAction<int> ScoredPoints
    {
        add { _ScoredPoints.AddListener(value); }
        remove { _ScoredPoints.RemoveListener(value); }
    }

    [SerializeField]
    private ScoreEvent _Bonus;

    public event UnityAction<int> Bonus
    {
        add { _Bonus.AddListener(value); }
        remove { _Bonus.RemoveListener(value);  }
    }

    void Start()
    {
    }

    void FixedUpdate()
    {
        var prestatus = SpooderMotion.GetPreStatus();
        var status = SpooderMotion.GetStatus(prestatus);
        if(prestatus.IsOnBoard)
        {
            if (!status.IsPhysicallySupported)
            {
                var angle_diff = GetAngle() - _AirAngle;
                angle_diff = angle_diff % 90f; // Continuous angle difference
                angle_diff = Mathf.Abs(angle_diff); // Direction don't matter
                angle_diff = Mathf.Min(angle_diff, 3f); // Sanity limit
                var points_f = angle_diff * 5; // Scale up
                var points = Mathf.FloorToInt(points_f);
                TotalPoints += points;
                _AngleBonus += angle_diff;
                _ScoredPoints.Invoke(points);
                _TotalPointsChanged.Invoke(TotalPoints);
            }
            else
            {
                // TODO: parameterized curve (AnimationCurve)
                var bonus = Mathf.CeilToInt(Mathf.Pow(_AngleBonus / 250, 6f) * 500f);
                TotalPoints += bonus;
                _AngleBonus = 0f;
                if (bonus >= 1)
                {
                    _Bonus.Invoke(bonus);
                    _TotalPointsChanged.Invoke(TotalPoints);
                }
            }
        }
        else
        {
            _AngleBonus = 0f;
        }
        _AirAngle = GetAngle();
    }

    public void OnBeforeSerialize()
    {
        TotalPoints = TotalPoints;
    }

    public void OnAfterDeserialize()
    {
    }
}
