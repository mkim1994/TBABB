using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityStandardAssets.Characters.FirstPerson;

public class DayCycleManager : MonoBehaviour
{

    public bool dayHasEnded;

    public bool switchOff;

    public List<Day> day;

    public GameObject blackPanel;

    public Vector3 resetPos;
    public Quaternion resetRot;

    public int currentDay;


    // Use this for initialization
    public void Awake()
    {


    }
    public void Start()
    {

        currentDay = 1;
        resetPos = Services.GameManager.player.transform.position;
        resetRot = Services.GameManager.player.transform.rotation;

        dayHasEnded = false;

        day = new List<Day>();
        day.Add(new Day(1)); //just one customer on the first day
        day.Add(new Day(2)); //two customers on the second day, etc.
        day.Add(new Day(1));

        switchOff = false;

        //currentNumCustomers = day[0].numCustomers;

    }

    public void ResetDay()
    {
        dayHasEnded = false;
        switchOff = false;
        blackPanel.SetActive(true);

        WaitTillNextDay();
    }

    public void Update()
    {
       /* if (currentDay < 3)
        {
        }
        if (dayHasEnded && switchOff)
        {
            ResetDay();
        }*/


    }

    private void Day(int today)
    {
        switch (today)
        {
            case 0:
                Day0();
                break;
            case 1:
                Day1();
                break;
        }
    }
    private void Day0()
    {

    }
    private void Day1()
    {

    }

    private void WaitTillNextDay()
    {
      //  dialogue.WaitSeconds(currentDay, 5f, player, resetPos, resetRot, blackPanel);

    }

    public void DayCycleTrueReset()
    {


    }




}
