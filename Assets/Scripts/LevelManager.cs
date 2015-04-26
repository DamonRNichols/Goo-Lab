using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

    //This class holds all the starting points and 
    //checkpoints for each scene into vector arrays

    //Tutorial
    public GameObject[] TutorialArray = new GameObject[6];

    public GameObject LevelStart;
    public GameObject CheckPoint1;
    public GameObject CheckPoint2;
    public GameObject CheckPoint3;
    public GameObject CheckPoint4;
    public GameObject LevelEnd;

    public int CurrCP = 0;
    public bool IsAtEnd = false;


	// Use this for initialization
	void Start () 
    {
        SetTutorial();
    }
	
	// Update is called once per frame
	void Update () {}

    public void SetNextSpawnPoint(GameObject[] CPArray)
    {
        if (CurrCP >= TutorialArray.Length - 1)
        {
            IsAtEnd = true;
        }
        else
        {
            CurrCP++;
        }
            
    }

    void SetTutorial()
    {
        TutorialArray[0] = LevelStart;
        TutorialArray[1] = CheckPoint1;
        TutorialArray[2] = CheckPoint2;
        TutorialArray[3] = CheckPoint3;
        TutorialArray[4] = CheckPoint4;
        TutorialArray[5] = LevelEnd;
    }
}
