﻿using System;
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
                addInformation.SetStructure(GetStructureRadioButton());
                addInformation.SetDefinition(textBoxDefinition.Text);

                /* Commit the class object to the Wiki */
                Wiki.Add(addInformation);
            }
            ClearUIElements("record");
            SortWiki();
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
                SearchRecord(textBoxName.Text);
                // Add highlight code here
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
            
            int selectedRecord = GetSelectedIndex();
            if (selectedRecord != -1)
            {
                var userDecision = MessageBox.Show("Are you sure you want to delete the selected record " + Wiki[selectedRecord].GetName() + "?",
                    "Confirm record deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (userDecision == DialogResult.Yes)
                {
                    try
                    {
                        Wiki.RemoveAt(selectedRecord);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        Trace.TraceError(ex.ToString());
                        statusStrip.Items.Add("Please select a valid record to delete");
                    }
                }
                /* Record not deleted */
                else
                {
                    statusStrip.Items.Add("Record " + Wiki[selectedRecord].GetName() + " not deleted");
                }
            }
            /* No record selected */
            else
            {
                statusStrip.Items.Add("No valid record selected");
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
                    }
                    /* Record name unchanged */
                    else
                    {
                        statusStrip.Items.Add("Record " + oldName + " edited");
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
                }
                else
                {
                    statusStrip.Items.Add("Please add a record to the wiki");
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

                if (foundIndex >= 0)
                {
                    listViewWiki.SelectedItems.Clear();
                    listViewWiki.Items[foundIndex].Selected = true;
                    listViewWiki.Focus();

                    textBoxName.Text = Wiki[foundIndex].GetName();
                    comboBoxCategory.Text = Wiki[foundIndex].GetCategory();
                    SetStructureRadioButton(foundIndex);
                    textBoxDefinition.Text = Wiki[foundIndex].GetDefinition();

                    // Add highlight code here
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
            //int selectedIndex = GetSelectedIndex();

            WikiToUI(GetSelectedIndex());
            //Trace.TraceInformation(selectedIndex.ToString());
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

        #endregion

        // 6.15 The Wiki application will save data when the form closes.
        #region 6.15

        #endregion

        // 6.16 All code is required to be adequately commented.
        // Map the programming criteria and features to your code/methods by adding comments above the method signatures.
        // Ensure your code is compliant with the CITEMS coding standards (refer http://www.citems.com.au/).
        #region 6.16

        #endregion


    }
}
