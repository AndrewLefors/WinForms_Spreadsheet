using SpreadsheetEngine;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static SpreadsheetEngine.Spreadsheet;

namespace Spreadsheet_Andrew_Lefors
{
    public partial class Form1 : Form
    {
        private SpreadsheetEngine.Spreadsheet spreadsheet;

        public Form1()
        {
            this.InitializeComponent();
            this.InitializeDataGrid();
            this.spreadsheet = new SpreadsheetEngine.Spreadsheet(50, 26);
            this.spreadsheet.PropertyChanged += this.Cell_PropertyChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void InitializeDataGrid(int colLength = 26, int rowLength = 50)
        {
            // Clear View
            this.dataGridView1.Rows.Clear();
            this.dataGridView1.Columns.Clear();

            // Init Cols
            // for the 26 letters in the Alphabet; can extend for more columns
            for (int i = 0; i < colLength; i++)
            {
                // ASCII 65 = A, add i to get next Char in sequnce. Convert the Char to a string to use as colName and colHeaderText
                // Set the actual name of row header index to match column header index (starts at 0)
                string colName = Convert.ToString(Convert.ToChar(65 + i));
                this.dataGridView1.Columns.Add(Convert.ToString(i), colName);
            }

            // Init Rows

            this.dataGridView1.Rows.Add(rowLength);
            for (int i = 0; i < rowLength; i++)
            {
                this.dataGridView1.Rows[i].HeaderCell.Value = Convert.ToString(i + 1);
            }

            this.dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }

        private void Cell_PropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            if (sender is Cell)
            {
                Cell editCell = (Cell)sender;
                int row = Convert.ToInt32(editCell.Row);
                int col = Convert.ToInt32(editCell.Column);

                if (eventArgs.PropertyName == "BGColor")
                {
                    this.dataGridView1.Rows[row].Cells[col].Style.BackColor = System.Drawing.Color.FromArgb((int)editCell.BGColor);
                }
                else if (eventArgs.PropertyName == "LoadXML")
                {
                    this.dataGridView1.Rows[row].Cells[col].Value = Convert.ToString(editCell.CellText);
                }
                else
                {
                    this.dataGridView1.Rows[row].Cells[col].Value = Convert.ToString(editCell.CellValue);
                    string cellName = string.Empty;
                    cellName = Convert.ToString(Convert.ToChar(Convert.ToInt32(editCell.Column) + 65)); // use A instead of B as A represents column 0
                    cellName = cellName + (Convert.ToString(editCell.Row + 1)); // add 1 to the row index to get the correct row number

                    if (Expression.variables.ContainsKey(cellName))
                    {
                        try
                        {
                            Expression.variables[cellName] = Convert.ToDouble(editCell.CellValue); // update the cell value in the expression variable dictionary
                        }
                        catch (Exception ex) 
                        {
                            editCell.CellValue = "Invalid!";
                        }
                    }
                    else
                    {
                        try
                        {
                            Expression.variables.Add(cellName, Convert.ToDouble(editCell.CellValue)); // add a new cell variable to the expression variable dictionary
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    // Evaluate all dependent cells that reference this cell
                    foreach (Cell dependentCell in editCell.referencingCells)
                    {
                        SpreadsheetCell cell = (SpreadsheetCell)dependentCell;
                        string dependentCellName = Convert.ToString(Convert.ToChar(Convert.ToInt32(cell.Column) + 65)) + (Convert.ToString(cell.Row + 1));
                        this.Cell_PropertyChanged(cell, new PropertyChangedEventArgs("CellUpdate")); // evaluate the dependent cell with the updated expression variables
                        this.spreadsheet.UpdateReferencingCells(cell, new PropertyChangedEventArgs("CellUpdate"));

                    }
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (sender is DataGridView)
            {
                DataGridView dataGridView = (DataGridView)sender;

            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Highlight the cell that is being edited
            this.dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.Yellow;
            this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = this.spreadsheet.spreadsheetCells?[e.RowIndex, e.ColumnIndex].CellText;

        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Remove the highlight from the edited cell
            this.dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
            this.spreadsheet.spreadsheetCells[e.RowIndex, e.ColumnIndex].CellText = this.dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
            this.Cell_PropertyChanged(this.spreadsheet.spreadsheetCells[e.RowIndex, e.ColumnIndex], new PropertyChangedEventArgs("CellEndEdit"));


        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                string refString = "=B" + (i +1).ToString();
                this.spreadsheet.spreadsheetCells[i, 5].CellText = "Hello, CPTS 321!";
                this.spreadsheet.spreadsheetCells[i, 1].CellText = $"This Cell is B{i+1}";
                this.spreadsheet.spreadsheetCells[i, 0].CellText = refString;
                Color color = ColorTranslator.FromHtml("0xFF0000");
                UpdateCellColor(dataGridView1.Rows[1].Cells[0], color);

            }

        }

        private void UpdateCellColor(DataGridViewCell cell, Color color)
        {
            cell.Style.BackColor = color;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var stream = new FileStream("spreadsheet.xml", FileMode.Create))
            {
                spreadsheet.Save(stream);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML files (*.xml)|*.xml";
            saveFileDialog.Title = "Save Spreadsheet";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                using (var stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    this.spreadsheet.Save(stream);
                }
            }
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML files (*.xml)|*.xml";
            openFileDialog.Title = "Load Spreadsheet";
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName != null)
            {
                using (var stream = new FileStream(openFileDialog.FileName, FileMode.Open)) 
                {
                    this.spreadsheet.Load(stream);
                }
            }
        }
    }
}