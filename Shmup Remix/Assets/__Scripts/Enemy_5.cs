using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_5 : Enemy
{
    void Start()
    {
        materials = Utils.GetAllMaterials(gameObject);
        foreach (Material m in materials)
        {
            m.color = new Color32(75, 192, 214, 255);
        }
    }
}
