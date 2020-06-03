using OMF.Common.Formatters;

namespace OMF.Common.Excel
{
    public class EntityImportItem
    {
        private string excelColumnName;
        private string businessRuleType;
        private bool isForeignkey;
        private string foreignkeyColumnName;
        private string baseInfoColumnName;
        private string defaultValue;
        private string uniqeKeyGetList;
        private Formatter formatter;

        public string PropName { get; set; }

        public string ExcelColumnName
        {
            get
            {
                return this.excelColumnName;
            }
            set
            {
                this.excelColumnName = value;
            }
        }

        public string BusinessRuleType
        {
            get
            {
                return this.businessRuleType;
            }
            set
            {
                this.businessRuleType = value;
            }
        }

        public bool IsForeignkey
        {
            get
            {
                return this.isForeignkey;
            }
            set
            {
                this.isForeignkey = value;
            }
        }

        public string ForeignkeyColumnName
        {
            get
            {
                return this.foreignkeyColumnName;
            }
            set
            {
                this.foreignkeyColumnName = value;
            }
        }

        public string BaseInfoColumnName
        {
            get
            {
                return this.baseInfoColumnName;
            }
            set
            {
                this.baseInfoColumnName = value;
            }
        }

        public string DefaultValue
        {
            get
            {
                return this.defaultValue;
            }
            set
            {
                this.defaultValue = value;
            }
        }

        public string UniqeKeyGetList
        {
            get
            {
                return this.uniqeKeyGetList;
            }
            set
            {
                this.uniqeKeyGetList = value;
            }
        }

        public Formatter Formatter
        {
            get
            {
                return this.formatter;
            }
            set
            {
                this.formatter = value;
            }
        }
    }
}
