using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Root
{
    public int totalTime;
    public List<Package> packages = new List<Package>();
}

[Serializable]
public class Package
{
    public int type_id;
    public int typeId;
    public string type;
    public int subtype_id;
    public int subtypeId;
    public string subtype;
    public int time;
    public int level;
    public List<Exercise> exercises = new List<Exercise>();
}

[Serializable]
public class Exercise
{
    public int id;
    public string distractors;
    public string target;
    public string display;
    public string word;
    public string chunks;     
    public int min_movements;    
    public string sentence;      
}