using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    private float _Value = 100;
    public float Value
    {
        get
        {
            return _Value;
        }        
        set
        {                                               
            _Value = Mathf.Clamp(value, 0, MaxValue);

            HealthChanged?.Invoke();
            if (_Value == 0) HealthIsZero?.Invoke();
            else if (_Value == MaxValue) HealthIsMax?.Invoke();
        }
    }
    public float MaxValue = 100;

    public event UnityAction HealthChanged;
    public event UnityAction HealthIsZero;
    public event UnityAction HealthIsMax;
}
