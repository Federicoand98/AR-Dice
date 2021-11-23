using System.Collections.Generic;

[System.Serializable] public class Preset {

    private int _LIMIT = 25;
    private int current;

    private List<int> quantities;

    public Preset() {
        quantities = new List<int>();

        // Order in list is d4, d6, d8, d10, d12, d20
        for(int i = 0; i < 6; i++)
            quantities.Add(0);

        current = 0;
    }

    public int GetD4() {
        return quantities[0];
    }

    public int GetD6() {
        return quantities[1];
    }

    public int GetD8() {
        return quantities[2];
    }

    public int GetD10() {
        return quantities[3];
    }

    public int GetD12() {
        return quantities[4];
    }

    public int GetD20() {
        return quantities[5];
    }

    public int GetIndex(int index) {
        return quantities[index];
    }

    public bool Increment(int index) {
        if ((current + 1) <= _LIMIT) {
            quantities[index]++;
            current++;
            return true;
        }
        else { 
            return false;
        }
    }

    public bool Decrement(int index) {
        if (current != 0 && quantities[index] != 0) {
            quantities[index]--;
            current--;
            return true;
        }
        else { 
            return false;
        }
    }

    public List<int> GetPresetList() {
        return quantities;
    }

}