namespace MiP.IO.Win32
{
    public enum CopyFileCallbackAction
    {
        // continue normally
        Continue = 0,
        // cancel an delete destination
        Cancel = 1,
        // pause copying (end pause by continue)
        Stop = 2,
        // continue, but dont call the callback anymore
        Quiet = 3
    }
}