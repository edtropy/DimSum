using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceScript : MonoBehaviour
{
    public LogicScript logic;
    private void Start()
    {
        // getting reference to logic script like in flappy bird tutorial
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    //everything that happens when a choice is clicked...
    public void OnMouseDown()
    {
        Debug.Log("Clicked " + gameObject.name); // check the console to see what we're clicking on and if this is working

        if (gameObject.name == logic.correctAnswer)
        {
            Debug.Log("Correct Choice!");
            gameObject.GetComponent<SpriteRenderer>().color = new Color32(159, 255, 112, 255);
            logic.AddScore(1); // add 1 to score, can replace this with money
        }
        else
        {
            Debug.Log("Wrong Choice!");
            // we will give the wrong choice a red color tint, but you could change the graphic or similar
            gameObject.GetComponent<SpriteRenderer>().color = new Color32(132, 52, 52 , 255);
        }
    }
}
