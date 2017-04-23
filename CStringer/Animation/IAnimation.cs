using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStringer.Animation
{
    interface IAnimation<T>
    {
        T From { get; set; }
        T To { get; set; }
        T AnimValue { get; }
        T Step { get; set; }
        int Interval { get; set; }
        bool IsActive { get; }
        void Start();
        void Stop();

        event EventHandler<T> Completed;
        event EventHandler<T> Started;
        event EventHandler<T> AnimTick;
    }
}
