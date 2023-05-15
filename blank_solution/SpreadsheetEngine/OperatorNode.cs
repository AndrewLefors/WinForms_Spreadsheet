using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    /// <summary>
    /// Operator Node class inherited from abstract base Node class. 
    /// Implements operator logic for expression trees and extensibility for new user-provided operators.
    /// </summary>
    internal abstract class OperatorNode : Node
    {
        public string Symbol { get; }
        public int Precedence { get; }

        protected OperatorNode(string symbol, int precedence)
        {
            Symbol = symbol;
            Precedence = precedence;
        }

        internal Node Left { get; set; }
        internal Node Right { get; set; }

        internal override double Evaluate()
        {
            return Evaluate(Left, Right);
        }

        protected abstract double Evaluate(Node left, Node right);
    }
}