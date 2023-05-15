using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    internal class SubtractionOperatorNode : OperatorNode
    {
        public SubtractionOperatorNode() : base("-", 1) { }

        protected override double Evaluate(Node left, Node right)
        {
            return left.Evaluate() - right.Evaluate();
        }
    }
}
