using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    /// <summary>
    /// ConstantNode inherited from abstract base class Node to handle all logic for setting and evaluating constants.
    /// </summary>
    internal class ConstantNode : Node
    {
        public double Value { get; set; }

        public ConstantNode(double value)
        {
            this.Value = value;
        }
        internal override double Evaluate()
        {
            return Value;
        }
    }
}
