using System;

namespace MiP.Tools.IO
{
    public class FileCopyProgressChangedEventArgs : EventArgs
    {
        public FileCopyProgressChangedEventArgs(double percent)
        {
            Percent = percent;
        }

        public double Percent { get; private set; }
    }
}