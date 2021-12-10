using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class Theme {
    private float _dieR, _dieG, _dieB;
    private float _numbR, _numbG, _numbB;
    
    public Theme() {
        // Die color initialized to grey
        _dieR = .345f;
        _dieG = .345f;
        _dieB = .345f;

        // Number color initialized to white
        _numbR = 1f;
        _numbG = 1f;
        _numbB = 1f;
    }

    public float dieR {
        get => _dieR;
        set => _dieR = value;
    }

    public float dieG {
        get => _dieG;
        set => _dieG = value;
    }

    public float dieB {
        get => _dieB;
        set => _dieB = value;
    }

    public float numbR {
        get => _numbR;
        set => _numbR = value;
    }

    public float numbG {
        get => _numbG;
        set => _numbG = value;
    }

    public float numbB {
        get => _numbB;
        set => _numbB = value;
    }

    public void SetDieColor(Color color) {
        _dieR = color.r;
        _dieG = color.g;
        _dieB = color.b;
    }
    
    public void SetNumbColor(Color color) {
        _numbR = color.r;
        _numbG = color.g;
        _numbB = color.b;
    }

    public Color GetDieColor() {
        return new Color(_dieR, _dieG, _dieB);
    }
    
    public Color GetNumbColor() {
        return new Color(_numbR, _numbG, _numbB);
    }

    /*
    public Material GetDieMaterial() {
        // to do
    }
    
    public Material GetNumbMaterial() {
        // to do
    }
    */
}