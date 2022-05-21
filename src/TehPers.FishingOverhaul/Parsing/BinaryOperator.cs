namespace TehPers.FishingOverhaul.Parsing
{
    /// <summary>
    /// A binary operataor in an expression.
    /// </summary>
    internal enum BinaryOperator
    {
        /// <summary>
        /// The add operator (<c>+</c>).
        /// </summary>
        Add,

        /// <summary>
        /// The subtraction operator (<c>-</c>).
        /// </summary>
        Subtract,

        /// <summary>
        /// The multiplication operator (<c>*</c>).
        /// </summary>
        Multiply,

        /// <summary>
        /// The division operator (<c>/</c>).
        /// </summary>
        Divide,

        /// <summary>
        /// The modulus operator (<c>%</c>).
        /// </summary>
        Modulo,

        /// <summary>
        /// The power operator (<c>^</c>).
        /// </summary>
        Power,
    }
}
