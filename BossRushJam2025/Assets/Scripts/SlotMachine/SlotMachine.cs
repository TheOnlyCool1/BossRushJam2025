using System.Collections.Generic;
using UnityEngine;
public class SlotMachine : MonoBehaviour
{
    public List<Symbol> startingSymbols = new List<Symbol>();
    public List<Symbol> symbols = new List<Symbol>();
    public List<GameObject> reels = new List<GameObject>();
    //yay stack
    public Stack<Symbol> rolledSymbols = new Stack<Symbol>();
    int nextReel = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeReels();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //uses up symbols
    public void GetSymbols(int numSymbols) {
        List<Symbol> symbols = new List<Symbol>();

        for (int i = 0; i < numSymbols; i++) 
        {
            if (rolledSymbols.Count <= 0 ) { break; }
            reels[nextReel - 1].GetComponent<Reel>().ResetReel();
            nextReel--;

            symbols.Add(rolledSymbols.Pop());
        }
        //return symbols;
    }
    public void SpinReel() {
        if (nextReel < reels.Count) {
            rolledSymbols.Push(reels[nextReel].GetComponent<Reel>().Spin());
            nextReel++;
        } else {
            Debug.Log("all reels filled!");
        }
    }
    public void InitializeReels() {
        foreach (GameObject reel in reels) {
            reel.GetComponent<Reel>().availableSymbols = new List<Symbol>(startingSymbols);
        }
    }
}
