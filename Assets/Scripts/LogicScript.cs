using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicScript : MonoBehaviour
{
    // We create a choice prefab beforehand and assign it to this choice variable in the Unity inspector
    public GameObject choice;
    // We also created the locations to display these choices in Unity and reference them here
    public GameObject[] choiceLocations;
    // We create a variable to store the number of choices we want to create
    public int numberOfChoices = 6;

    // A empty location to store the choices we create
    public Transform choiceContainer;

    // We are manually creating the orders for each choice so they are related words, not random
    // This is just one solution to assigning the orders. You can rework and move this code so the user could assign them at the start of the game later.

    // create all the orders we want and assign them in the Start function
    public List<string> order1 = new List<string>();
    public List<string> order2 = new List<string>();
    public List<string> order3 = new List<string>();
    // etc...
    // then a list to store all the order lists
    public List<List<string>> orders = new List<List<string>>();

    //this variable will keep track of what the correct answer is supposed to be
    public string correctAnswer;

    // We need a reference to the Next Customer button so we can control when it is active or not (displayed on screen)
    public GameObject nextCustomerButton;
    // and a reference to the hint buttons to also control when they are visible
    public GameObject orderButtons;
    // and the customer graphic as well
    public GameObject customerGraphic;
    // and the text we display when there are no more customers
    public GameObject noMoreCustomersText;

    private bool needToClear = false; // this bool var will keep track of it the choices from the last order need clearing

    // also all the score stuff from Flappy Bird. Change to money or any other resource you want to track
    public int playerScore;
    public Text scoreText;

    public GameObject hint1;

    //The Start function is used to set everything up before the game starts
    private void Start()
    {
        // We are just using text (strings) to refer to the objects for now
        // By using the exact same name of the object here and in the asset names we can match them precisely
        order1.InsertRange (order1.Count, new string[] { "Cigarette", "Desk", "Hat", "Sewing Machine", "Shirt", "Water Bottle" });
        order2.InsertRange (order2.Count, new string[] { "Desk", "Cigarette", "Hat", "Sewing Machine", "Shirt", "Water Bottle" });
        order3.InsertRange (order3.Count, new string[] { "Hat", "Desk", "Cigarette", "Sewing Machine", "Shirt", "Water Bottle" });

        // add all the orders to the orders list
        orders.InsertRange (orders.Count, new List<string>[] { order1, order2, order3 });        
    }

     private void PickRandomOrder()
    { 
        // pick a random order from the orders list
        int randomIndex = Random.Range(0, orders.Count);
        List<string> selectedOrder = orders[randomIndex];

        // set the order to the selected order
        SetOrder(selectedOrder);

        // remove the selected order from the orders list so it can't be selected again
        orders.RemoveAt(randomIndex);
    }   
    
    private void SetOrder(List<string> orderNumber)
    { 
        // the first value of the order is the correct answer so we handle that differently from the rest
        GameObject correctChoice = Instantiate(choice);
        correctChoice.name = orderNumber[0];
        correctChoice.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(orderNumber[0]);
        correctChoice.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(orderNumber[0]);
        correctChoice.transform.parent = choiceContainer.transform;

        correctAnswer = orderNumber[0]; // talking to the global variable at the top rather than the internal variables of this function
        // let's play the audio of the correct answer in a different function
       
        StartCoroutine(WaitBeforePlayingAudio());

        // for every other value in the orderNumber list
        // create a new choice object
        // set the name of the choice object to the value in the orderNumber list
        // set the sprite renderer image of the object to the value in the orderNumber list
        // set the audio clip of the object to the value in the orderNumber list
        // set the parent of the object to the ChoiceManager object (to stay organised in the hierarchy)
        for (int i = 1; i < orderNumber.Count; i++) // we normally count from 0 but already dealt with 0 as correct choice so count from 1
        {
            GameObject newChoice = Instantiate(choice);
            newChoice.name = orderNumber[i];
            newChoice.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(orderNumber[i]);
            newChoice.transform.parent = choiceContainer.transform;
        }

        // for each Transform position in the choiceLocations, add a child from the choiceContainer randomly, but only use each location once
        for (int i = 0; i < choiceLocations.Length; i++)
        {
            int randomChoice = Random.Range(0, choiceContainer.childCount);
            choiceContainer.GetChild(randomChoice).transform.position = choiceLocations[i].transform.position;
            choiceContainer.GetChild(randomChoice).transform.parent = choiceLocations[i].transform;
        }        
   }

   IEnumerator WaitBeforePlayingAudio()
    {
        // wait a milisecond before playing audio so the new choices are loaded
        yield return new WaitForSeconds(0.001f);
        // now we can play audio
        CorrectAnswerAudio();
    }

    public void CorrectAnswerAudio()
    {
        // play the audio of the correct answer. this plays automatically from SetOrder, but also from the OnClick() function of the PlayAudio button (so it's a public function)
        GameObject correctChoice = GameObject.Find(correctAnswer);
        Debug.Log(correctChoice);
        correctChoice.GetComponent<AudioSource>().Play();
    }
    public void NextCustomer() // This is called by the OnClick() function of the NextCustomer button (so it's a public function)
    {
        if (needToClear) // if there are old choices we need to clear them before the next customer
        {
            // we need to destroy all the child gameobjects of the choicelocations.length
            for (int i = 0; i < choiceLocations.Length; i++)
        {
            Destroy(choiceLocations[i].transform.GetChild(0).gameObject);
        }
        needToClear = false;

        }
        if (orders.Count == 0) // if there are no more orders we display a text, could update this with Game Over & Final Score
        {
            noMoreCustomersText.SetActive(true);

        } else
        {
            PickRandomOrder();
            nextCustomerButton.SetActive(false);
            orderButtons.SetActive(true);
            customerGraphic.SetActive(true);
        }
    }

    public void AddScore(int scoreToAdd)
    {
        // this is called by the ChoiceScript when the correct choice is clicked
        playerScore = playerScore + scoreToAdd;
        scoreText.text = playerScore.ToString();
        // set the bool to remember to clear choices on next order
        needToClear = true;
        // and make the Next Customer button visible
        nextCustomerButton.SetActive(true);
        orderButtons.SetActive(false);
        customerGraphic.SetActive(false);
    }

    public void Hint1()
    {
        // play the audio that is on the hint1 object e.g.
        hint1.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(correctAnswer + "_hint1");
        hint1.GetComponent<AudioSource>().Play();
        //this will play audio from the resources folder that is labelled like Cigarette_hint1.mp3
    }
}
