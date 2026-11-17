using System;
using Unity.Cinemachine;
using UnityEngine;

public class CemeraManager : MonoBehaviour
{
    public enum 虚拟相机
    {
        主要,
        动作特写,
    }

    [SerializeField] private CinemachineCamera 主要虚拟相机;
    [SerializeField] private CinemachineCamera 动作特写虚拟相机;

    private void Start()
    {
        BaseAction.OnAnyActionStart += BaseAction_OnAnyActionStart;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
    }


    public void SetCinemachineCameraPriority(虚拟相机 showCinemachineCamera)
    {
        主要虚拟相机.Priority = 0;
        动作特写虚拟相机.Priority = 0;

        switch (showCinemachineCamera)
        {
            case 虚拟相机.主要:
                主要虚拟相机.Priority = 20;
                break;
            case 虚拟相机.动作特写:
                动作特写虚拟相机.Priority = 20;
                break;
            default:
                Debug.LogError("尝试设置一个未定义的虚拟相机类相机");
                break;
        }

    }

    private void BaseAction_OnAnyActionStart(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                Unit shootUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetTargetUnit();

                Vector3 cameraCharacterHeight = Vector3.up * 1.7f;
                Vector3 shootDir = (targetUnit.GetWorldPosition() - shootUnit.GetWorldPosition()).normalized;

                float shoulderOffsetAmount = 0.5f;
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * shoulderOffsetAmount;

                Vector3 actionCameraPosition =
                shootUnit.GetWorldPosition() + cameraCharacterHeight + shoulderOffset + (shootDir * -1);

                动作特写虚拟相机.transform.position = actionCameraPosition;
                动作特写虚拟相机.transform.LookAt(targetUnit.GetWorldPosition());

                SetCinemachineCameraPriority(虚拟相机.动作特写);
                break;
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                SetCinemachineCameraPriority(虚拟相机.主要);
                break;
        }
    }
}
