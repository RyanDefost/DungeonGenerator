using System;
using UnityEngine;

namespace Passes
{
    [CreateAssetMenu(fileName = "Pass", menuName = "Pass/BasePass", order = 1)]
    public abstract class GenerationPass : ScriptableObject
    {
        public bool isRequired = false;
        public bool isDebug = false;
        
        protected Generator generator;
        public void Connect(Generator generator) => this.generator = generator;
        
        public abstract bool SetPass();
    }
}