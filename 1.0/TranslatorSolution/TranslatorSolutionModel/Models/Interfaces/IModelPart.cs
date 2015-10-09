namespace TranslatorSolutionLib.Models.Interfaces
{
    /// <summary>
    /// Implements a visitor pattern enabled Accept method that can pass on an additional parameter.
    /// </summary>
    public interface IModelPart
    {
        /// <summary>
        /// Implements a method that accepts a visitor according to the visitor pattern.
        /// </summary>
        /// <param name="modelVisitor"></param>
        /// <param name="objCursor">May be an object that represents the current
        /// traversal state or me be null. This parameter should (at best) be
        /// handled from visited element back to visitor without being further
        /// evaluated.</param>
        void Accept(IVisitor modelVisitor, object objCursor);
    }
}
