using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreFXController : MonoBehaviour
{
    public GameObject FXTextPrefab;

    

    void Start()
    {
        
    }

    private static IEnumerator DestroyAfter(GameObject game_object, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(game_object);
    }

    private static IEnumerator TrackWorldPosition(RectTransform rt, Vector3 world_pos)
    {
        var camera = rt.GetComponentInParent<Canvas>().worldCamera;
        if (camera == null)
            camera = Camera.main;
        for(; ; )
        {
            var screen_pos = camera.WorldToScreenPoint(world_pos);
            rt.position = screen_pos;
            yield return new WaitForEndOfFrame();
        }
    }

    public FXText Create(string text, Transform target, float ttl=0f)
    {
        var fx_text = Instantiate(FXTextPrefab).GetComponent<FXText>();
        fx_text.transform.SetParent(transform, true);
        fx_text.Value = text;
        if(ttl > 0)
            fx_text.StartCoroutine(DestroyAfter(fx_text.gameObject, ttl));
        if(target != null)
            fx_text.StartCoroutine(TrackWorldPosition((RectTransform)fx_text.transform, target.transform.position));
        return fx_text;
    }
}
