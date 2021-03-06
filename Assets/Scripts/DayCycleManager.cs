﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Yarn.Unity;
using System.Linq;

public class DayCycleManager : MonoBehaviour
{
    public bool noteSigned;
    public int maxDays = 3; //actually 7
    public bool dayHasEnded;

    public bool dayReallyStarted;

    public bool switchOff;

    public List<Day> days;
    public List<NPC> currentCustomers;

    public GameObject blackPanel, whitePanel;

    public Vector3 resetPos;
    public Quaternion resetRot;

    public int currentDay;

    private float elapsedTime;
    private float offsetTime;
    public bool doorOpened;


    public Transform spawnPoint1, spawnPoint2;

    public bool skipTutorial;

    private bool removedSahanaFromDay3;

    /*
     * order:
     * if !dayReallyStarted, we're only in the backroom scene.
     * in the backroom scene, if the player interacts with the door --> dayReallyStarted = true
     * if dayReallyStarted, the player is now in the bar scene.
     * until every customer is gone, dayHasEnded = false.
     * when every customer is gone, dayHasEnded = true.
     * if dayHasEnded, the switch can be switched off, ending the day.
     * this triggers the black panel, with the title segments. 
     * once the title segments are over, the next day begins. dayReallyStarted = false.
     * 
     * 
     * 
     * */


    // Use this for initialization
    public void Awake()
    {


    }
    public void Start()
    {
        DOTween.Init();
        //skipTutorial 
        doorOpened = false;
        removedSahanaFromDay3 = false;
        currentCustomers = new List<NPC>();
        elapsedTime = 0f;
        //currentDay = 0; // 0th day is day 1
        offsetTime = 0f;
        dayReallyStarted = false;
        resetPos = Services.GameManager.player.transform.position;
        resetRot = Services.GameManager.player.transform.rotation;

        dayHasEnded = false;
        switchOff = false;

        List<List<NPC>> npcsDaysOrder= new List<List<NPC>>();
        AddCustomersDays(Services.GameManager.CustomerIvory,npcsDaysOrder);
        AddCustomersDays(Services.GameManager.CustomerSahana,npcsDaysOrder);
        AddCustomersDays(Services.GameManager.CustomerJulia, npcsDaysOrder);
        AddCustomersDays(Services.GameManager.CustomerIzzy, npcsDaysOrder);
        AddCustomersDays(Services.GameManager.CustomerJulia, npcsDaysOrder);

        for (int n1 = 0; n1 < npcsDaysOrder.Count; ++n1){
            for (int n2 = 0; n2 < npcsDaysOrder[n1].Count; n2++){
                npcsDaysOrder[n2] = npcsDaysOrder[n2].Distinct().ToList();
            }
        }

        days = new List<Day>();
        for (int d = 0; d < 7; ++d)
        {
            days.Add(new Day(npcsDaysOrder[d])); //first day
        }


        if (skipTutorial)
        {
            MovePlayerTo(spawnPoint2); //next to the bar
            dayReallyStarted = true;
        } else{
            MovePlayerTo(spawnPoint1);
        }

        if(currentDay == 0){
            StartCoroutine(StartGame());
        }

    }

    void MovePlayerTo(Transform t){
        Services.GameManager.player.transform.position =
            new Vector3(t.position.x,
                        Services.GameManager.player.transform.position.y,
                        t.position.z);
        Services.GameManager.player.transform.rotation = t.rotation;
    }

    IEnumerator StartGame(){ //showing Day 1 card
        Services.GameManager.playerInput.isInputEnabled = false;
        blackPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        blackPanel.SetActive(false);
        Services.GameManager.playerInput.isInputEnabled = true;
    }

    void AddCustomersDays(NPC cust, List<List<NPC>> list){
        float[] dv = cust.GetComponent<CustomerData>().daysvisiting;
        for (int l = 0; l < dv.Length; ++l)
        { //adds 7 indices
            list.Add(new List<NPC>());
        }
        for (int a = 0; a < dv.Length; ++a)
        {
            if (dv[a] > 0f)
            {
                list[a].Add(cust);
            }
        }
    }

