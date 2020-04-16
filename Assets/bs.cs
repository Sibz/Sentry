using System;
using System.Collections;
using System.Collections.Generic;
using Sibz.Sentry;
using UnityEngine;

public class bs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        new Bootstrap().Initialize("DefaultMyWorld");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
