using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WikiApplication_AT2
{
    public partial class FormWikiApplication : Form
    {
        public FormWikiApplication()
        {
            InitializeComponent();
        }

        // 6.2 Create a global List<T> of type Information called Wiki.
        #region 6.2
        private List<Information> Wiki = new List<Information>();
        #endregion

        // 6.3 Create a button method to ADD a new item to the list.
        // Use a TextBox for the Name input, ComboBox for the Category, Radio group for the Structure and Multiline TextBox for the Definition.
        #region 6.3
        private void ButtonAdd_MouseClick(object sender, MouseEventArgs e)
        {
            AddInformation();
        }

        private void AddInformation()
        {
            statusStrip.Items.Clear();
            
            if (CheckAllGUIElements())
            {
                Information addInformation = new Information();

                /* Set the new Information object's attributes */
                addInformation.SetName(textBoxName.Text);
                addInformation.SetCategory(comboBoxCategory.Text);
                addInformation.SetStructure("Non-Linear");
                addInformation.SetDefinition(textBoxDefinition.Text);

                /* Commit the class object to the Wiki */
                Wiki.Add(addInformation);
            }
            Wiki.Sort();
            ClearTextBoxes("record");
            DisplayList();
        }

        private bool CheckAllGUIElements()
        {
            if (ValidName(textBoxName.Text))
            {
                if (!string.IsNullOrEmpty(textBoxName.Text) &&
                    !string.IsNullOrEmpty(comboBoxCategory.Text) &&
                    (!string.IsNullOrEmpty(radioButtonLinear.Text) ||
                    !string.IsNullOrEmpty(radioButtonNonLinear.Text)) &&
                    !string.IsNullOrEmpty(textBoxDefinition.Text))
                {
                    return true;
                }
                else
                {
                    statusStrip.Items.Add("Fill in all the details for Wiki");
                }
            }
            else
            {
                statusStrip.Items.Add("Record " + textBoxName.Text + " already exists.");
                // Add binary search code here
                // Add highlight code here
            }
            return false;
        }

        private void ClearTextBoxes(string mode)
        {
            switch (mode)
            {
                case "all":
                    ClearTextBoxes("record");
                    ClearTextBoxes("search");
                    break;
                case "record":
                    textBoxName.Clear();
                    textBoxDefinition.Clear();
                    break;
                case "search":
                    textBoxSearch.Clear();
                    break;
                default:
                    Trace.TraceError("ClearTextBoxes() method requires the mode to be set to either \r\n" +
                        "\"all\", \"record\" or \"search\"");
                    break;
            }
        }

        private void DisplayList()
        {
            listViewWiki.Items.Clear();
            foreach(var information in Wiki)
            {
                ListViewItem lvi = new ListViewItem(information.GetName());
                lvi.SubItems.Add(information.GetCategory());
                listViewWiki.Items.Add(lvi);
            }
        }
        #endregion

        // 6.4 Create a custom method to populate the ComboBox when the Form Load method is called. The six categories must be read from a simple text file.
        #region 6.4
        private void FormWikiApplication_Load(object sender, EventArgs e)
        {
            PopulateComboBox();
        }

        private void PopulateComboBox()
        {
            comboBoxCategory.Items.Clear();
            LoadTextFile("categories.txt");
        }

        private void LoadTextFile(string categoryTextFileName)
        {
            var categoryData = string.Empty;

            if (File.Exists(categoryTextFileName))
            {
                StreamReader stream = new StreamReader(categoryTextFileName);
                
                while (!stream.EndOfStream)
                {
                    try
                    {
                        categoryData = stream.ReadLine();
                        if (categoryData != null)
                        {
                            comboBoxCategory.Items.Add(categoryData);
                        }
                    }
                    catch (FileLoadException ex)
                    {
                        Trace.TraceError(ex.ToString());
                        MessageBox.Show("Unable to load categories.txt file.", "Categories not loaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (IOException ex)
                    {
                        Trace.TraceError(ex.ToString());
                        MessageBox.Show("Unable to load categories.txt file, the file may be corrupted or not be in the correct text file format.", "Categories not loaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            } 
            else
            {
                MessageBox.Show("Text file categories.txt does not exist.", "Categories file does not exist", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        // 6.5 Create a custom ValidName method which will take a parameter string value from the Textbox Name and returns a Boolean after checking for duplicates.
        // Use the built in List<T> method “Exists” to answer this requirement.
        #region 6.5
        private bool ValidName(string newName)
        {
            if (!Wiki.Exists(w => w.GetName() == newName))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
