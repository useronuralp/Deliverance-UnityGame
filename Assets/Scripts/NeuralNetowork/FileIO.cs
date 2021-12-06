using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
//This class is used to read and write data to/from .txt files.
public struct TrainingData //A helper class that makes the training of the AI tidier.
{
    public int EnemyAction;
    public float ReactionTime; //Currently has no impact on the outcome of the neural network.
    public float Stamina;      //Currently has no impact on the outcome of the neural network.
    public int CorrectActionToTake;

    public void Print() //Used for debug.
    {
        Debug.Log("Enemy Action: " + EnemyAction);
        Debug.Log("Reaction Time: " + ReactionTime);
        Debug.Log("Stamina: " + Stamina);
        Debug.Log("Action: " + CorrectActionToTake);
    }
}
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
    public static void ParseFLOAT(string str, out float output)
    {
        float.TryParse(str, out output);
    }
    public static void ParseINT(string str, out int output)
    {
        int.TryParse(str, out output);
    }
    public static void PopulateTrainingData(List<string> rawData, out List<TrainingData> trainingData)
    {
        trainingData = new List<TrainingData>();
        for (int i = 0; i < rawData.Count; i++)
        {
            if (rawData[i].Contains("Data"))
            {
                //Use regex to extract the desired data from the rawData.
                var enemyAction = Regex.Match(rawData[i + 1], @"\d+").Groups[0].Value;
                var stamina = Regex.Match(rawData[i + 2], @"\d+").Groups[0].Value;
                var reactionTime = Regex.Match(rawData[i + 3], @"([-+]?[0-9]*\.?[0-9]+)").Groups[0].Value;
                var action = Regex.Match(rawData[i + 4], @"\d+").Groups[0].Value;

                //Parse the rawData and write to the out variables.
                ParseFLOAT(stamina, out float staminaFLOAT);
                ParseFLOAT(reactionTime, out float reactionTimeFLOAT);
                ParseINT(enemyAction, out int enemyActionINT);
                ParseINT(action, out int actionINT);

                //Populate the actual training data that will be passed to the neural network.
                trainingData.Add(new TrainingData { EnemyAction = enemyActionINT, Stamina = staminaFLOAT, ReactionTime = reactionTimeFLOAT, CorrectActionToTake = actionINT });
            }
        }
    }
}
