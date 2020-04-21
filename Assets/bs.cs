using System;
using System.Collections;
using System.Collections.Generic;
using Sibz.Sentry;
using Unity.Entities;
using UnityEngine;

public class bs : MonoBehaviour
{
    private void OnEnable()
    {
        new Bootstrap().Initialize("DefaultMyWorld");
    }

    private void OnDestroy()
    {

    }
}