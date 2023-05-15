using Microsoft.Win32.SafeHandles;
using SpreadsheetEngine;

namespace SpreadsheetEngineTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            System.Reflection.Assembly.Load("SpreadsheetEngine");
        }

        [Test]
        public void ColumnHeaderAsciiTest()
        {
            if (Convert.ToString(Convert.ToChar(65 + 0)) == "A")
                { Assert.Pass(); }
            Assert.Fail();
        }

        [Test]
        public void SpreadsheetCellValueTest()
        {
            SpreadsheetEngine.Spreadsheet spreadsheet = new SpreadsheetEngine.Spreadsheet(50,26);
            spreadsheet.spreadsheetCells[0, 0].CellText = "A";
            if (spreadsheet.spreadsheetCells[0,0].CellValue == "A")
            {
                Assert.Pass();
            }
            Assert.Fail();
        }

        [Test]
        public void SpreadsheetReferenceTest()
        {
            SpreadsheetEngine.Spreadsheet spreadsheet = new SpreadsheetEngine.Spreadsheet(50, 26);
            spreadsheet.spreadsheetCells[0, 0].CellText = "A";
            spreadsheet.spreadsheetCells[0, 1].CellText = "=A1";
            if (spreadsheet.spreadsheetCells[0, 1].CellValue == "A")
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public void CellReferenceCascadeTest()
        {
            SpreadsheetEngine.Spreadsheet spreadsheet = new SpreadsheetEngine.Spreadsheet(50, 26);
            spreadsheet.spreadsheetCells[0, 0].CellText = "Based Text W";
            spreadsheet.spreadsheetCells[1, 0].CellText = "=A1";
            spreadsheet.spreadsheetCells[2, 0].CellText = "=A2";
            if (spreadsheet.spreadsheetCells[2, 0].CellValue == "Based Text W" && spreadsheet.spreadsheetCells[2,0].CellText == "=A2")
            {
                Console.WriteLine($"CellText: {spreadsheet.spreadsheetCells[2, 0].CellText}\nCellValue: {spreadsheet.spreadsheetCells[2, 0].CellValue}");
                Assert.Pass();
            }


            Assert.Fail();
        }

        [Test]
        public void ConvertToPostFixWithSpaceTest()
        {
            string expression = "1+2+3";
            string output = "1 2 + 3 +";

            string result = SpreadsheetEngine.Expression.ConvertToPostFix(expression);
            Console.WriteLine (result);
            if (result != string.Empty)
            {
                Assert.AreEqual(output, result);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void ConvertPostfixParenthTest1()
        {
            string expression = "(1+2)";
            string output = "1 2 +";

            string result = SpreadsheetEngine.Expression.ConvertToPostFix(expression);
            Console.WriteLine(result);
            if (result != string.Empty)
            {
                Assert.AreEqual(output, result);
            }
            else
            {
                Assert.Fail();
            }

        }
        [Test]
        public void HW9_Test1()
        {
            Spreadsheet spreadsheet = new Spreadsheet(10, 10);
            spreadsheet.spreadsheetCells[0, 0].CellText = "HELLO, WORLD!";
            
            FileStream fileStream = new FileStream("hw9_test1.xml", FileMode.OpenOrCreate);
            spreadsheet.Save(fileStream);
            Spreadsheet newSpreadsheet = new Spreadsheet(10, 10);
            fileStream.Close();
            fileStream = new FileStream("hw9_test1.xml", FileMode.Open);
            newSpreadsheet.Load(fileStream);

            Console.WriteLine(spreadsheet.spreadsheetCells[0, 0].CellText.ToString());
            Console.WriteLine("Original SpreadSheet -> : " + spreadsheet.spreadsheetCells[0, 0].CellText.ToString());
            Console.WriteLine("New Spreadsheet using Load -> : " + newSpreadsheet.spreadsheetCells[0, 0].CellText);
            if (newSpreadsheet.spreadsheetCells[0,0].CellValue == spreadsheet.spreadsheetCells[0,0].CellValue)
            {
                Assert.Pass();
                return;
            }

            Assert.Fail();
        }
        [Test]
        public void HW10_Test1()
        {
            Spreadsheet spreadsheet = new Spreadsheet(10, 10);
            spreadsheet.spreadsheetCells[0, 0].CellText = "100";
            spreadsheet.spreadsheetCells[0, 1].CellText = "=A1";

            if (spreadsheet.spreadsheetCells[0,0].CellValue == spreadsheet.spreadsheetCells[0,1].CellValue)
            {
                Assert.Pass();
                Console.WriteLine(spreadsheet.spreadsheetCells[0, 0].CellValue);
                Console.WriteLine(spreadsheet.spreadsheetCells[0, 1].CellValue);
                return;
            }
            Console.WriteLine(spreadsheet.spreadsheetCells[0, 0].CellValue);
            Console.WriteLine(spreadsheet.spreadsheetCells[0, 1].CellValue);
            Assert.Fail();
        }
        [Test]
        public void HW10_Test2()
        {
            Spreadsheet spreadsheet = new Spreadsheet(10, 10);
            spreadsheet.spreadsheetCells[0, 0].CellText = "100";
            spreadsheet.spreadsheetCells[0, 1].CellText = "=A1+10";
            spreadsheet.spreadsheetCells[0, 1].Evaluate();
            string answer = "110";

            if (answer == spreadsheet.spreadsheetCells[0, 1].CellValue)
            {
                Assert.Pass();
                Console.WriteLine(spreadsheet.spreadsheetCells[0, 0].CellValue);
                Console.WriteLine(spreadsheet.spreadsheetCells[0, 1].CellValue);
                return;
            }
            Console.WriteLine(spreadsheet.spreadsheetCells[0, 0].CellValue);
            Console.WriteLine(spreadsheet.spreadsheetCells[0, 1].CellValue);
            Assert.Fail();
        }
    }
}