﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Waveconfig waveConfig;
    private List<Transform> waypoints;
    private Vector2 laserMotion = new Vector2(0, 0);
    private Coroutine firingCoroutine;
    private GameSession gameSession;

    [Header("Enemy")]
    [SerializeField] float health = 500f;
    [SerializeField] int enemyScore = 150;

    [Header("Sound Effect")]
    [SerializeField] AudioClip deathSound;
    [SerializeField] [Range(0, 1)] float deathVolume = 1f;
    [SerializeField] AudioClip laserSound;
    [SerializeField] [Range(0, 1)] float laserVolume = 0.3f;

    [Header("Explosion Effect")]
    [SerializeField] GameObject explosionEffect;
    [SerializeField] float durationOfExplosion = 1f;

    [Header("Laser Movement")]
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] GameObject laser;
    [SerializeField] float laserSpeed = 20f;
    [SerializeField] float firingPeriod = 0.5f;

    int waypointIndex = 0;

    private void Start()
    {
        waypoints = waveConfig.GetWaypoints();
        gameSession = FindObjectOfType<GameSession>();
        transform.position = CurrentPosition();
        ResetShotCounter();
        laserMotion.y -= laserSpeed;
    }

    private void Update()
    {
        EnemyWaypoints();
        CountDownAndShoot();
    }

    private void ResetShotCounter()
    {
        shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }
    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            ResetShotCounter();
        }
    }

    void Fire()
    {
        GameObject newLaser = Instantiate(laser, transform.position, Quaternion.identity);
        newLaser.GetComponent<Rigidbody2D>().velocity = laserMotion;
        AudioSource.PlayClipAtPoint(laserSound, Camera.main.transform.position, laserVolume);
    }

    private void EnemyWaypoints()
    {
        if (waypointIndex <= waypoints.Count - 1)
        {
            Vector2 targetPosition = CurrentPosition();
            float step = waveConfig.GetMoveSpeed() * Time.deltaTime;

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);

            if (IsEnemyArrived(targetPosition))
            {
                waypointIndex++;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Laser laser = collision.gameObject.GetComponent<Laser>();
        if (!laser) return;
        ProcessHit(laser);
    }

    private void ProcessHit(Laser laser)
    {
        health -= laser.GetDamage();
        laser.Hit();
        Die();
    }

    private void Die()
    {
        if (health <= 0)
        {
            gameSession.AddToScore(enemyScore);
            Destroy(gameObject);
            Instantiate(explosionEffect, transform.position, transform.rotation);
            AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathVolume);
        }
    }

    public void SetWaveConfig(Waveconfig waveConfig)
    {
        this.waveConfig = waveConfig;
    }

    private bool IsEnemyArrived(Vector2 targetPosition)
    {
        return (transform.position.x == targetPosition.x) && (transform.position.y == targetPosition.y);
    }

    private Vector2 CurrentPosition()
    {
        return waypoints[waypointIndex].transform.position;
    }
}
