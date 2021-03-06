
/*
This RPG data streaming assignment was created by Fernando Restituto.
Pixel RPG characters created by Sean Browning.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; // to access stream reader and writer
using System; //


#region Assignment Instructions

/*  Hello!  Welcome to your first lab :)

Wax on, wax off.

    The development of saving and loading systems shares much in common with that of networked gameplay development.  
    Both involve developing around data which is packaged and passed into (or gotten from) a stream.  
    Thus, prior to attacking the problems of development for networked games, you will strengthen your abilities to develop solutions using the easier to work with HD saving/loading frameworks.

    Try to understand not just the framework tools, but also, 
    seek to familiarize yourself with how we are able to break data down, pass it into a stream and then rebuild it from another stream.


Lab Part 1

    Begin by exploring the UI elements that you are presented with upon hitting play.
    You can roll a new party, view party stats and hit a save and load button, both of which do nothing.
    You are challenged to create the functions that will save and load the party data which is being displayed on screen for you.

    Below, a SavePartyButtonPressed and a LoadPartyButtonPressed function are provided for you.
    Both are being called by the internal systems when the respective button is hit.
    You must code the save/load functionality.
    Access to Party Character data is provided via demo usage in the save and load functions.

    The PartyCharacter class members are defined as follows.  */

public partial class PartyCharacter
{
    public int classID;

    public int health;
    public int mana;

    public int strength;
    public int agility;
    public int wisdom;

    public LinkedList<int> equipment;

}

/*
    Access to the on screen party data can be achieved via …..

    Once you have loaded party data from the HD, you can have it loaded on screen via …...

    These are the stream reader/writer that I want you to use.
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader

    Alright, that’s all you need to get started on the first part of this assignment, here are your functions, good luck and journey well!
*/

#endregion


#region Assignment Part 1

static public class AssignmentPart1
{
    // signifiers used to save party character info and equipment info respectively
    static int PartyCharacterSaveSignifier = 0;
    static int EquipmentSaveSignifier = 1;

    // path to file where data is saved to
    static string path = Application.dataPath + Path.DirectorySeparatorChar + "SavedDataFile.txt";
    static public void SavePartyButtonPressed()
    {
        Debug.Log("Party Saved!");

        // where we are saving data to (Done)
        StreamWriter sw = new StreamWriter(path);

        foreach (PartyCharacter pc in GameContent.partyCharacters)
        {
            // writing data into text file (Done)
            sw.WriteLine(PartyCharacterSaveSignifier + "," + pc.classID + "," + pc.health + "," + pc.mana + "," +
                   pc.strength + "," + pc.agility + "," + pc.wisdom);

            // save equipment
            foreach(int equipID in pc.equipment)
            {
                //Debug.Log("1," + equip);
                sw.WriteLine(EquipmentSaveSignifier + "," + equipID);
            }
        }
        sw.Close(); // closes file we are saving to
    }

    static public void LoadPartyButtonPressed()
    {
        GameContent.partyCharacters.Clear();

        Debug.Log("Loading Party...");
        
        if (File.Exists(path))
        {

            string line = "";
            StreamReader sr = new StreamReader(path);

            while ((line = sr.ReadLine()) != null)
            {
                string[] cvs = line.Split(',');

                //foreach(string i in cvs)
                //{
                //    Debug.Log(i);
                //}
                //Debug.Log(line);

                int saveDataSignifier = int.Parse(cvs[0]);

                if(saveDataSignifier == PartyCharacterSaveSignifier)
                {
                    PartyCharacter pc = new PartyCharacter(int.Parse(cvs[1]), int.Parse(cvs[2]), int.Parse(cvs[3]),
                    int.Parse(cvs[4]), int.Parse(cvs[5]), int.Parse(cvs[6]));

                    GameContent.partyCharacters.AddLast(pc);
                }
                else if(saveDataSignifier == EquipmentSaveSignifier)
                {
                    GameContent.partyCharacters.Last.Value.equipment.AddLast(int.Parse(cvs[1]));
                }
                
            }
        }

        GameContent.RefreshUI();
    }
}

#endregion


#region Assignment Part 2

//  Before Proceeding!
//  To inform the internal systems that you are proceeding onto the second part of this assignment,
//  change the below value of AssignmentConfiguration.PartOfAssignmentInDevelopment from 1 to 2.
//  This will enable the needed UI/function calls for your to proceed with your assignment.
static public class AssignmentConfiguration
{
    public const int PartOfAssignmentThatIsInDevelopment = 2;
}

/*

In this part of the assignment you are challenged to expand on the functionality that you have already created.  
    You are being challenged to save, load and manage multiple parties.
    You are being challenged to identify each party via a string name (a member of the Party class).

To aid you in this challenge, the UI has been altered.  

    The load button has been replaced with a drop down list.  
    When this load party drop down list is changed, LoadPartyDropDownChanged(string selectedName) will be called.  
    When this drop down is created, it will be populated with the return value of GetListOfPartyNames().

    GameStart() is called when the program starts.

    For quality of life, a new SavePartyButtonPressed() has been provided to you below.

    An new/delete button has been added, you will also find below NewPartyButtonPressed() and DeletePartyButtonPressed()

Again, you are being challenged to develop the ability to save and load multiple parties.
    This challenge is different from the previous.
    In the above challenge, what you had to develop was much more directly named.
    With this challenge however, there is a much more predicate process required.
    Let me ask you,
        What do you need to program to produce the saving, loading and management of multiple parties?
        What are the variables that you will need to declare?
        What are the things that you will need to do?  
    So much of development is just breaking problems down into smaller parts.
    Take the time to name each part of what you will create and then, do it.

Good luck, journey well.

*/

static public class AssignmentPart2
{
    public const string PartyMetaFile = "PartyIndicesAndNames.txt";
    
