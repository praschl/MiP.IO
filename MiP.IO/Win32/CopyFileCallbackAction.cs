namespace MiP.IO.Win32
{
    public enum CopyFileCallbackAction
    {
        /// <summary>
        /// Continue normally, or end a pause, paused by Stop().
        /// </summary>
        Continue = 0,

        /// <summary>
        /// Cancel the delete action, the destination file is being removed (TODO: test if removed)
        /// </summary>
        Cancel = 1,

        /// <summary>
        /// Pauses copying of the file until Continue() is called.
        /// </summary>
        Stop = 2,

        /// <summary>
        /// Continue copying, but dont call the event handler anymore.
        /// </summary>
        Quiet = 3
    }
}