using System.Collections.Generic;
using UnityEngine;

public class Reel : MonoBehaviour
{
    public List<Symbol> availableSymbols;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeReel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddSymbol(Symbol symbol) {
        availableSymbols.Add(symbol);
    }
    void InitializeReel() {
        availableSymbols = new List<Symbol>
        {
            Symbol.One,
            Symbol.Two
        };
    }
    public Symbol Spin() {
        return availableSymbols[Random.Range(0, availableSymbols.Count)];
    }
}
