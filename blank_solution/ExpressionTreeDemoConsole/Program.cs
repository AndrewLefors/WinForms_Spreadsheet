using SpreadsheetEngine;
public class Program 
{
    
    public static int Main(string[] args)
    {
        ExpressionTree expressionTree = null;
        string? expression = string.Empty;
        bool loop = true;
        while(loop)
        {
            if (expression != null)
            {
                Console.WriteLine($"Menu (Current Expression: {expression})");
             }

            Console.WriteLine($"\t1: Enter a New Expression.");
            Console.WriteLine($"\t2: Set a Variable Value.");
            Console.WriteLine($"\t3: Evaluate Tree.");
            Console.WriteLine($"\t4: Quit");

            var option = int.Parse(Console.ReadLine());
            switch(option) 
            {
                case 1:
                    Console.WriteLine($"Enter Expression: ");
                    expression = Console.ReadLine();
                    Console.WriteLine(expression);
                    expressionTree = new ExpressionTree(expression);
                    
                    break;
                case 2:
                    Console.WriteLine("Enter the Variable name, press enter, then enter the value and enter again.");
                    string varName = Console.ReadLine();
                    double varVal = double.Parse(Console.ReadLine());
                    expressionTree.SetVariable(varName, varVal);
                    break;
                case 3:
                    if (expressionTree != null)
                    {
                        
                        Console.WriteLine(expressionTree.Evaluate());
                    }
                    else
                    {
                        Console.WriteLine("First Construct an expression Tree by entering an Expression.");
                    }
                    break;
                case 4:
                    Console.WriteLine("Goodbye!");
                    loop = false;
                    break;

                default:
                    Console.WriteLine($"Error: input {option} not recognized.");
                   
                    break;
            }

        }

       
        return 0;
    }
}