    public void ResetDay()
    {
        dayHasEnded = false;
        dayReallyStarted = false;
        switchOff = false;
        switch (currentDay)
        {
            //blackPanel.GetComponentInChildren<Text>().text =;
            case 0:
                blackPanel.GetComponentInChildren<Text>().text = "TO BE A BETTER BARTENDER";
                break;
            case 1:
                blackPanel.GetComponentInChildren<Text>().text = "RAISE YOUR HANDS";
                break;
            case 2:
                blackPanel.GetComponentInChildren<Text>().text = "AS IF IN PRAYER";
                break;
        }
        blackPanel.SetActive(true);
        Services.GameManager.audioController.spotlightsfx.Play();
        Services.GameManager.audioController.currentlyPlayingBgm.Stop();
        Services.GameManager.audioController.bgmIvory.Stop();
        Services.GameManager.audioController.bgmIzzy.Stop();
        Services.GameManager.audioController.bgmJulia.Stop();
        Services.GameManager.audioController.bgmSahana.Stop();
        Services.GameManager.audioController.bgmYun.Stop();
        Services.GameManager.audioController.currentlyPlayingBgm = null;
        Services.GameManager.audioController.signhum.Stop();
        currentDay++;

        //if (skipTutorial && currentDay + 1 == maxDays)
        if(currentDay + 1 > maxDays)
        {
            blackPanel.GetComponentInChildren<Text>().text = "THE END (FOR NOW)";
        }
        else
        {
            StartCoroutine(WaitTillNextDay());
        }
    }

    public void Update()
    {
        if(currentDay == 2 && !removedSahanaFromDay3) //day 3
        {
            if(Services.GameManager.dialogue.variableStorage.GetValue("$gaveSahanaAlcohol").AsNumber < 0){
                //remove Sahana from day 3
                /* days is a List of Day, which is a class that has 
                 * list of NPC called customers
                 * 
                 * */
                //days[currentDay].customers.Remove();
                for (int i = 0; i < days[currentDay].customers.Count; ++i){
                    if(days[currentDay].customers[i].characterName == "Sahana"){
                        days[currentDay].customers.RemoveAt(i);
                        removedSahanaFromDay3 = true;
                    }
                }
            }
        }

        if(doorOpened){ //doorOpened = true when door interacted with after checking note
            doorOpened = false;
            StartCoroutine(TransitionSequence());
        }

        if (dayReallyStarted)
        {
            //if(Services.GameManager.player.)
            if(offsetTime == elapsedTime){ //need to do offsetTime to account for the beginning sequence
                offsetTime = Time.timeSinceLevelLoad;
            }
            if (!dayHasEnded)
            { //day ends when customers are all gone
                elapsedTime = Time.timeSinceLevelLoad - offsetTime;
                for (int i = 0; i < days[currentDay].customers.Count; ++i)
                {
                    if (elapsedTime >= days[currentDay].customers[i].GetComponent<CustomerData>().daysvisiting[currentDay])
                    {

                        if (!currentCustomers.Contains(days[currentDay].customers[i]))
                        {
                            days[currentDay].customers[i].insideBar = true;
                            currentCustomers.Add(days[currentDay].customers[i]);
                            days[currentDay].customers.RemoveAt(i);
                            break;
                        }
                    }
                }
                if (days[currentDay].customers.Count == 0 && currentCustomers.Count == 0)
                {
                    dayHasEnded = true;
                    days[currentDay].customers.Clear();
                }

            }
            else
            {
                //able to be switched off when day has ended
                if (switchOff)
                {
                    ResetDay();
                }
            }

        }

    }


    IEnumerator TransitionSequence(){
        //fade
        whitePanel.SetActive(true);
        whitePanel.GetComponentInChildren<Image>().DOFade(1f, 3f);
        yield return new WaitForSeconds(3f);
        whitePanel.GetComponentInChildren<Image>().DOFade(0f, 1f).OnComplete(() => whitePanel.SetActive(false));

        MovePlayerTo(spawnPoint2);

        dayReallyStarted = true;
    }

    IEnumerator WaitTillNextDay(){
        // give some time delay
        yield return new WaitForSeconds(3f);
        blackPanel.GetComponentInChildren<Text>().text = "DAY "+(currentDay+1);
        yield return new WaitForSeconds(3f);
        BeginDay();
    }


    void BeginDay(){
        Debug.Log("beginning the next day");

        if (skipTutorial)
        {
            MovePlayerTo(spawnPoint2);
            dayReallyStarted = true;
        } else{
            MovePlayerTo(spawnPoint1);
        }


        elapsedTime = Time.timeSinceLevelLoad - offsetTime;
        offsetTime = elapsedTime; //no need to keep track of time

        Debug.Log("elapsedTime: " + elapsedTime);
        //Debug.Log("offsetTime: " + offsetTime);


        Services.GameManager.audioController.signhum.Play();
        blackPanel.SetActive(false);
        Services.GameManager.playerInput.isInputEnabled = true;

    }


}
