using System;

namespace MiP.IO
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