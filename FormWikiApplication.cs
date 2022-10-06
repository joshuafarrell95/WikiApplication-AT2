using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

// Joshua Farrell, M153428
// 12/10/2022

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
                addInformation.SetStructure(GetStructureRadioButton());
                addInformation.SetDefinition(textBoxDefinition.Text);

                /* Commit the class object to the Wiki */
                Wiki.Add(addInformation);
                ClearUIElements("record");
                SortWiki();
            }
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
                    statusStrip.Items.Add("Record " + textBoxName.Text + " added to Wiki.");
                    return true;
                }
                else
                {
                    statusStrip.Items.Add("Fill in all the details for Wiki.");
                }
            }
            else
            {
                SearchRecord(textBoxName.Text);
                statusStrip.Items.Clear();      /* Needed to clear statusStrip items from SearchRecord() */
                statusStrip.Items.Add("Record " + textBoxName.Text + " already exists, the existing record is populated.");
            }
            return false;
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

        private int GetSelectedIndex()
        {
            try
            {
                return listViewWiki.SelectedIndices[0];
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Trace.TraceError(ex.Message);
                return -1;
            }
        }

        private void TextBoxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            FilterInput(e);
        }


        private void TextBoxDefinition_KeyPress(object sender, KeyPressEventArgs e)
        {
            FilterInput(e);
        }

        private void TextBoxSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            FilterInput(e);
        }

        private void FilterInput(KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            
            /* Only allow letters, punctuation, spaces, and the delete key */
            if (((!char.IsLetter(c) && !char.IsPunctuation(c)) && !char.IsSeparator(c)) && c != 0x0008)
            {
                /* Handle the event and not print out the invalid character */
                e.Handled = true;

                /* Output a message to Trace (console) and statusStrip (user-facing) */
                string message = "Invalid character " + c + " filtered.";
                Trace.TraceInformation("6.3 - FilterInput() - " + message);

                statusStrip.Items.Clear();
                statusStrip.Items.Add(message);
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
            bool isValid = false;
            Trace.TraceInformation("6.5 - ValidName string newName is: " + newName);
            
            if (!Wiki.Exists(w => w.GetName() == newName))
            {
                isValid = true;
            }
            Trace.TraceInformation("6.5 - ValidName bool isValid: " + isValid);
            return isValid;
        }
        #endregion

        // 6.6 Create two methods to highlight and return the values from the Radio button GroupBox.
        // The first method must return a string value from the selected radio button (Linear or Non-Linear).
        // The second method must send an integer index which will highlight an appropriate radio button.
        #region 6.6
        private string GetStructureRadioButton()
        {
            string rbValue = "";
            foreach (RadioButton rb in groupBoxStructure.Controls.OfType<RadioButton>())
            {
                if (rb.Checked)
                {
                    rbValue = rb.Text;
                    break;
                }
                else
                {
                    rbValue = "Other";
                }
            }
            return rbValue;
        }

        /* Default method to reset all radio buttons */
        private void SetStructureRadioButton()
        {
            foreach (RadioButton rb in groupBoxStructure.Controls.OfType<RadioButton>())
            {
                rb.Checked = false;
            }
        }

        /* Overloaded method to set the radio button */
        private void SetStructureRadioButton(int ind)
        {
            foreach (RadioButton rb in groupBoxStructure.Controls.OfType<RadioButton>())
            {
                if (rb.Text == Wiki[ind].GetStructure())
                {
                    rb.Checked = true;
                }
                else
                {
                    rb.Checked = false;
                }
            }
        }
        #endregion

        // 6.7 Create a button method that will delete the currently selected record in the ListView.
        // Ensure the user has the option to backout of this action by using a dialog box.
        // Display an updated version of the sorted list at the end of this process.
        #region 6.7
        private void ButtonDelete_MouseClick(object sender, MouseEventArgs e)
        {
            statusStrip.Items.Clear();
            string deletedRecordName = "";
            int selectedRecord = GetSelectedIndex();
            Trace.TraceInformation("6.7 - ButtonDelete_MouseClick string selectedRecord is " + selectedRecord);
            if (selectedRecord != -1)
            {
                deletedRecordName = Wiki[selectedRecord].GetName();
                Trace.TraceInformation("6.7 - ButtonDelete_MouseClick string deletedRecordName is " + deletedRecordName);
                var userDecision = MessageBox.Show("Are you sure you want to delete the selected record " + deletedRecordName + "?",
                    "Confirm record deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                Trace.TraceInformation("6.7 - ButtonDelete_MouseClick var userDecision is " + userDecision);

                if (userDecision == DialogResult.Yes)
                {
                    try
                    {
                        Wiki.RemoveAt(selectedRecord);
                        statusStrip.Items.Add("Record " + deletedRecordName + " successfully deleted");
                        Trace.TraceInformation("6.7 - Record " + deletedRecordName + " successfully deleted");
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        Trace.TraceError(ex.ToString());
                        statusStrip.Items.Add("Please select a valid record to delete");
                        Trace.TraceInformation("6.7 - Please select a valid record to delete");
                    }
                }
                /* Record not deleted */
                else
                {
                    statusStrip.Items.Add("Record " + deletedRecordName + " not deleted");
                    Trace.TraceInformation("6.7 - Record " + deletedRecordName + " not deleted");
                }
            }
            /* No record selected */
            else
            {
                statusStrip.Items.Add("No valid record selected");
                Trace.TraceInformation("6.7 - No valid record selected");
            }
            SortWiki();
        }
        #endregion

        // 6.8 Create a button method that will save the edited record of the currently selected item in the ListView.
        // All the changes in the input controls will be written back to the list.
        // Display an updated version of the sorted list at the end of this process.
        #region 6.8
        private void ButtonEdit_MouseClick(object sender, MouseEventArgs e)
        {            
            statusStrip.Items.Clear();
            string oldName;
            string newName;
            
            int selectedRecord = GetSelectedIndex();
            if (selectedRecord != -1)
            {
                try
                {
                    oldName = Wiki[selectedRecord].GetName();
                    newName = textBoxName.Text;
                    
                    Wiki[selectedRecord].SetName(newName);
                    Wiki[selectedRecord].SetCategory(comboBoxCategory.Text);
                    Wiki[selectedRecord].SetStructure(GetStructureRadioButton());
                    Wiki[selectedRecord].SetDefinition(textBoxDefinition.Text);

                    /* Record name changed */
                    if (oldName != newName)
                    {
                        statusStrip.Items.Add("Record " + oldName + " edited and renamed to " + newName);
                        Trace.TraceInformation("6.8 - Record " + oldName + " edited and renamed to " + newName);
                    }
                    /* Record name unchanged */
                    else
                    {
                        statusStrip.Items.Add("Record " + oldName + " edited");
                        Trace.TraceInformation("6.8 - Record " + oldName + " edited");
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Trace.TraceError(ex.Message);
                }
            }
            /* No record selected */
            else
            {
                if (Wiki.Count > 0)
                {
                    statusStrip.Items.Add("Please select a record to edit");
                    Trace.TraceInformation("6.8 - Please select a record to edit");
                }
                else
                {
                    statusStrip.Items.Add("Please add a record to the wiki");
                    Trace.TraceInformation("6.8 - Please add a record to the wiki");
                }
            }
            SortWiki();
        }
        #endregion

        // 6.9 Create a single custom method that will sort and then display the Name and Category from the wiki information in the list.
        #region 6.9
        private void SortWiki()
        {
            Wiki.Sort();
            DisplayList();
        }
        #endregion

        // 6.10 Create a button method that will use the builtin binary search to find a Data Structure name.
        // If the record is found the associated details will populate the appropriate input controls and highlight the name in the ListView.
        // At the end of the search process the search input TextBox must be cleared.
        #region 6.10
        private void ButtonSearch_MouseClick(object sender, MouseEventArgs e)
        {
            SearchRecord(textBoxSearch.Text);
        }

        private void SearchRecord(string searchString)
        {
            statusStrip.Items.Clear();

            if (!string.IsNullOrEmpty(searchString))
            {
                Information findInfo = new Information();
                findInfo.SetName(searchString);
                int foundIndex = Wiki.BinarySearch(findInfo);

                /* If Search string is found, highlight and populate the record */
                if (foundIndex >= 0)
                {
                    listViewWiki.SelectedItems.Clear();
                    listViewWiki.Focus();
                    listViewWiki.Items[foundIndex].Selected = true;

                    textBoxName.Text = Wiki[foundIndex].GetName();
                    comboBoxCategory.Text = Wiki[foundIndex].GetCategory();
                    SetStructureRadioButton(foundIndex);
                    textBoxDefinition.Text = Wiki[foundIndex].GetDefinition();

                    statusStrip.Items.Add(searchString + " found and highlighted");
                }
                /* Not found */
                else
                {
                    statusStrip.Items.Add(searchString + " cannot be found in the Wiki");
                    textBoxSearch.Clear();
                    textBoxSearch.Focus();
                }
            }
            /* Search text box empty */
            else
            {
                statusStrip.Items.Add("Please enter a search term in the search textbox");
                textBoxSearch.Focus();
            }
        }
        #endregion

        // 6.11 Create a ListView event so a user can select a Data Structure Name from the list of Names
        // and the associated information will be displayed in the related text boxes combo box and radio button.
        #region 6.11
        private void ListViewWiki_MouseClick(object sender, MouseEventArgs e)
        {
            WikiToUI(GetSelectedIndex());
        }

        private void ListViewWiki_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            statusStrip.Items.Clear();
            
            var userDecision = MessageBox.Show("Are you sure you want to delete all Wiki records?", "Confirm Wiki deletion", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            
            if (userDecision == DialogResult.OK)
            {
                Wiki.Clear();
                ClearUIElements();
            }
            else
            {
                statusStrip.Items.Add("Wiki deletion was cancelled, no records were deleted.");
            }
            DisplayList();
        }

        private void WikiToUI(int ind)
        {
            textBoxName.Text = Wiki[ind].GetName();
            comboBoxCategory.Text = Wiki[ind].GetCategory();
            SetStructureRadioButton(ind);
            textBoxDefinition.Text = Wiki[ind].GetDefinition();
        }
        #endregion

        // 6.12 Create a custom method that will clear and reset the TextBoxes, ComboBox and Radio button
        #region 6.12
        private void ClearUIElements()
        {
            ClearUIElements("all");
        }

        private void ClearUIElements(string mode)
        {
            switch (mode)
            {
                case "all":
                    ClearUIElements("record");
                    ClearUIElements("search");
                    break;
                case "record":
                    textBoxName.Clear();
                    comboBoxCategory.Text = "";
                    SetStructureRadioButton();
                    textBoxDefinition.Clear();
                    break;
                case "search":
                    textBoxSearch.Clear();
                    break;
                default:
                    //Trace.TraceError("ClearTextBoxes() method requires the mode to be set to either \r\n" +
                    //    "\"all\", \"record\" or \"search\"");
                    break;
            }
        }
        #endregion

        // 6.13 Create a double click event on the Name TextBox to clear the TextBboxes, ComboBox and Radio button.
        #region 6.13
        private void TextBoxName_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ClearUIElements();
        }

        #endregion

        // 6.14 Create two buttons for the manual open and save option; this must use a dialog box to select a file or rename a saved file.
        // All Wiki data is stored/retrieved using a binary reader/writer file format.
        #region 6.14
        const string DEFAULT_FILE_NAME = "WikiData.dat";
        /* Use currentFileName as a pointer with the default name*/
        string currentFileName = DEFAULT_FILE_NAME;

        private void ButtonLoad_MouseClick(object sender, MouseEventArgs e)
        {
            statusStrip.Items.Clear();
            string loadedFileName = "";

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Application.StartupPath;
            ofd.Filter = "dat files (*.dat)|*.dat";
            ofd.Title = "Open a DAT file";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                loadedFileName = LoadWikiData(ofd.FileName);

                if (loadedFileName != "")
                {
                    loadedFileName = Path.GetFileName(loadedFileName);

                    statusStrip.Items.Add("File " + loadedFileName + " loaded successfully");

                    currentFileName = loadedFileName;
                }
            }
        }

        private string LoadWikiData(string loadFileName)
        {
            try
            {
                Wiki.Clear();
                using (Stream stream = File.Open(loadFileName, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        while (stream.Position < stream.Length)
                        {
                            Information addInformation = new Information();

                            /* Set the new Information object's attributes */
                            addInformation.SetName(reader.ReadString());
                            addInformation.SetCategory(reader.ReadString());
                            addInformation.SetStructure(reader.ReadString());
                            addInformation.SetDefinition(reader.ReadString());

                            /* Commit the class object to the Wiki */
                            Wiki.Add(addInformation);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Trace.TraceInformation(ex.ToString());
                MessageBox.Show("File " + loadFileName + " was unable to be loaded due to an IO Error. Please try again.", "Load IO Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
            DisplayList();
            return Path.GetFileName(loadFileName);
        }

        private void ButtonSave_MouseClick(object sender, MouseEventArgs e)
        {
            statusStrip.Items.Clear();
            string savedFileName = "";

            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "dat files (*.dat)|*.dat";
            sfd.Title = "Save a DAT file";
            sfd.InitialDirectory = Application.StartupPath;
            sfd.AddExtension = true;
            sfd.DefaultExt = "dat";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string fileName = sfd.FileName;
                /* If there is a valid file name, save the file */
                if (sfd.FileName != "")
                {
                    savedFileName = SaveWikiData(fileName);
                }
                /* Else, use the default file name "WikiData.dat" */
                else
                {
                    savedFileName = SaveWikiData(Application.StartupPath + DEFAULT_FILE_NAME);
                }
            }

            if (savedFileName != "")
            {
                savedFileName = Path.GetFileName(savedFileName);

                statusStrip.Items.Add("File " + savedFileName + " saved successfully");
            }
        }

        private string SaveWikiData(string saveFileName)
        {
            SortWiki();
            try
            {
                using (Stream stream = File.Open(saveFileName, FileMode.Create))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        foreach (var record in Wiki)
                        {
                            writer.Write(record.GetName());
                            writer.Write(record.GetCategory());
                            writer.Write(record.GetStructure());
                            writer.Write(record.GetDefinition());
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Trace.TraceInformation(ex.ToString());
                MessageBox.Show("File " + saveFileName + " was unable to be saved due to an IO Error. Please try again.", "Save IO Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
            return Path.GetFileName(saveFileName);
        }
        #endregion

        // 6.15 The Wiki application will save data when the form closes.
        #region 6.15
        private void FormWikiApplication_FormClosing(object sender, FormClosingEventArgs e)
        {
            string message = "";
            string title = "Confirm Wiki Application closure?";
            var button = MessageBoxButtons.YesNoCancel;

            bool isWikiNotEmpty = false;

            /* If there is any Information object in the Wiki, offer to save it */
            if (Wiki.Count > 0)
            {
                message = "Are you sure you want to save your Wiki Application data before closing? \"Cancel\" will close this window without closing Wiki Application. \r\n\n" +
                "If you click on \"Yes\", the wiki data will be saved as " + currentFileName +
                "before this application closes.";
                isWikiNotEmpty = true;
            }
            /* Else if the Wiki is empty */
            else
            {
                message = "Are you sure you want to close Wiki Application?";
                button = MessageBoxButtons.OKCancel;
            }

            var userDecision = MessageBox.Show(message, title, button, MessageBoxIcon.Question);

            /* If the user clicks on Yes, save the data before closing if there is at least one record, */
            /* otherwise, don't save the Wiki */
            if (userDecision == DialogResult.Yes)
            {
                if (isWikiNotEmpty)
                {
                    if (currentFileName != DEFAULT_FILE_NAME)
                    {
                        SaveWikiData(currentFileName);
                    }
                    else
                    {
                        SaveWikiData(DEFAULT_FILE_NAME);
                    }
                }
            }
            /* Else stop the form from closing if Cancel is clicked */
            else if (userDecision == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            /* Implicit else if No or OK is clicked, just close the form */
        }
        #endregion

        // 6.16 All code is required to be adequately commented.
        // Map the programming criteria and features to your code/methods by adding comments above the method signatures.
        // Ensure your code is compliant with the CITEMS coding standards (refer http://www.citems.com.au/).
        #region 6.16
        /* Mouse Enter methods to display tooltips over GUI elements*/
        bool isToolTipsEnabled = true;

        private void TextBoxName_MouseEnter(object sender, EventArgs e)
        {
            DisplayToolTip("Enter the Data Structure Name here, or double click on this text box to clear all fields for this record.", textBoxName);
        }

        private void ComboBoxCategory_MouseEnter(object sender, EventArgs e)
        {
            DisplayToolTip("Select a Category here from this combo box.", comboBoxCategory);
        }

        private void RadioButtonLinear_MouseEnter(object sender, EventArgs e)
        {
            DisplayToolTip("Click on this radio button for a linear structure. \r\n" +
                "Clicking this radio button will clear the non-linear radio button.", radioButtonLinear);
        }

        private void RadioButtonNonLinear_MouseEnter(object sender, EventArgs e)
        {
            DisplayToolTip("Click on this radio button for a non-linear structure. \r\n" +
                "Clicking this radio button will clear the linear radio button.", radioButtonNonLinear);
        }

        private void TextBoxDefinition_MouseEnter(object sender, EventArgs e)
        {
            DisplayToolTip("Enter the Definition here.", textBoxDefinition);
        }

        private void ButtonAdd_MouseEnter(object sender, EventArgs e)
        {
            DisplayToolTip("Click on this button to add a record to the Wiki.", buttonAdd);
        }

        private void ButtonEdit_MouseEnter(object sender, EventArgs e)
        {
            DisplayToolTip("Click on this button to edit a record to the Wiki.", buttonEdit);
        }

        private void ButtonDelete_MouseEnter(object sender, EventArgs e)
        {
            DisplayToolTip("Click on this button to delete a record from the Wiki.", buttonDelete);
        }

        private void TextBox_MouseEnter(object sender, EventArgs e)
        {
            DisplayToolTip("Enter a search term here, then click on the SEARCH button", textBoxSearch);
        }

        private void ButtonSearch_MouseEnter(object sender, EventArgs e)
        {
            DisplayToolTip("Enter a search term in the Search textbox, then click on this button.", buttonSearch);
        }

        private void ListViewWiki_MouseEnter(object sender, EventArgs e)
        {
            DisplayToolTip("Click on a record to display its contents.\r\n" +
                "Double click to delete all records. ", listViewWiki);
        }

        private void ButtonLoad_MouseEnter(object sender, EventArgs e)
        {
            DisplayToolTip("Click on this button to load a wiki .dat file. \r\n" +
                "This will open a standard load file dialog", buttonLoad);
        }

        private void ButtonSave_MouseEnter(object sender, EventArgs e)
        {
            DisplayToolTip("Click on this button to save a wiki .dat file. \r\n" +
                "This will open a standard save file dialog", buttonSave);
        }

        /* Tooltip utilities */
        private void DisplayToolTip(string message, TextBox tb)
        {
            if (isToolTipsEnabled)
            {
                toolTip.SetToolTip(tb, message);
            }
        }

        private void DisplayToolTip(string message, ComboBox cb)
        {
            if (isToolTipsEnabled)
            {
                toolTip.SetToolTip(cb, message);
            }
        }

        private void DisplayToolTip(string message, RadioButton rdb)
        {
            if (isToolTipsEnabled)
            {
                toolTip.SetToolTip(rdb, message);
            }
        }

        private void DisplayToolTip(string message, ListView lv)
        {
            if (isToolTipsEnabled)
            {
                toolTip.SetToolTip(lv, message);
            }
        }

        private void DisplayToolTip(string message, Button btn)
        {
            if (isToolTipsEnabled)
            {
                toolTip.SetToolTip(btn, message);
            }
        }
        #endregion
    }
}
