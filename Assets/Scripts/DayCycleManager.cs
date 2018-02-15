using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DayCycleManager : MonoBehaviour
{

    public bool dayHasEnded;

    public bool switchOff;

    public List<Day> days;
    public List<NPC> currentCustomers;

    public GameObject blackPanel;

    public Vector3 resetPos;
    public Quaternion resetRot;

    public int currentDay;

    private float elapsedTime;
    private float offsetTime;


    // Use this for initialization
    public void Awake()
    {


    }
    public void Start()
    {

        currentCustomers = new List<NPC>();
        elapsedTime = 0f;
        currentDay = 0; // 0th day is day 1
        offsetTime = 0f;
        resetPos = Services.GameManager.player.transform.position;
        resetRot = Services.GameManager.player.transform.rotation;

        dayHasEnded = false;
        switchOff = false;

        List<List<NPC>> npcsDaysOrder= new List<List<NPC>>();
        AddCustomersDays(Services.GameManager.CustomerIvory,npcsDaysOrder);
        AddCustomersDays(Services.GameManager.CustomerSahana,npcsDaysOrder);

        days = new List<Day>();
        for (int d = 0; d < 7; ++d)
        {
            days.Add(new Day(npcsDaysOrder[d])); //first day
        }


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
        switchOff = false;
        blackPanel.SetActive(true);
        currentDay++;
        WaitTillNextDay();
    }

    public void Update()
    {

        if(!dayHasEnded){ //day ends when customers are all gone
            elapsedTime = Time.time - offsetTime;
            for (int i = 0; i < days[currentDay].customers.Count; ++i){
                if(elapsedTime >= days[currentDay].customers[i].GetComponent<CustomerData>().daysvisiting[currentDay]){
                    days[currentDay].customers[i].insideBar = true;
                    currentCustomers.Add(days[currentDay].customers[i]);
                    days[currentDay].customers.RemoveAt(i);
                    break;
                }
            }
            if(days[currentDay].customers.Count == 0 && currentCustomers.Count == 0){
                dayHasEnded = true;
            }

        } else{
            //able to be switched off when day has ended
            if(switchOff){
                ResetDay();
            }
        }


    }

    void WaitTillNextDay(){
        // give some time delay
        BeginDay();
    }

    void BeginDay(){

        elapsedTime = Time.time - offsetTime;
        offsetTime = elapsedTime; //no need to keep track of time
    }


}
