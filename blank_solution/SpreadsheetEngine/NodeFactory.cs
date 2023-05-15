using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class NodeFactory
    {
        private static readonly Dictionary<string, Type> nodeTypes = new Dictionary<string, Type>();

        static NodeFactory()
        {
            LoadOperatorNodeTypes();
        }

        internal static Node CreateNode(string token, Dictionary<string, double> variables)
        {
            if (double.TryParse(token, out double value))
            {
                return new ConstantNode(value);
            }

            if (char.IsLetter(token[0]))
            {
                if (variables.ContainsKey(token))
                {
                    VariableNode varNode = new VariableNode(token, variables[token]);
                    return varNode;
                }
                else
                {
                    VariableNode varNode = new VariableNode(token, 0);
                    return varNode;
                }
            }

            if (nodeTypes.ContainsKey(token))
            {
                return (Node)Activator.CreateInstance(nodeTypes[token]);
            }

            throw new NotSupportedException($"Token '{token}' is not supported.");
        }

        private static void LoadOperatorNodeTypes()
        {
            var operatorNodeTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => type.IsSubclassOf(typeof(OperatorNode)) && !type.IsAbstract);

            foreach (var type in operatorNodeTypes)
            {
                var nodeType = (OperatorNode)Activator.CreateInstance(type);
                nodeTypes[nodeType.Symbol] = type;
            }
        }
    }
}