    private static LinkedList<PartySaveData> Parties;
    private static uint lastUsedIndex;

    static public void GameStart()
    {
        GameContent.RefreshUI();

        LoadPartyMetaData();
    }

    static public List<string> GetListOfPartyNames()
    {
        if (Parties == null)
        {
            return new List<string>()
            {
                "EMPTY"
            };
        }

        List<string> pNames = new List<string>();

        foreach (PartySaveData psd in Parties)
        {
            pNames.Add(psd.name);
        }



        return pNames;
    }

    static public void LoadPartyDropDownChanged(string selectedName)
    {
        foreach (PartySaveData psd in Parties)
        {
            if (selectedName == psd.name)
            {
                psd.LoadParty();
            }
        }

        GameContent.RefreshUI();
        Debug.Log("Loading party: " + selectedName);
    }

    static public void SavePartyButtonPressed()
    {
        lastUsedIndex++;

        PartySaveData p = new PartySaveData(lastUsedIndex, GameContent.GetPartyNameFromInput());
        Parties.AddLast(p);

        SavePartyMetaData();
        p.SaveParty();

        GameContent.RefreshUI();
        Debug.Log("SAVED PARTY!");
    }

    static public void NewPartyButtonPressed()
    {
        Debug.Log("New Party Created!");
    }

    static public void DeletePartyButtonPressed()
    {
        Debug.Log("Party Deleted... D':");
    }

    static public void SavePartyMetaData()
    {
        StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + PartyMetaFile);

        sw.WriteLine("1," + lastUsedIndex);

        foreach (PartySaveData pData in Parties)
        {
            sw.WriteLine("2," + pData.index + "," + pData.name);
        }
        sw.Close();
    }
    
    static public void LoadPartyMetaData()
    {
        Parties = new LinkedList<PartySaveData>();

        string pathPMF = Application.dataPath + Path.DirectorySeparatorChar + PartyMetaFile;

        if (File.Exists(pathPMF))
        {
            string line = "";
            StreamReader sr = new StreamReader(pathPMF);

            while ((line = sr.ReadLine()) != null)
            {
                string[] csv = line.Split(',');

                int saveDataSignifier = int.Parse(csv[0]);

                if (saveDataSignifier == 1)
                {
                    lastUsedIndex = uint.Parse(csv[1]);
                }
                else if (saveDataSignifier == 2)
                {
                    Parties.AddLast(new PartySaveData(uint.Parse(csv[1]), csv[2]));
                }
            }
            sr.Close();
        }
    }
}
#endregion

class PartySaveData
{
    // signifiers used to save party character info and equipment info respectively
    const int PartyCharacterSaveSignifier = 0;
    const int EquipmentSaveSignifier = 1;

    public uint index;
    public string name;
    
    public PartySaveData(uint index, string name)
    {
        this.index = index;
        this.name = name;
    }

    public void SaveParty()
    {
        StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + index + ".txt");

        foreach (PartyCharacter pc in GameContent.partyCharacters)
        {
            // writing data into text file (Done)
            sw.WriteLine(PartyCharacterSaveSignifier + "," + pc.classID + "," + pc.health + "," + pc.mana + "," +
                   pc.strength + "," + pc.agility + "," + pc.wisdom);

            // save equipment
            foreach (int equipID in pc.equipment)
            {
                sw.WriteLine(EquipmentSaveSignifier + "," + equipID);
            }
        }
        sw.Close(); // closes file we are saving to       
    }

    public void LoadParty()
    {
        Debug.Log("Loading Party...");
        string path = Application.dataPath + Path.DirectorySeparatorChar + index + ".txt";

        if (File.Exists(path))
        {
            GameContent.partyCharacters.Clear();

            string line = "";
            StreamReader sr = new StreamReader(path);

            while ((line = sr.ReadLine()) != null)
            {
                string[] cvs = line.Split(',');

                int saveDataSignifier = int.Parse(cvs[0]);

                if (saveDataSignifier == PartyCharacterSaveSignifier)
                {
                    PartyCharacter pc = new PartyCharacter(int.Parse(cvs[1]), int.Parse(cvs[2]), int.Parse(cvs[3]),
                    int.Parse(cvs[4]), int.Parse(cvs[5]), int.Parse(cvs[6]));

                    GameContent.partyCharacters.AddLast(pc);
                }
                else if (saveDataSignifier == EquipmentSaveSignifier)
                {
                    GameContent.partyCharacters.Last.Value.equipment.AddLast(int.Parse(cvs[1]));
                }
            }
            sr.Close();
        }
        GameContent.RefreshUI();
    }
}


/* A1 THINGS TO DO
 * 
 * (DONE) Figure out how to format the data
 * (DONE) Where are we saving the data? - Application.dataPath
 * (DONE) Write data into a text file
 * 
 * 
 * ... Loading stuff
 * (DONE) find the file
 * (DONE) instanciate reader
 * (DONE) open file
 * (DONE) read data line by line
 * 
 */

/* A2 THINGS TO DO
 * 
 * (DONE) Create class that packages file name and an index
 * (DONE) Save and Load party with dummy file
 * 
 * 
 * (DONE) Write party with file name with given index
 * (DONE) (file index, party name)
 * 
 * (DONE) Declare file name as a constant
 * 
 * (DONE) Open the file
 * 
 * (DONE) Figue out the line
 * 
 * (DONE) Write the index + "," + partyname.txt
 * 
 * (DONE) Sequntially create a new index for each new party
 * (DONE) When new index is created, last used index++ (counter)
 * (DONE) Save and load last index used (counter)
 * 
 * 
 * (DONE) Save party with file name of the given index
 * (DONE) Load party with file name of the given index
 * 
 */