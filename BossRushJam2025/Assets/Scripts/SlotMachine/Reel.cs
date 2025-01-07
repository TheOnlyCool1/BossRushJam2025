using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum ReelMod {
        Normal,
        Double
    }

public class Reel : MonoBehaviour
{
    public List<Symbol> availableSymbols;
    public Symbol currSymbol;
    public ReelMod currMod;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddSymbol(Symbol symbol) {
        availableSymbols.Add(symbol);
    }
    //void InitializeReel() {
        ///availableSymbols = new List<Symbol>();
    ///}
    public Symbol Spin() {
        Symbol symbol = availableSymbols[Random.Range(0, availableSymbols.Count)];
        currSymbol = symbol;
        gameObject.GetComponent<Image>().sprite = symbol.sprite;
        return symbol;
    }
    public void SetMod(ReelMod mod) {
        currMod = mod;
    }
    public void ResetReel() {
        currSymbol = null;
        gameObject.GetComponent<Image>().sprite = null;
    }
}
