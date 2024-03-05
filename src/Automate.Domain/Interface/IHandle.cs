namespace Automate.Domain.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHandle
    {
        /// <summary>
        /// 
        /// </summary>
        string Command { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        void Execution(string[] args);
    }
}