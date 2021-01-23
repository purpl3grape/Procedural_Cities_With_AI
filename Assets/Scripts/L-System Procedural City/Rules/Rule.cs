using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Purplegrapestudios
{
    [CreateAssetMenu(menuName ="ProdeduralCity/Rule")]
    public class Rule : ScriptableObject
    {
        public string letter;
        [SerializeField]
        private string[] results = null;

        public string GetResult()
        {
            return results[0];
        }
    }
}