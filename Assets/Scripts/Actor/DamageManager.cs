using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health), typeof(Armor))]
public class DamageManager : MonoBehaviour
{
    [SerializeField] private Health Heath;
    [SerializeField] private Armor Armor;

    public void TakeDamage(Damage d)
    {
        float ReducedDamage = d.Value * (1 - Armor.DamageReduction);
        Heath.Value -= ReducedDamage;
    }
}
