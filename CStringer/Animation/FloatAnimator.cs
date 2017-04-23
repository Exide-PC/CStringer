using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStringer.Animation
{
    class FloatAnimator : Animator<float>
    {
        public FloatAnimator(float from, float to, InitWith initType, int source1, int source2):
            base(from, to, initType, source1, source2)
        {

        }

        protected override float GetStep()
        {
            return (To - From) / FrameCount;
        }

        protected override bool IsFinish()
        {
            return From < To && AnimValue > To || To < From && AnimValue < To;
        }

        protected override float NextValue()
        {
            return AnimValue + Step;
        }
    }
}
