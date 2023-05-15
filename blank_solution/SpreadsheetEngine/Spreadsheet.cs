using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SpreadsheetEngine
{
    /// <summary>
    ///Spreadsheet Engine Core
    /// </summary>
    /// 
    [Serializable]
    public class Spreadsheet : INotifyPropertyChanged, ISerializable
    {
        public SpreadsheetCell[,]? spreadsheetCells = null;
        private int? rows = null;
        private int? columns = null;
        /// <summary>
        ///The Spreadsheet is the core of the SpreadsheetAppEngine.
        ///It Generates, stores, and modifies the cells used in the spreadsheet application.
        ///All cells are listened to using the Observer-Broadcaster pattern.
        ///
        /// </summary>
        /// <param name="rows">Integer</param>
        /// <param name="columns">Integer</param>
        public Spreadsheet(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;

            if (this.spreadsheetCells == null)
            {
                this.spreadsheetCells = new SpreadsheetCell[rows, columns];
            }

            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                {
                    this.spreadsheetCells[rowIndex, columnIndex] = new SpreadsheetCell(rowIndex, columnIndex);
                    this.spreadsheetCells[rowIndex, columnIndex].PropertyChanged += CellPropertyChanged;
                }
            }
        }

        public Spreadsheet() { }

        /// <summary>
        /// Get Column Count
        /// </summary>
        private int? ColumnCount
        {
            get
            {
                return this.columns;
            }
        }

        /// <summary>
        /// Get RowCount
        /// </summary>
        private int? RowCount
        {
            get
            {
                return this.rows;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Property Event Handler for CellPropertyChanged.
        /// This function is invoked whenever the CellText in a Cell type is modified.
        /// The method parses the string if a formula is found and implements appropriate behavior.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is SpreadsheetCell)
            {
                SpreadsheetCell editCell = (SpreadsheetCell)sender;
                string? myString = editCell.CellText;
                if (myString != null)
                {
                    if (myString.ToCharArray()[0] == '=')
                    {
                        if (char.IsLetter(myString[1]))
                        {
                            int refCol = Convert.ToInt32(myString[1]) - 65;

                            string? rowString = myString.Substring(2);
                            try
                            {
                                int refRow = Convert.ToInt32(rowString) - 1;
                                Cell? refCell = this.GetCell(refRow, refCol);
                                if (refCell  != null && refCell.CellText == "")
                                {
                                    editCell.CellValue = "0";
                                }
                                else if (refCell.Row == editCell.Row && refCell.Column == editCell.Column)
                                {
                                    editCell.CellValue = "Circular Reference!";
                                }
                                else
                                {
                                    refCell.AddReferencingCell(editCell);
                                    refCell.PropertyChanged += UpdateReferencingCells;
                                    editCell.CellValue = refCell.CellValue;
                                    this.PropertyChanged(refCell, new PropertyChangedEventArgs("UpdateCellReference"));
                                }

                                editCell.PropertyChanged += UpdateReferencingCells;
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    string expressionString = myString.Substring(1);
                                    string varString = expressionString.Substring(0, 2);
                                    ExpressionTree expressionTree = new ExpressionTree(expressionString);
                                    expressionTree.SetVariable(varString, Expression.variables[varString]);
                                    double var = expressionTree.Evaluate();
                                    editCell.CellValue = var.ToString();
                                }
                                catch (Exception innerEx)
                                {
                                    editCell.CellValue = innerEx.Message;
                                }
                            }
                        }

                        // Should then subscribe to the refCells property changed event for when the value is changed.
                        // When the cells text is changed, it automatically calls a property event handler to update its value
                        // So when the the text is changed, the value is changed, and thus all referencing cells are changed
                        else
                        {
                            try
                            {
                                string expressionString = myString.Substring(1);

                                // This is where the expression evaluator comes into play

                                ExpressionTree expressionTree = new ExpressionTree(expressionString);
                                double var = expressionTree.Evaluate();
                                editCell.CellValue = var.ToString();
                            }
                            catch (KeyNotFoundException)
                            {
                                editCell.CellValue = $"INVALID OPERATOR";
                            }
                        }
                    }

                    else
                    {
                        editCell.CellValue = editCell.CellText;
                    }
                }
            }

            this.PropertyChanged(sender, e);
        }

       // public void UpdateReferencingCells(object sender, PropertyChangedEventArgs e)
        //{
            //// Get the cell that triggered the event
            //SpreadsheetCell changedCell = (SpreadsheetCell)sender;

            //foreach (SpreadsheetCell cell in changedCell.referencingCells)
            //{
            //    string cellContents = "=" + Convert.ToChar(changedCell.Column + 65) + Convert.ToString(changedCell.Row + 1);
            //    if (cell != null && cell.CellText.Contains(cellContents))
            //    {
            //        try
            //        {
            //            // Evaluate the dependent cell's formula with the new value of the changed cell
            //            ExpressionTree expressionTree = new ExpressionTree(cell.CellText.Substring(1));
            //            expressionTree.SetVariable(Convert.ToChar(changedCell.Column + 65).ToString() + (changedCell.Row + 1).ToString(), Convert.ToDouble(changedCell.CellValue));
            //            double var = expressionTree.Evaluate();
            //            cell.CellValue = var.ToString();

            //            // Update all dependent cells recursively
            //            UpdateReferencingCells(cell, new PropertyChangedEventArgs("UpdateCells"));
            //        }
            //        catch (Exception ex)
            //        {
            //            cell.CellValue = "Error: " + ex.Message;
            //        }
            //    }
            //}

            //this.PropertyChanged(sender, new PropertyChangedEventArgs("UpdateCells"));


       // }
       public void UpdateReferencingCells(object sender, PropertyChangedEventArgs e)
        {
            // Get the cell that triggered the event
            SpreadsheetCell changedCell = (SpreadsheetCell)sender;

            // Create a dictionary to hold the values of all the variables in the expression
            Dictionary<string, double> variableValues = new Dictionary<string, double>();
            variableValues[Convert.ToChar(changedCell.Column + 65).ToString() + (changedCell.Row + 1).ToString()] = Convert.ToDouble(changedCell.CellValue);

            // Update all dependent cells recursively
            UpdateDependentCells(changedCell.referencingCells, variableValues);

            // Notify listeners that the cells have been updated
            this.PropertyChanged(sender, new PropertyChangedEventArgs("UpdateCells"));
        }

        private void UpdateDependentCells(List<Cell> dependentCells, Dictionary<string, double> variableValues)
        {
            foreach (SpreadsheetCell cell in dependentCells)
            {
                try
                {
                    // Evaluate the dependent cell's formula with the new values of the variables
                    ExpressionTree expressionTree = new ExpressionTree(cell.CellText.Substring(1));
                    foreach (KeyValuePair<string, double> variableValue in variableValues)
                    {
                        expressionTree.SetVariable(variableValue.Key, variableValue.Value);
                    }
                    double result = expressionTree.Evaluate();
                    cell.CellValue = result.ToString();

                    // Update all dependent cells recursively
                    UpdateDependentCells(cell.referencingCells, variableValues);
                }
                catch (Exception ex)
                {
                    cell.CellValue = "Error: " + ex.Message;
                }
            }
        }

        /// <summary>
        /// Takes a row and column index and returns the cell at that location or null if there is no such cell.
        /// Be Careful with the indexes. The form is set up with 1-based indexing but the spreadsheet uses 0-based indexing.
        /// </summary>
        /// <param name="row">Integer</param>
        /// <param name="col">Integer</param>
        /// <returns>Cell or null</returns>
        private Cell? GetCell(int row, int col)
        {
            if (this.spreadsheetCells != null)
            {
                if (this.spreadsheetCells[row, col] != null)
                {
                    return this.spreadsheetCells[row, col];
                }
            }

            return null;
        }

        public void Save(FileStream fstream)
        {
            int rows = (int)this.RowCount;
            int cols = (int)this.ColumnCount;

            // Convert the 2D array to a list of SpreadsheetCell objects
            List<SpreadsheetCell> cells = new List<SpreadsheetCell>();
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    cells.Add(this.spreadsheetCells[row, col]);
                }
            }

            // Serialize the list of SpreadsheetCell objects to XML
            XmlSerializer serializer = new XmlSerializer(typeof(List<SpreadsheetCell>));
            serializer.Serialize(fstream, cells);
        }

        public void Load(FileStream fstream)
        {
            // Deserialize the XML to a list of SpreadsheetCell objects
            XmlSerializer serializer = new XmlSerializer(typeof(List<SpreadsheetCell>));
            List<SpreadsheetCell> cells = (List<SpreadsheetCell>)serializer.Deserialize(fstream);

            // Populate the 2D array with the deserialized SpreadsheetCell objects
            int index = 0;
            foreach (SpreadsheetCell cell in cells)
            {
                int row = index / (int)this.ColumnCount;
                int col = index % (int)this.ColumnCount;

                this.spreadsheetCells[row, col].CellText = cell.CellText;
                this.PropertyChanged(this.spreadsheetCells[row, col], new PropertyChangedEventArgs("LoadXML"));
                index++;
            }
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("rows", rows);
            info.AddValue("columns", columns);
            var cells = new List<SpreadsheetCell>();
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (spreadsheetCells[row, col] != null)
                    {
                        cells.Add(spreadsheetCells[row, col]);
                    }
                }
            }

            info.AddValue("cells", cells.ToArray());
        }

        /// <summary>
        /// Concrete Class for Cell
        /// </summary>
        [Serializable]
        public class SpreadsheetCell : Cell
        {
            /// <summary>
            /// This is a standard text.
            /// with more text.
            /// </summary>
            /// <param name="rowIndex"> a</param>
            /// <param name="columnIndex"> b</param>
            /// 

            public SpreadsheetCell() { }
            public SpreadsheetCell(int rowIndex, int columnIndex)
            {
                this.rowIndex = rowIndex;
                this.columnIndex = columnIndex;

                this.cellText = string.Empty;
                this.CellValue = string.Empty;
            }

            public bool IsDefault()
            {
                return string.IsNullOrEmpty(this.cellText) && string.IsNullOrEmpty(this.cellValue);
            }

            /// <summary>
            /// Gets and Sets CellValue.
            /// </summary>
            internal new string? CellValue
            {
                get
                {
                    return this.cellValue;
                }

                set
                {
                    if (value != null)
                    {
                        if (value == this.cellValue)
                        {
                            return;
                        }

                        //if (value[0] == '=')
                        //{
                        //    //Evaluate cellText for cellValue
                        //    return;
                        //}

                        this.cellValue = value;
                        //this.PropertyChanged(this, new PropertyChangedEventArgs("CellValue"));
                    }

                    return;
                }
            }

            public override string Evaluate()
            {
                return this.CellValue;
            }
        }
    }
}
