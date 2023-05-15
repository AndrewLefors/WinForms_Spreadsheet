using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

// ,#####,
// #_   _#
// |a` `a|
// |  u  |
// \  =  /
// |\___/|

//    ,--.    ,--.
//    ((O))--((O))
//  ,'_`--'____`--'_`.
// _: ____________: _

namespace SpreadsheetEngine
{
    /// <summary>
    /// Expression class is the factory for the nodes and contains the operators supported by the application and their precedence.
    /// </summary>
    public class Expression
    {
        public static Dictionary<string, double> variables = new Dictionary<string, double>();
        internal static Dictionary<char, int> Operators = new Dictionary<char, int>
        {
            { '+', 0 },
            { '-', 0 },
            { '/', 1 },
            { '*', 1 },
            { '^', 2 },
            { '(', 3 },
            { ')', 3 },
        };

        /// <summary>
        /// ConvertToPostFix takes an infix expression without spaces and applies a modified Shunting Yard alg.
        /// To obtain the polish-expression with spaces delimiting constants, variables, and operators. 
        /// precedence is maintained without the need for parenthesis.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string ConvertToPostFix(string expression)
        {
            StringBuilder postfix = new StringBuilder();
            Stack<char> opStack = new Stack<char>();

            int expressionSize = expression.Length;
            for (int stringIndex = 0; stringIndex < expressionSize; stringIndex++)
            {
                if (char.IsLetter(expression[stringIndex]))
                {
                    int variableIndex = stringIndex;
                    while (variableIndex < expressionSize && char.IsLetterOrDigit(expression[variableIndex]))
                    {
                        postfix.Append(expression[variableIndex]);
                        variableIndex++;
                    }

                    stringIndex = variableIndex - 1;
                    postfix.Append(" ");
                }
                else if (char.IsDigit(expression[stringIndex]))
                {
                    int constantIndex = stringIndex;
                    while (constantIndex < expressionSize && char.IsDigit(expression[constantIndex]))
                    {
                        postfix.Append(expression[constantIndex]);
                        constantIndex++;
                    }

                    stringIndex = constantIndex - 1;
                    postfix.Append(" ");
                }
                else if (expression[stringIndex] == '(')
                {
                    opStack.Push(expression[stringIndex]);
                }
                else if (expression[stringIndex] == ')')
                {
                    while (opStack.Count > 0 && opStack.Peek() != '(')
                    {
                        char op = opStack.Pop();
                        postfix.Append(op);
                        postfix.Append(" ");
                    }

                    if (opStack.Count > 0 && opStack.Peek() == '(')
                    {
                        opStack.Pop();
                    }
                }
                else if (expression[stringIndex] == ' ')
                {
                    // Do nothing
                }
                else
                {
                    while (opStack.Count > 0 && opStack.Peek() != '(' && Operators[expression[stringIndex]] <= Operators[opStack.Peek()])
                    {
                        char op = opStack.Pop();
                        postfix.Append(op);
                        postfix.Append(" ");
                    }

                    opStack.Push(expression[stringIndex]);
                }
            }

            while (opStack.Count > 0)
            {
                char op = opStack.Pop();
                postfix.Append(op);
                postfix.Append(" ");
            }

            return postfix.ToString().Trim();
        }


        /// <summary>
        /// Compile takes a postfix expression and parses it using the space character as delimiter. 
        /// The expression tokens are converted into appropriate nodes and pushed onto a stack for evaluation.
        /// The root node of the tree is returned.
        /// Should this be in it's own Factory class?
        /// </summary>
        /// <param name="s">s</param>
        /// <returns>s</returns>
        /// <exception cref="ArgumentException">s</exception>
        internal static Node Compile(string s)
        {
            Stack<Node> nodeStack = new Stack<Node>();
            string[] tokens = ConvertToPostFix(s).Split(' ');

            foreach (string token in tokens)
            {
                Node nextNode = NodeFactory.CreateNode(token, Expression.variables);

                if (nextNode != null)
                {
                    nodeStack.Push(nextNode);
                }
                else
                {
                    throw new ArgumentException("Invalid expression");
                }

                if (nodeStack.Count >= 2 && nodeStack.Peek() is OperatorNode)
                {
                    OperatorNode operatorNode = (OperatorNode)nodeStack.Pop();
                    operatorNode.Right = nodeStack.Pop();
                    operatorNode.Left = nodeStack.Pop();
                    nodeStack.Push(operatorNode);
                }
            }

            if (nodeStack.Count != 1)
            {
                throw new ArgumentException("Invalid expression");
            }

            return nodeStack.Pop();
        }
    }
}
