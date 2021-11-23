using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class PersistanceController {

    private int presetNumb;
    private int themeNumb;

    public PersistanceController() {
        presetNumb = 10;
        themeNumb = 6;
    }
    
    public PersistanceController(int maxP, int maxT) {
        presetNumb = maxP;
        themeNumb = maxT;
    }

    public void SavePreset(Preset preset, int n) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/preset" + n + ".dice";

        FileStream st = new FileStream(path, FileMode.Create);

        formatter.Serialize(st, preset);
        
        st.Close();
    }

    public void SaveTheme(Theme theme, int n) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/theme" + n + ".dice";

        FileStream st = new FileStream(path, FileMode.Create);

        formatter.Serialize(st, theme);
        
        st.Close();
    }

    public Preset LoadPreset(int n) {
        string path = Application.persistentDataPath + "/preset" + n + ".dice";
        Preset res;

        if(File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream st = new FileStream(path, FileMode.Open);

            res = formatter.Deserialize(st) as Preset;

            st.Close();
        }
        else {
            res = new Preset();
            SavePreset(res, n);
        }

        return res;
    }

    public Theme LoadTheme(int n) {
        string path = Application.persistentDataPath + "/theme" + n + ".dice";
        Theme res;

        if(File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream st = new FileStream(path, FileMode.Open);

            res = formatter.Deserialize(st) as Theme;

            st.Close();
        }
        else {
            res = new Theme();
            SaveTheme(res, n);
        }

        return res;
    }

    public List<Preset> LoadAllPresets() {
        List<Preset> res = new List<Preset>();

        for(int i = 1; i <= presetNumb; i++){
            Preset temp = LoadPreset(i);
            res.Add(temp);
        }
        
        return res;
    }
    
    public List<Theme> LoadAllThemes() {
        List<Theme> res = new List<Theme>();

        for(int i = 1; i <= themeNumb; i++){
            Theme temp = LoadTheme(i);
            res.Add(temp);
        }
        
        return res;
    }

    public List<Preset> ResetPresets(){
        DeleteAllPresets();
        return LoadAllPresets();
    }
    
    public List<Theme> ResetThemes(){
        DeleteAllThemes();
        return LoadAllThemes();
    }

    private void DeleteAllPresets() {
        for(int i = 1; i <= 7; i++){
            string path = Application.persistentDataPath + "/preset" + i + ".dice";
            if(File.Exists(path))
                File.Delete(path);
        }
    }
    
    private void DeleteAllThemes() {
        for(int i = 1; i <= 7; i++){
            string path = Application.persistentDataPath + "/theme" + i + ".dice";
            if(File.Exists(path))
                File.Delete(path);
        }
    }
    
    public void SaveDefaultValues(int presNumb, int themeNumb) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/default.dice";

        FileStream st = new FileStream(path, FileMode.Create);

        formatter.Serialize(st, presNumb);
        formatter.Serialize(st, themeNumb);
        
        st.Close();
    }
    
    public List<int> LoadDefaultValues() {
        string path = Application.persistentDataPath + "/default.dice";
        List<int> res = new List<int>();

        if(File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream st = new FileStream(path, FileMode.Open);

            res.Add((int) formatter.Deserialize(st));
            res.Add((int) formatter.Deserialize(st));

            st.Close();
        }
        else {
            res = new List<int>();
            SaveDefaultValues(1, 1);
        }

        return res;
    }
}