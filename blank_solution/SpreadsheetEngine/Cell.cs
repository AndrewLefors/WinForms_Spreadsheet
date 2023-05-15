using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace SpreadsheetEngine
{
    /// <summary>
    /// The Abstract Base Class Cell enables us to set the text value of a specific cell but only retrieve the value.
    /// The Setting of the value should happen in the concrete classes inheriting from this one.
    /// </summary>
    public abstract class Cell : INotifyPropertyChanged
{
    private protected int? rowIndex;
    private protected int? columnIndex;

    private protected string? cellText;
    private protected string? cellValue;

    private protected uint bgColor;

    public List<Cell> referencingCells = new List<Cell>();
    

        /// <summary>
        /// Constructor for abstract base class Cell, sets rowIndex and ColumnIndex for cell retrieval.
        /// </summary>
        /// <param name="rowIndex">a</param>
        /// <param name="columnIndex">b</param>
        public Cell(int? rowIndex = null, int? columnIndex = null)
    {
        this.rowIndex = rowIndex;
        this.columnIndex = columnIndex;

        this.cellText = string.Empty;
        this.cellValue = string.Empty;

        this.bgColor = 0xFFFFFFFF;
    }

    public event PropertyChangedEventHandler? PropertyChanged = delegate { };

    /// <summary>
    /// gets Row index.
    /// </summary>
    public int? Row
    {
        get
        {
            return this.rowIndex;
        }
    }

    /// <summary>
    /// Gets Integer Column index.
    /// </summary>
    public int? Column
    {
        get { return this.columnIndex; }
    }

    /// <summary>
    /// Gets and Sets CellText.
    /// </summary>
    public string? CellText
    {
        get
        {
            return this.cellText;
        }

        set
        {
            if (this.cellText == value)
            {
                return;
            }

            this.cellText = value;

            if (this.PropertyChanged != null)
            {

                this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(this.CellText)));
            }
        }
    }
        public uint BGColor
    {
        get
        {
            return bgColor;
        }

        set
        {
            if (bgColor != value)
            {
                bgColor = value;
                this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(this.BGColor)));
            }
        }
    }

    /// <summary>
    /// Gets CellValue.
    /// </summary>
    public string? CellValue
    {
        get
        {
            return this.cellValue;
        }

        set
        {
            this.cellValue = value;

            // Notify referencing cells that this cell has changed
            foreach (var referencingCell in referencingCells)
            {
                referencingCell.Evaluate();
            }

            this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(this.cellValue)));
        }
    }

    /// <summary>
    /// Adds a referencing cell to the list of cells that reference this cell.
    /// </summary>
    /// <param name="cell">The referencing cell.</param>
    public void AddReferencingCell(Cell cell)
    {
        referencingCells.Add(cell);
    }

    /// <summary>
    /// Removes a referencing cell from the list of cells that reference this cell.
    /// </summary>
    /// <param name="cell">The referencing cell.</param>
    public void RemoveReferencingCell(Cell cell)
    {
        referencingCells.Remove(cell);
    }

    /// <summary>
    /// Evaluates the value of this cell.
    /// </summary>
    /// <returns>The evaluated value.</returns>
    public abstract string Evaluate();
}

}