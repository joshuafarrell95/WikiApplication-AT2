using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiApplication_AT2
{
    // 6.1 Create a separate class file to hold the four data items of the Data Structure(use the Data Structure Matrix as a guide).
    // Use private properties for the fields which must be of type “string”. The class file must have separate setters and getters,
    // add an appropriate IComparable for the Name attribute. Save the class as “Information.cs”.

    // This class implements IComparable for an Information class object
    internal class Information : IComparable<Information>
    {
        // The fields are private properties of type string
        private string name;
        private string category;
        private string structure;
        private string definition;

        #region Constructors
        public Information()
        {

        }
        #endregion

        // The class uses separate getters and setters
        #region Getters and Setters
        public string GetName()
        {
            return name;
        }

        public void SetName(string newName)
        {
            name = newName;
        }

        public string GetCategory()
        {
            return category;
        }

        public void SetCategory(string newCategory)
        {
            category = newCategory;
        }

        public string GetStructure()
        {
            return structure;
        }

        public void SetStructure(string newStructure)
        {
            structure = newStructure;
        }

        public string GetDefinition()
        {
            return definition;
        }

        public void SetDefinition(string newDefinition)
        {
            definition = newDefinition;
        }
        #endregion

        // This class uses an overloaded CompareTo statement for the Name attribute
        #region Utilities
        public int CompareTo(Information info)
        {
            return name.CompareTo(info.name);
        }
        #endregion
    }
}
