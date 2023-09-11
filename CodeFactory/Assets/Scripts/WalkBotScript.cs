﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkBotScript : MonoBehaviour
{
    //Create a list of gameobjects that holds all of the "goals" (ores and targets) that the walkbot uses
    public GameObject[] goals;
    //Create a transform that holds the closest goal in goals (created previously)
    public Transform tMin;
    //Creates a vector3 with the start position
    Vector3 startPos;

    //Creates a int holding all of the objects/ores that the walkbot has mined
    public int InvContaining = 0;

    //Create a string that will hold the "code" of the walkbot (inputed on the players side then applied to this)
    public string code = "";

    //Create bools dependent on the code (created previously) on if the walkbot can mine, store, or return its previous position
    public bool canMine = false;
    public bool canStore = false;
    public bool returnon = false;

    //Create floats that hold the time that the function its used in should wait
    public float firerate = 1f;
    public float nextfire = 4f;

    //Create a int that holds if the inventory is full (not sure why this is not a bool)
    public int invFullState = 0;

    // Start is called before the first frame update
    void Start() {
        //Set the vector3 startPos to the walkbots current starting position
        startPos = this.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //If the code given by the player exists, then
        if (code != "")
        {
            //Create a list that contains every number in the code
            string[] characters = new string[code.Length];
            //for every number in the list
            for (int i = 0; i < code.Length; i++)
            {
                //Convert that number into a string so it can be read
                characters[i] = System.Convert.ToString(code[i]);
            }
            //foreach (number converted) string in list
            foreach (string character in characters)
            {
                //If the inventory is not full
                if (invFullState != 2)
                {
                    //If code is 1: The walkbot can mine
                    if (character == "1")
                    {
                        canMine = true;
                    }
                    //If code is 2: The walkbot can store the ores it mines
                    if (character == "2")
                    {
                        canStore = true;
                    }
                    //If code is 3: The walkbot can break the ore
                    if (character == "3")
                    {
                        //Testing: See if there is an ore already close
                        // Debug.Log(tMin);
                        //If there is no ore that is close
                        if (tMin == null)
                        {
                            //Assigns all game objects with the tag "ore" as the variable "goals"
                            goals = GameObject.FindGameObjectsWithTag("ore");

                            //Creates a infinitely sized distance as a temporary value
                            float minDist = Mathf.Infinity;

                            //for all ores in goals
                            for (int i = 0; i < goals.Length; i++)
                            {
                                //Finds location of the current walkbot
                                Vector3 currentPos = transform.position;

                                //Sets a new variable "dist" as the Vector3's distance between a ore and the walkbots current position
                                float dist = Vector3.Distance(goals[i].transform.position, currentPos);

                                //If the distance between the ore and the walkbot is smaller than the minimum distance
                                if (dist < minDist)
                                {
                                    //Set the new minimum distance to that distance
                                    minDist = dist;
                                    //Set the closest transform to that ore
                                    tMin = goals[i].transform;
                                }
                            }
                            //Testing: show what it thinks the closest ore is (red line with 3 thickness)
                            Debug.DrawLine(this.transform.position, tMin.transform.position, Color.red, 3);
                            //Get the NavMeshAgent off this object
                            NavMeshAgent agent = GetComponent<NavMeshAgent>();
                            //Set the NavMeshAgents target as the closest ore
                            agent.destination = tMin.position;
                        }
                    }
                    //If code is 4: The walkbot can return to its original position
                    if (character == "4")
                    {
                        //Get the NavMeshAgent off this object
                        NavMeshAgent agent = GetComponent<NavMeshAgent>();
                        //Set the NavMeshAgent to go back to the walkbots starting position
                        agent.destination = startPos;
                    }
                }
                //If code is 5
                if (character == "5")
                {
                    if (InvContaining >= 5)
                    {
                        invFullState = 1;
                    }
                    else
                    {
                        invFullState = 2;
                    }
                }
                //If code is 6
                if (character == "6")
                {
                    invFullState = 1;
                }
            }

        }
        /*
        // Checks if goals is 0
        if (goals.Length < 1)
        {
            //Unused code
        }
        */
    }
    //If another object collides with the walkbot
    private void OnTriggerEnter(Collider other)
    {
        //Blows up the ore using the ore tag, and the walkbot can mine
        if (other.tag == "ore" && canMine == true)
        {
            //If the walkbot can store objects, and the timer has run out
            if (canStore == true && Time.time > nextfire)
            {
                //Reset timer
                nextfire = Time.time + firerate;
                //Add 1 to the int holding the objects the walkbot has mined
                InvContaining++;
            }
            //Get the other script on this object and set collect ore to can stores value
            this.GetComponent<OreMining>().collectOre = canStore;
            //Get the other script on this object and set blowup to be true
            this.GetComponent<OreMining>().blowup = true;
            //Get the other script on this object and set the closest ore to be the ore that was just mined
            this.GetComponent<OreMining>().oreClosest = other.gameObject;
        }
    }
}