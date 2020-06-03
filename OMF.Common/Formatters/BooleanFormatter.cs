namespace OMF.Common.Formatters
{
    public class BooleanFormatter : Formatter
    {
        private string trueValue;
        private string falseValue;

        public BooleanFormatter()
        {
            this.trueValue = "Yes";
            this.falseValue = "No";
        }

        public BooleanFormatter(string trueValue, string falseValue)
        {
            this.trueValue = trueValue;
            this.falseValue = falseValue;
        }

        public override string Format(object value)
        {
            if (value is bool)
            {
                if ((bool)value)
                    return this.trueValue;
                return this.falseValue;
            }
            if (value.ToString() == "0")
                return this.falseValue;
            if (value.ToString() == "1")
                return this.trueValue;
            return (string)null;
        }
    }
}
