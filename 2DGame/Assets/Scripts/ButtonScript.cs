using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    OptionScript optionObject;
    // Start is called before the first frame update
    void Start()
    {
        optionObject = gameObject.transform.GetComponentInParent<OptionScript>();
    }

    public void TaskOnClick()
    {
        Debug.Log("TaskOnClik");
        //Add current chosen option to the tree
        optionObject.textObject.optionTree += optionObject.textObject.currentPage + "O" + optionObject.textObject.options.IndexOf(gameObject.name);
        //reset everything
        optionObject.gameObject.SetActive(false);
        optionObject.textObject.hasNextOption = true;
        optionObject.textObject.hasNextPage = true;
        optionObject.textObject.notOption = true;
        //Delete instantiated buttons
        for (int i = 1; i <= optionObject.numOfButtons; i++)
        {
            Destroy(optionObject.gameObject.transform.GetChild(i).gameObject);
        }
        //call DisplayDialog to check if there is more text or options
        optionObject.textObject.DisplayDialog(true);
    }
}