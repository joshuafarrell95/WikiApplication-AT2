using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiApplication_AT2
{
    internal class Information : IComparable<Information>
    {
        private string name;
        private string category;
        private string structure;
        private string definition;

        #region Setters and Getters
        public void SetName(string newName)
        {
            name = newName;
        }

        public string GetName()
        {
            return name;
        }

        public void SetCategory(string newCategory)
        {
            category = newCategory;
        }

        public string GetCategory()
        {
            return category;
        }

        public void SetStructure(string newStructure)
        {
            structure = newStructure;
        }

        public string GetStructure()
        {
            return structure;
        }

        public void SetDefinition(string newDefinition)
        {
            definition = newDefinition;
        }

        public string GetDefinition()
        {
            return definition;
        }
        #endregion

        #region Utilities
        public int CompareTo(Information info)
        {
            return name.CompareTo(info.name);
        }
        #endregion
    }
}
