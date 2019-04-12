using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameplayHUDController : MonoBehaviour, ISerializationCallbackReceiver
{
    [Serializable]
    public class Grade
    {
        public string Text;
        public int Points;
        public Sprite Fill;
        public Grade(string text, int points, Sprite fill = null)
        {
            Text = text;
            Points = points;
            Fill = fill;
        }
    }

    [SerializeField]
    private MonospaceTextController _ScoreReadout;

    [SerializeField]
    private MonospaceTextController _TimerReadout;

    [SerializeField]
    private ScoreFXController _ScoreFX;

    [SerializeField]
    private int _Score;
    public int Score
    {
        get { return _Score; }
        set
        {
            _Score = value;
            if (_ScoreReadout != null)
                _ScoreReadout.Text = value.ToString("D" + _ScoreReadout.NumberOfCharacters);
        }
    }

    [SerializeField]
    private float _TimerValue;

    public float TimerValue
    {
        get { return _TimerValue; }
        set
        {
            _TimerValue = value;
            var msec = Mathf.FloorToInt(1000f * value);
            var sec = msec / 1000;
            if(_TimerReadout != null)
                _TimerReadout.Text = $"{sec / 60:00}:{sec % 60:00}:{msec % 1000:000}";
        }
    }


    public Grade[] BonusGrades = new Grade[]
    {
        new Grade("AWESOME", 500),
        new Grade("SUPER", 300),
        new Grade("COOL", 100)
    };

    public void OnScore(int points)
    {

    }

    public void OnBonus(int points)
    {
        var grade = BonusGrades.FirstOrDefault(g => points > g.Points);
        if (grade != null)
        {
            var target = GameObject.FindGameObjectWithTag("Player"); // TODO
            var fx_text = _ScoreFX.Create(grade.Text, target.transform, 3);
            fx_text.FillSprite = grade.Fill;
        }
    }

    public void OnBeforeSerialize()
    {
        Score = Score;
        TimerValue = TimerValue;
    }

    public void OnAfterDeserialize()
    {
    }
}
