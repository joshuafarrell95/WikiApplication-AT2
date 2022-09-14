using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
        private List<Information> Wiki = new List<Information>();

        // 6.3 Create a button method to ADD a new item to the list.
        // Use a TextBox for the Name input, ComboBox for the Category, Radio group for the Structure and Multiline TextBox for the Definition.
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
            else
            {
                statusStrip.Items.Add("Fill in all the details for Wiki");
            }
            Wiki.Sort();
            ClearTextBoxes("record");
            DisplayList();
        }

        private bool CheckAllGUIElements()
        {
            if (!string.IsNullOrEmpty(textBoxName.Text) &&
                !string.IsNullOrEmpty(comboBoxCategory.Text) && 
                (!string.IsNullOrEmpty(radioButtonLinear.Text) || 
                !string.IsNullOrEmpty(radioButtonNonLinear.Text)) &&
                !string.IsNullOrEmpty(textBoxDefinition.Text))
            {
                return true;
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
    }
}
