﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MinionController : EnemyController {
    public bool awake = false;
    public int damageAmount = 8;
    public float attackDuration = 0.5f;
    public float attackCoolDown = 1f;

    private GameObject player;
    private NavMeshAgent agent;
    private Animator anim;
    private ChasePlayer chaser;
    private bool isAttacking = false;
    private float stunDuration = 0.5f;

    void Start () {
        agent = GetComponent<NavMeshAgent> ();
        anim = GetComponentInChildren<Animator> ();
        chaser = GetComponent<ChasePlayer>();
    }

    public override void Hurt (WeaponStatsController stats) {
        stunned = true;
        agent.isStopped = true;
        chaser.knowsPlayerPosition = true;
        float duration = stats.name == "melee" ? stunDuration * 2 : stunDuration;
        StartCoroutine (Stunned (duration));
        StartCoroutine (TakeDamageAfterDelay (stats));
    }

    private IEnumerator TakeDamageAfterDelay (WeaponStatsController stats) {
        yield return new WaitForSeconds (stats.splatterDelay);
        TakeDamage (stats.damage);
    }

    void Update () {
        if (player == null) {
            player = FindObjectOfType<WeaponController> ().gameObject;
        }

        if (awake && !dead && !stunned) {
            HealthController playerHealth = chaser.Chase();
            if (playerHealth != null && !isAttacking) {
                StartCoroutine (Attack (playerHealth));
            }

            if (agent.isStopped) {
                anim.SetBool ("walking", false);
            } else {
                anim.SetBool ("walking", true);
            }
        }
    }

    IEnumerator Stunned (float duration) {
        yield return new WaitForSeconds (duration);
        stunned = false;
    }

    IEnumerator Attack (HealthController playerHealth) {
        isAttacking = true;
        yield return new WaitForSeconds (attackDuration);
        anim.SetBool ("attacking", true);
        if (!stunned && !dead) {
            playerHealth.TakeDamage (damageAmount);
            yield return new WaitForSeconds (attackCoolDown);
        }
        anim.SetBool ("attacking", false);
        isAttacking = false;
    }

    internal void WakeUp () {
        awake = true;
        chaser.knowsPlayerPosition = true;
    }

    public override void DeathEffects () {
        agent.isStopped = true;
        anim.SetBool ("dead", true);
        anim.SetBool ("walking", false);
        agent.updateRotation = false;
    }
}