using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CStringer.Animation
{
    abstract class Animator<T>: IAnimation<T>
    {
        public enum AnimationMode { Normal, Reset, Reverse }
        public enum InitWith { FpsDuration, FramesDuration, FramesFps }

        public AnimationMode Mode { get; set; } = AnimationMode.Normal;
        public T From { get; set; }
        public T To { get; set; }
        public T AnimValue { get; protected set; }
        public T Step { get; set; }
        public int Interval { get; set; }
        public bool IsActive { get; private set; }

        public event EventHandler<T> AnimTick;
        public event EventHandler<T> Completed;
        public event EventHandler<T> Started;


        InitWith InitType { get; set; }
        Timer TimerP { get; }
        int Source1 { get; set; }
        int Source2 { get; set; }

        protected int FrameCount { get; private set; }
        protected abstract T NextValue();
        protected abstract bool IsFinish();
        protected abstract T GetStep();

        public Animator(T from, T to, InitWith initType, int source1, int source2)
        {
            TimerP = new Timer();
            TimerP.Tick += TimerTickHandle;

            Update(from, to, initType, source1, source2);
        }

        private void Reset()
        {
            AnimValue = From;
        }        

        private void Update(
            T newFrom, T newTo, InitWith initType, int source1, int source2)
        {
            /* duration: 2500 ms
             * fps: 30
             * interval = 1000/fps = 33,(3)
             * framecount = duration/1000 * fps
             * step = delta/framecount
             */

            this.From = newFrom;
            this.AnimValue = From;
            this.To = newTo;
            this.Source1 = source1;
            this.Source2 = source2;
            this.InitType = initType;
            

            if (initType == InitWith.FpsDuration)
            {
                int fps = source1;
                int duration = source2;
                FrameCount = (int)((float)duration / 1000 * fps);

                this.Interval = 1000 / fps;
            }
            else if (initType == InitWith.FramesDuration)
            {
                FrameCount = source1;
                int duration = source2;

                this.Interval = (int)((float)duration / FrameCount);
            }
            else if (initType == InitWith.FramesFps)
            {
                FrameCount = source1;
                int fps = source2;

                this.Interval = 1000 / fps;
            }
            else
                throw new ArgumentException("Wrong initialization type parameter.");


            TimerP.Interval = Interval;
            this.Step = GetStep();
        }


        private void TimerTickHandle(object obj, EventArgs e)
        {
            // Increasing animvalue on each tick
            AnimValue = NextValue();

            // Check if current animation cycle is completed
            if (IsFinish())
            {
                // Finish if it's normal mode
                if (this.Mode == AnimationMode.Normal)
                {
                    this.Stop();
                    return;
                }
                // Reset animation and go on
                else if (this.Mode == AnimationMode.Reset)
                    Update(From, To, InitType, Source1, Source2);
                // Reverse animation and go on
                else if (this.Mode == AnimationMode.Reverse)
                    Update(To, From, InitType, Source1, Source2);
            }
            // Notify about next animation tick after all verifications
            this.AnimTick?.Invoke(this, AnimValue);
        }


        public void Start()
        {
            // TODO: More arguments
            this.IsActive = true;

            if (TimerP.Enabled)
                TimerP.Stop();

            Update(From, To, InitType, Source1, Source2);

            TimerP.Start();
            this.Started?.Invoke(this, AnimValue);
        }

        public void Stop()
        {
            this.IsActive = false;

            TimerP.Stop();
            this.Completed?.Invoke(this, AnimValue);
        }        
    }
}
