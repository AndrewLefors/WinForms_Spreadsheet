using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    /// <summary>
    /// Node base class to enforce desired functionality
    /// </summary>
    internal abstract class Node
    {
        public Node()
        {
        }
        /// <summary>
        /// Evaluate is enforced for all Node types.
        /// </summary>
        /// <returns></returns>
        internal abstract double Evaluate();
    }
}
