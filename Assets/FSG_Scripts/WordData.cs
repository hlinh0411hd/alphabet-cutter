using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordData : MonoBehaviour {

    public string word;
    public ArrayList listObj;
    
    public void setWord(string word)
    {
        this.word = word; 
    }

    public string getWord()
    {
        return word;
    }

    public void setLisiObj(ArrayList lst)
    {
        listObj = lst;
    }

    public ArrayList getListObj()
    {
        return listObj;
    }
}
