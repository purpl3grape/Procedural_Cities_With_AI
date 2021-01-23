using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Purplegrapestudios {

    public class LSystemGenerator : MonoBehaviour
    {
        public Rule[] rules;
        public string rootSentance; //Axiom of L-System
        [Range(0,10)]
        public int iterationLimit = 1;

        private void Start()
        {
            Debug.Log(GenerateSentance());
        }
        public string GenerateSentance(string word = null)
        {
            if(word == null)
            {
                word = rootSentance;
            }
            return GrowRecursive(word);
        }

        private string GrowRecursive(string word, int iterationIndex = 0)
        {
            if(iterationIndex >= iterationLimit)
            {
                return word;
            }
            StringBuilder newWord = new StringBuilder();

            foreach(var c in word)
            {
                newWord.Append(c);
                ProcessRulesRecursively(newWord, c, iterationIndex);
            }

            return newWord.ToString();
        }

        private void ProcessRulesRecursively(StringBuilder newWord, char c, int iterationIndex)
        {
            foreach(var rule in rules)
            {
                if(rule.letter == c.ToString())
                {
                    //Simple Example
                    //Axion (RootWord): A
                    //Rules: Append AB when you see A
                    //Iteration 0: A
                    //Iteration 1: AAB --> A(AB)
                    //Iteration 2: AAABB --> A(A(AB)B)
                    //Iteration 3: AAAABBB --> A(A(A(AB)B)B)

                    //Another Example
                    //Axion: [F]--F
                    //Rules: |[+F][-F]
                    //By replacing the F for what's in the 'rules', we get the first Recursive Iteration: [|[+F][-F]]--|[+F][-F]
                    newWord.Append(GrowRecursive(rule.GetResult(), iterationIndex + 1));
                }
            }
        }
    }
}
