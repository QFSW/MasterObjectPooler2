namespace QFSW.MOP2
{
    /// <summary>
    /// Allows the object to receive information about the pool that it is a part of.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Initializes the template object with the parent pool.
        /// </summary>
        /// <param name="pool">The pool that this template belongs to.</param>
        void InitializeTemplate(ObjectPool pool);
    }
}
