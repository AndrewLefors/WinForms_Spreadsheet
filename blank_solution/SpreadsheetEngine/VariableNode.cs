using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    /// <summary>
    /// Variable Node inherited from abstract base class Node
    /// Implements all logic to retrieve and evaluate Variables provided for an expression tree.
    /// </summary>
    internal class VariableNode : Node
    {
        internal string Name { get; set; }

        internal double Value { get; set; }


        public VariableNode(string name, double value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Evaluate for Variable Node. Checks if the Variable is defined, then returns the value. otherwise throws exception.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        internal override double Evaluate()
        {
           return this.Value;
        }
    }

}
