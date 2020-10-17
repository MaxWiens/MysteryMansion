﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LivingThing : MonoBehaviour
{
    public enum SoundSource { Human, Monster, Environment }
    public class NoiseEventArgs : EventArgs
    {
        public float Volume { get; set; }
        public SoundSource Source { get; set; }
    }
    public delegate void NoiseHeardEventHandler(LivingThing sender, NoiseEventArgs args);
    protected event NoiseHeardEventHandler NoiseHeard;

    public virtual void TriggerNoiseHeard(LivingThing sender, NoiseEventArgs a)
    {
        NoiseHeard?.Invoke(sender, a);
    }

    protected void MakeNoise(float volume, SoundSource source)
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 60f, 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Monster"));
        List<LivingThing> lts = new List<LivingThing>();
        foreach (Collider c in cols)
        {
            LivingThing lt = c.GetComponentInParent<LivingThing>();
            if (lt != null && lt != this)
                lts.Add(lt);
        }

        foreach(LivingThing lt in lts)
        {
            float distance = Vector3.Distance(transform.position, lt.transform.position);
            lt.TriggerNoiseHeard(this, new NoiseEventArgs() { Source = source, Volume = (volume * lt.NoiseSensitivity) / (distance * distance / 8) });
            //lt.TriggerNoiseHeard(this, new NoiseEventArgs() { Source = source, Volume = (volume * lt.NoiseSensitivity) / (distance / 2) });
        }
    }

    public int Health { get; private set; }
    // Percentage from 1 to 0
    private float _noiseSensitivity = 1;
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
}
