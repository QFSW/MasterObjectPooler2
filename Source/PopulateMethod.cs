namespace QFSW.MOP2
{
    /// <summary>
    /// Determines how many objects are needed when populating a pool.
    /// </summary>
    public enum PopulateMethod
    {
        /// <summary>If set is used, then populate will ensure the final population is the specified count.</summary>
        Set = 0,

        /// <summary>If add is used, then populate will add the specified count to the current population.</summary>
        Add = 1
    }
}
