using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.IO.Compression;
using UnityEngine.Networking;
using UnityEngine.UI;

//  <-- Versió reduïda i modificada de l'script original -->  //
public class ModdingObject : MonoBehaviour
{
    //  <-- Iniciar corrutines -->  //
    public void DownloadFile(string id) 
    {
        StartCoroutine(DownloadFileIE(owner, repo, Path.Combine(path, id + ".blb"), Path.Combine(Application.dataPath, "mods", id + ".blb"), "main", token));
        StartCoroutine(DownloadIE(Path.Combine(Application.dataPath, "mods", id), id));
    }

    //  <-- Descarregar les dades -->  //
    IEnumerator DownloadFileIE(string owner, string repo, string branch, string path, string localPath)
    {
        //  <-- Obtenim l'enllaç -->  //
        repo = repo.Replace(" ", "-");
        string fileUrl = ApiBaseUrl.Replace("{owner}", owner).Replace("{repo}", repo).Replace("{branch}", branch).Replace("{path}", path);

        //  <-- Enviem una sol·licitud -->  //
        UnityWebRequest request = UnityWebRequest.Get(fileUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            //  <-- Creem la carpeta (si no existeix) -->  //
            if (!Dir.Exists(Dir.Name(localPath))) {Dir.Create(Dir.Name(localPath));}

            //  <-- Escrivim l'arxiu -->  //
            System.IO.File.WriteAllBytes(localPath, request.downloadHandler.data);
        }
    }

    //  <-- Descarregar el mod -->  //
    IEnumerator DownloadIE(string filePath, string id)
    {
        //  <-- Llegim l'arxiu -->  //
        string[] lines = File.ReadAllLines(filePath + ".blb");

        //  <-- Comprovem la versió descarregada (si ja s'havia descarregat) -->  //
        bool download = true;
        try 
        {
            string[] version = File.ReadAllLines(Path.Combine(filePath, "version.json"));
            if (lines[1] == version[0]) 
            {
                download = false;
            }
        }
        catch {}
        if (download)
        {
            //  <-- Enviem una sol·licitud -->  //
            UnityWebRequest www = UnityWebRequest.Get(lines[0]);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                //  <-- Descarreguem l'arxiu -->  //
                string savePath = Path.Combine(filePath, "../Mod" + ".zip");
                File.WriteAllBytes(savePath, www.downloadHandler.data);

                //  <-- Eliminem la versió anterior (si existeix) -->  //
                if (Directory.Exists(filePath)) {Directory.Delete(filePath, true);}

                //  <-- Descomprimim -->  //
                ZipFile.ExtractToDirectory(savePath, filePath, true);

                //  <-- Eliminem l'arxiu .zip -->  //
                File.Delete(savePath);

                //  <-- Escrivim la versió actual -->  //
                KLyb.IO.File.Write(Path.Combine(filePath, "version.json"), lines[1]);
            }
        }

        //  <-- Eliminem l'arxiu .blb -->  //
        File.Delete(filePath + ".blb");
        //  <-- Assignem les dades per enviar-les als altres jugadors -->  //
        PlayerPrefs.SetString("$Modding:ID$", id);
        PlayerPrefs.SetString("$Modding:Last-Path$", filePath);
    }
}
