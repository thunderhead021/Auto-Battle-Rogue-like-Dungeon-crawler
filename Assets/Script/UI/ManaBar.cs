using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBar : MonoBehaviour
{
    public Transform bar;
    public Vector3 offset;

    private float maxMana;
    Transform target;

    public void Setup(Transform target, float maxMana)
    {
        this.maxMana = maxMana;
        UpdateBar(0);
        this.target = target;
    }

    public void UpdateBar(float newValue)
    {
        float newScale = newValue / maxMana;
        Vector3 scale = bar.transform.localScale;
        scale.x = newScale;
        bar.transform.localScale = scale;
    }

    private void Update()
    {
        if (target != null)
            this.transform.position = target.position + offset;
    }
}
