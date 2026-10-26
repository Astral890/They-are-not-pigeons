using System;
using UnityEngine;
using static PlayerStats;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int currentMoney = 0;
    [SerializeField] private float maxHunger = 100f;
    [SerializeField] private float hunger = 100f;
    [SerializeField] private float hungerDownMinute = 5f;

    public event Action<float> OnHungerChanged;

    private void Update()
    {
        if(hunger > 0f)
        {
            ReduceHunger();
        }
    }
    public void AddMoney(int amount)
    {
        currentMoney += amount;
    }

    public int GetMoney()
    {
        return currentMoney;
    }

    public float GetHunger()
    {
        return hunger;
    }

    private void ReduceHunger()
    {
        if (hunger <= 0f) return;

        float hungerDownSecond = hungerDownMinute / 60f;
        hunger -= hungerDownSecond * Time.deltaTime;
        hunger = Mathf.Max(hunger, 0f);
        OnHungerChanged?.Invoke(hunger);
    }

    private void eat(float amount)
    {
        hunger = Mathf.Clamp(hunger + amount, 0f, maxHunger);
        OnHungerChanged?.Invoke(hunger);
    }
}
