/*
 * Bruno Penna Barbosa de Almeida - 7772825
 * Rodrigo de Salles Cunha Ferreira - 7761158
 *
 * nPuzzle.cs
 * 
 * Revision History
 *      Bruno Penna Barbosa de Almeida, 2017.11.22: Created
 *      Rodrigo de Salles Cunha Ferreira, 2017.11.26: Included/Removed Comments
 *      Bruno Penna Barbosa de Almeida, 2017.11.27: Try Catchs
 *      Rodrigo de Salles Cunha Ferreira, 2017.11.29: Fixed infinite loop on solvable method
 *      Rodrigo de Salles Cunha Ferreira, 2017.11.30: Fixed negative numbers input
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nPuzzle
{
    /// <summary>
    /// Puzzle class
    /// </summary>
    public partial class Puzzle : Form
    {
        //Variable declarations
        const int STARTX = 75;
        const int STARTY = 170;
        //gap between each Tile
        const int STEP = 55;
        const int TILE_SIZE = 50;
        int[] randomNumbers = new int[0];
        int numberOfRows;
        int numberOfCol;
        //The number 0 in the array corresponds to the empty square
        const int BLANK_SPACE = 0;
        bool flagLoad = false;

        /// <summary>
        /// Initialize components
        /// </summary>
        public Puzzle()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Check if the array is a solvable game
        /// </summary>
        /// <param name="intArray">gets the shuffledArray</param>
        /// <returns>returns if the array is solvable or not</returns>
        public bool IsSolvable(int[] intArray)
        {
            bool solvable = false;
            int countInversions = 0;
            bool evenInversions = false;
            bool evenRowIndex = false;
            //Give me the row that the blank tile is
            int rowIndex;

            // for to find the blank space position
            for (int k = 0; k < intArray.Length; k++) 
            {
                if (intArray[k] == BLANK_SPACE)
                {
                    rowIndex = (k / numberOfCol);
                    if (rowIndex % 2 == 0)
                    {
                        evenRowIndex = true;
                        break;
                    }
                }
            }

            // for to count the inversions
            for (int i = 0; i < intArray.Length - 1; i++) 
            {
                for (int j = i + 1; j < intArray.Length; j++)
                {
                    if (intArray[i] > intArray[j] && intArray[j] != BLANK_SPACE)
                    {
                        countInversions++;
                    }
                }
            }

            if (countInversions % 2 == 0)
            {
                evenInversions = true;
            }

            if (intArray.Length % 2 != 0 && evenInversions)
            {
                solvable = true;
            }
            else if (intArray.Length % 2 == 0)
            {
                if (evenRowIndex && !evenInversions)
                {
                    solvable = true;
                }
                else if (!evenRowIndex && evenInversions)
                {
                    solvable = true;
                }
            }
            if (numberOfCol == 1 && countInversions > 0 || numberOfRows == 1 && countInversions > 0)
            {
                solvable = false;
            }
            if (numberOfCol == 1 && countInversions == 0 || numberOfRows == 1 && countInversions == 0)
            {
                solvable = true;
            }

            return solvable;
        }

        /// <summary>
        /// Shuffles the array
        /// </summary>
        /// <returns>returns the shuffled array</returns>
        public int[] ShuffleArray()
        {
            int totalSize = numberOfRows * numberOfCol;
            int[] shuffledArray = new int[totalSize];
            int tempVar;

            do
            {
                //Initialize the array
                for (int i = 0; i < totalSize; i++) 
                {
                    shuffledArray[i] = i;
                }
                Random z = new Random();

                for (int t = 0; t < shuffledArray.Length; t++)
                {
                    tempVar = shuffledArray[t];
                    int r = z.Next(t, shuffledArray.Length);
                    shuffledArray[t] = shuffledArray[r];
                    shuffledArray[r] = tempVar;
                }
            } while (!IsSolvable(shuffledArray));

            return shuffledArray;
        }

        /// <summary>
        /// Creates the Tiles
        /// </summary>
        public void GeneratePuzzle()
        {
            int count = 0;
            int tileStartX = STARTX;
            int tileStartY = STARTY;
            bool validColRow = true;
            const string BLANK_SPACE = "0";

            try
            {
                numberOfRows = int.Parse(txtRow.Text);
                numberOfCol = int.Parse(txtCol.Text);
                if (numberOfRows < 0 || numberOfCol < 0)
                {
                    throw new Exception();
                }
                validColRow = true;
            }
            catch (Exception)
            {
                validColRow = false;
                MessageBox.Show("Please type a valid positive column and row");
            }
            
            if (validColRow == true)
            {
                if (!flagLoad)
                {
                    randomNumbers = ShuffleArray();
                }

                for (int i = 0; i < numberOfRows; i++)
                {
                    for (int j = 0; j < numberOfCol; j++)
                    {
                        Tile t = new Tile(this);
                        t.Left = tileStartX;
                        t.Top = tileStartY;
                        t.Width = TILE_SIZE;
                        t.Height = TILE_SIZE;
                        t.Text = randomNumbers[count].ToString();
                        tileStartX += STEP;
                        count++;

                        if (t.Text != BLANK_SPACE)
                        {
                            this.Controls.Add(t);

                        }
                        t.Click += Button_Click;
                    }
                    tileStartY += STEP;
                    tileStartX = STARTX;
                }
            }
            
        }

        /// <summary>
        /// Deletes all buttons below the starting Y position
        /// </summary>
        public void DeleteButtons()
        {
            List<Tile> buttons = this.Controls.OfType<Tile>().ToList();
            foreach (Tile btn in buttons)
            {
                if (btn.Top >= STARTY)
                {
                    this.Controls.Remove(btn);
                    btn.Dispose();
                }
            }
        }

        /// <summary>
        /// Check winning condition
        /// </summary>
        /// <returns>return if the player won or not after every move</returns>
        public bool CheckWin()
        {
            for (int i = 0; i < randomNumbers.Length - 1; i++)
            {
                if (randomNumbers[i] != i + 1)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Create a save file
        /// </summary>
        /// <param name="fileName">name of the file</param>
        public void SaveFile(string fileName)
        {
            StreamWriter writer;
            writer = new StreamWriter(fileName);

            writer.WriteLine(numberOfRows);
            writer.WriteLine(numberOfCol);

            for (int i = 0; i < randomNumbers.Length; i++)
            {
                writer.WriteLine(randomNumbers[i]);
            }
            writer.Close();
        }

        /// <summary>
        /// Load an existing file
        /// </summary>
        /// <param name="fileName">name of the file</param>
        public void LoadFile(string fileName)
        {
            StreamReader reader;
            reader = new StreamReader(fileName);
            while (reader.EndOfStream == false)
            {
                numberOfRows = int.Parse(reader.ReadLine());
                numberOfCol = int.Parse(reader.ReadLine());
                int[] tempArray = new int[numberOfRows * numberOfCol];
                for (int i = 0; i < tempArray.Length; i++)
                {
                    tempArray[i] = Convert.ToInt32(reader.ReadLine());
                }
                randomNumbers = tempArray;
            }
            reader.Close();
            txtRow.Text = numberOfRows.ToString();
            txtCol.Text = numberOfCol.ToString();
            flagLoad = true;
            GeneratePuzzle();
        }

        /// <summary>
        /// Check which position the clicked Tile can move
        /// </summary>
        /// <param name="sender">the button that is originating the event</param>
        /// <param name="e"></param>
        private void Button_Click(object sender, EventArgs e)
        {
            Tile btn = (Tile)sender;

            //Question
            int position = 0;

            for (int i = 0; i < randomNumbers.Length; i++)
            {
                if (btn.Text == randomNumbers[i].ToString())
                {
                    position = i;
                }
            }

            if (position % numberOfCol != 0 && randomNumbers[position - 1] == BLANK_SPACE)
            {
                btn.Left -= STEP;
                randomNumbers[position - 1] = randomNumbers[position];
                randomNumbers[position] = BLANK_SPACE;
            }
            else if (position % numberOfCol != numberOfCol - 1 && randomNumbers[position + 1] == BLANK_SPACE)
            {
                btn.Left += STEP;
                randomNumbers[position + 1] = randomNumbers[position];
                randomNumbers[position] = BLANK_SPACE;
            }
            else if (position >= numberOfCol && randomNumbers[position - numberOfCol] == BLANK_SPACE)
            {
                btn.Top -= STEP;
                randomNumbers[position - numberOfCol] = randomNumbers[position];
                randomNumbers[position] = BLANK_SPACE;
            }
            else if (position < randomNumbers.Length - numberOfCol && randomNumbers[position + numberOfCol] == BLANK_SPACE)
            {
                btn.Top += STEP;
                randomNumbers[position + numberOfCol] = randomNumbers[position];
                randomNumbers[position] = BLANK_SPACE;
            }

            if (CheckWin())
            {
                MessageBox.Show("Congratulations, you have solved the puzzle", "Puzzle solved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DeleteButtons();
            }

        }

        /// <summary>
        /// Button to generate new Tiles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            DeleteButtons();
            GeneratePuzzle();
        }

        /// <summary>
        /// Button to save a new file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlgSave = new SaveFileDialog();
            dlgSave.DefaultExt = ".txt";
            DialogResult result = dlgSave.ShowDialog();
            switch (result)
            {
                case DialogResult.Cancel:
                    break;
                case DialogResult.OK:
                    try
                    {
                        string filename = dlgSave.FileName;
                        SaveFile(filename);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error in file save");
                    }

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Button to load a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            DeleteButtons();
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.DefaultExt = ".txt";
            DialogResult result = dlgOpen.ShowDialog();
            switch (result)
            {
                case DialogResult.Cancel:
                    break;
                case DialogResult.OK:
                    try
                    {
                        string filename = dlgOpen.FileName;
                        LoadFile(filename);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error in file load");
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
