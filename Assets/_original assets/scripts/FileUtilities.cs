// todo: add async methods for saving here

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class FileUtilities
{
	public static string CurrentTimestamp
    {
        get
        {
            return DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd_HH-mm-ss");
        }
    }

    public static string ValidatedFileName(string name)
    {
        string ValidatedSaveName1 = name.Replace('*', '_');
        string ValidatedSaveName2 = ValidatedSaveName1.Replace('\\', '_');
        string ValidatedSaveName3 = ValidatedSaveName2.Replace(':', '_');
        string ValidatedSaveName4 = ValidatedSaveName3.Replace('<', '_');
        string ValidatedSaveName5 = ValidatedSaveName4.Replace('>', '_');
        string ValidatedSaveName6 = ValidatedSaveName5.Replace('?', '_');
        string ValidatedSaveName7 = ValidatedSaveName6.Replace('/', '_');
        string ValidatedSaveName8 = ValidatedSaveName7.Replace('|', '_');
        string ValidatedSaveName9 = ValidatedSaveName8.Replace('"', '_');

        return ValidatedSaveName9;
    }

    public static string ValidatedUniqueSaveName(string original)
    {
        string newname = ValidatedFileName(original);

        while (Directory.Exists(Application.persistentDataPath + "/saves/" + newname)) // so we don't have duplicates
        {
            newname = newname + "-";
        }
        return newname;
    }

    public static string ValidatedUniqueBoardName(string original)
    {
        string newname = ValidatedFileName(original);

        while (File.Exists(Application.persistentDataPath + "/savedboards/" + newname + ".tungboard")) // so we don't have duplicates
        {
            newname = newname + "-";
        }
        return newname;
    }

    public static void SaveToFile(string filepath, string filename, object savethis)
    {
        if (!Directory.Exists(filepath)) { Directory.CreateDirectory(filepath); }

        BinaryFormatter br = new BinaryFormatter();
        FileStream stream = new FileStream(filepath + "/" + filename, FileMode.Create);

        br.Serialize(stream, savethis);
        stream.Close();
    }

    public static object LoadFromFile(string filepath, string filename)
    {
        return LoadFromFile(filepath + "/" + filename);
    }

    public static object LoadFromFile(string fullpath)
    {
        if (!File.Exists(fullpath)) { return null; }

        BinaryFormatter br = new BinaryFormatter();
        FileStream stream = new FileStream(fullpath, FileMode.OpenOrCreate);

        object loadedobject = br.Deserialize(stream);
        stream.Close();

        return loadedobject;
    }

    public static void SaveBytesToFile(string filepath, string filename, byte[] bytes)
    {
        File.WriteAllBytes(filepath + "/" + filename, bytes);
    }

    public static byte[] LoadBytesFromFile(string filepath, string filename)
    {
        return File.ReadAllBytes(filepath + "/" + filename);
    }

    // no idea how this works. Copied from the C# documentation: https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
    public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        // If the destination directory doesn't exist, create it.
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, false);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
        }
    }
}