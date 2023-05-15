using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpreadsheetEngine
{
    /// <summary>
    /// Stores the root node for any compiled expression trees and evaluates the contents based on defualt and user supplied operators.
    /// </summary>
    public class ExpressionTree
    {
        private Node root;
        private Expression expression = new Expression();
        private Dictionary<string, double> variables = new Dictionary<string, double>();

        public ExpressionTree(string expression)
        {
            string postfix = Expression.ConvertToPostFix(expression);
            Console.WriteLine(postfix);
            root = Expression.Compile(expression);
        }

        /// <summary>
        /// Sets a variable in the varibales node dictionary to a desired value.
        /// Should this method be in the VariableNode class?
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetVariable(string name, double value)
        {
            if (Expression.variables.ContainsKey(name))
            {
                Expression.variables[name] = value;

            }
            else
            {
                Expression.variables.Add(name, value);
            }

        }

        /// <summary>
        /// Public method to evaluate tree. Each node has it's own respective evaluate method to handle speific use cases.
        /// </summary>
        /// <returns></returns>
        public double Evaluate()
        {
            return this.root.Evaluate();
        }
    }
}
