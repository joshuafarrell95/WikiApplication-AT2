using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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


    }
}
