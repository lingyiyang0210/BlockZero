using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class BinaryDataStream
{
    public static void Save<T>(T serializedObject, string fileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path + fileName + ".dat", FileMode.Create);

        try
        {
            formatter.Serialize(fileStream, serializedObject);
        }
        catch (SerializationException e)
        {
            Debug.Log("Save failed. Error: " + e.Message);
        }
        finally
        {
            fileStream.Close();
        }
    }

    public static T Read<T>(string fileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        string fullPath = path + fileName + ".dat";

        if (!File.Exists(fullPath))
        {
            return default(T);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(fullPath, FileMode.Open);
        T returnType = default(T);

        try
        {
            returnType = (T)formatter.Deserialize(fileStream);
        }
        catch (SerializationException e)
        {
            Debug.Log("Read failed. Error: " + e.Message);
        }
        finally
        {
            fileStream.Close();
        }

        return returnType;
    }


    public static bool Exist(string fileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        return File.Exists(path + fileName + ".dat");
    }
}