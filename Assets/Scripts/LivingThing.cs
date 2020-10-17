using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LivingThing : MonoBehaviour
{
    public class NoiseEventArgs : EventArgs
    {
        public float Distance { get; set; }
        public float Volume { get; set; }
    }
    public delegate void NoiseHeardEventHandler(object sender, NoiseEventArgs args);
    public event NoiseHeardEventHandler NoiseHeard;

    public virtual void TriggerNoiseHeard(object sender, NoiseEventArgs a)
    {
        NoiseHeard?.Invoke(sender, a);
    }

    public int Health { get; private set; }
    // Percentage from 1 to 0
    private float _noiseSensitivity;
    public float NoiseSensitivity {
        get => _noiseSensitivity;
        protected set {
            if (value < 0 || value > 1)
                throw new Exception($"Noise Sensitivity out of bounds: {value}");
            _noiseSensitivity = value;
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0)
        {
            //die
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
