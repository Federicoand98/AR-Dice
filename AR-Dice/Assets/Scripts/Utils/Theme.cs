using System.Collections.Generic;

[System.Serializable] public class Theme {

    private float _dieR;
    private float _dieG;
    private float _dieB;

    private float _numbR;
    private float _numbG;
    private float _numbB;

    public Theme() {
        _dieR = 0f;
        _dieG = 0f;
        _dieB = 0f;

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
}