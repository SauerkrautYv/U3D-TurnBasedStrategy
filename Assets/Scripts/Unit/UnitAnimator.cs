using System;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;

    [SerializeField] private Transform rifleTransform;
    [SerializeField] private Transform swordTransform;

    private void Awake()
    {
        if (TryGetComponent(out MoveAction moveAction))
        {
            moveAction.OnMoveStart += MoveAction_OnMoveStart;
            moveAction.OnMoveStop += MoveAction_OnMoveStop;
        }

        if (TryGetComponent(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }

        if (TryGetComponent(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted;
            swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
        }
    }

    private void Start()
    {
        EquipRifle();
    }

    private void SwordAction_OnSwordActionStarted(object sender, EventArgs e)
    {
        EquipSword();
        animator.SetTrigger("isSwordSlash");

    }

    private void SwordAction_OnSwordActionCompleted(object sender, EventArgs e)
    {
        EquipRifle();
    }

    private void MoveAction_OnMoveStart(object sender, EventArgs e)
    {
        animator.SetBool("isRunning", true);
    }

    private void MoveAction_OnMoveStop(object sender, EventArgs e)
    {
        animator.SetBool("isRunning", false);
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger("isShoot");

        Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);
        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
        //不射击到脚上，抬高射击点
        targetUnitShootAtPosition.y = shootPointTransform.position.y;

        bulletProjectile.Setup(targetUnitShootAtPosition);
    }

    private void EquipSword()
    {
        swordTransform.gameObject.SetActive(true);
        rifleTransform.gameObject.SetActive(false);
    }

    private void EquipRifle()
    {
        swordTransform.gameObject.SetActive(false);
        rifleTransform.gameObject.SetActive(true);
    }
}
