using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class FileIO
{
    private string path;
    public FileIO() { } 
    public FileIO(string fileName)
    {
        path = Application.streamingAssetsPath + '\\' + fileName;

        if (!File.Exists(path))
        {
            File.WriteAllText(path, "");
        }
    }
    public void WriteToFile(int enemyAction, float stamina, float reactionTime, int action)
    {
        string content = "Enemy Action: " + enemyAction + "\n" + "Stamina: " + stamina + "\n" + "Reaction Time: " + reactionTime
            + "\n" + "Action Taken: " + action; 
        File.AppendAllText(path, "Data\n");
        File.AppendAllText(path, content + "\n\n");
    }
    public List<string> ReadFromFile(string fileName)
    {
        string filePath = Application.streamingAssetsPath + '\\' + fileName;

        return File.ReadAllLines(filePath).ToList();
    }
    public void ParseFLOAT(string str, out float output)
    {
        float.TryParse(str, out output);
    }
    public void ParseINT(string str, out int output)
    {
        int.TryParse(str, out output);
    }
}
