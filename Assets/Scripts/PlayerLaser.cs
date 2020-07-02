﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaser : MonoBehaviour
{
    [SerializeField] float damage = 100f;

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    public float GetDamage() => damage;

    public void Hit()
    {
        Destroy(gameObject);
    }
}
