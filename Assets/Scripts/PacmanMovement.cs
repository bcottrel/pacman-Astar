﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PacmanMovement : MonoBehaviour
{
    public float speed = 1;
    public GhostMovement[] ghosts;
    public TextMeshProUGUI scoreText;
    public GameObject gc;
    GameController controller;

    public int scorePerBigDot = 50;
    public int scorePerGhost = 200;
    public int lives = 3;
    public Vector3 pacmanstart;
    public bool canMove = true;
    
    Rigidbody2D rb;
    Animator anim;
    Vector2 movement;
    
    float edibletimer; 
    float delaytime = 1.1f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        edibletimer = 0;
        controller = gc.GetComponent<GameController>();
    }

    void FixedUpdate()
    {
        rb.SetRotation(0f);
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (v != 0)
            h = 0;
        
        Move(h, v);
    }

    void Move(float h, float v)
    {
        if (canMove)
        {           
            movement.Set(h, v);
            movement = movement.normalized * (speed * Time.deltaTime);
            
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + movement, speed * Time.deltaTime);
            
            // Animation Parameters
            GetComponent<Animator>().SetFloat("DirX", h);
            GetComponent<Animator>().SetFloat("DirY", v);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ghost"))
        {
            if (!GhostMovement.scared)
            {
                canMove = false;
                anim.SetTrigger("Die");
               
                StartCoroutine(ResetnDelay(delaytime));
                //ResetPostitions();
                if (lives > 1)
                {
                    lives--;
                }
                else
                {                   
                    controller.GameOver();
                }
            }
        }

    }

    void ResetPostitions()
    {
        
        transform.position = pacmanstart;
        canMove = true;
        foreach(GhostMovement ghost in ghosts)
        {
            ghost.transform.position = ghost.jailPoint;
        }
    }

    IEnumerator ResetnDelay(float time)
    {
        yield return new WaitForSeconds(time);

        ResetPostitions();
    }

    public void AddGhostScore()
    {      
        controller.score += scorePerGhost;
        scorePerGhost += scorePerGhost;       
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Big Dot"))
        {
            controller.dotsCollected++;
            Debug.Log("dots: " + controller.dotsCollected);
            controller.score += scorePerBigDot;
            other.gameObject.SetActive(false);
            edibletimer = 8;
            foreach (GhostMovement ghost in ghosts)
            {
                ghost.SetEdible();
            }
        }
        
       else if (other.gameObject.CompareTag("Teleport"))
        {
            rb.transform.position = new Vector3(rb.transform.position.x * -.95f, rb.transform.position.y, rb.transform.position.z);
        }
    }
} 
