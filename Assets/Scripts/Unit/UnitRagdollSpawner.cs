using System;
using UnityEngine;

public class UnitRagdollSpawner : MonoBehaviour
{
    [SerializeField] private Transform 布娃娃预制体;
    [SerializeField] private Transform 原始骨骼根节点;

    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        healthSystem.OnDead += HealthSystem_OnDead;
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        Transform ragdollTransform = Instantiate(布娃娃预制体, transform.position, transform.rotation);
        UnitRagdoll unitRagdoll = ragdollTransform.GetComponent<UnitRagdoll>();

        unitRagdoll.Setup(原始骨骼根节点);
    }
}
