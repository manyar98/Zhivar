namespace OMF.Common.Formatters
{
    public abstract class Formatter : IFormatter
    {
        public abstract string Format(object value);
    }
}
