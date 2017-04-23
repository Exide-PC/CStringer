using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace CStringer.Custom
{
    class CustomButtonBase: Button
    {
        public enum CustomButtonState
        {
            Normal, Hot, Pressed, HotFocused, Focused, Disabled
        }

        public class ButtonStateArgs: EventArgs
        {
            public CustomButtonState PreviousState { get; private set; }
            public CustomButtonState CurrentState { get; private set; }
            

            public ButtonStateArgs(CustomButtonState prevState, CustomButtonState currentState)
            {
                this.PreviousState = prevState;
                this.CurrentState = currentState;
            }            
        }

        protected CustomButtonState State { get; private set; }
        protected virtual void OnStateChanged(ButtonStateArgs sevent) { }

        protected CustomButtonBase()
        {
            State = CustomButtonState.Normal;
        }

        private void UpdateState(CustomButtonState newState)
        {
            CustomButtonState oldState = this.State;
            this.State = newState;

            ButtonStateArgs args = new ButtonStateArgs(oldState, newState);

            OnStateChanged(args);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (this.Focused)
                UpdateState(CustomButtonState.HotFocused);
            else
                UpdateState(CustomButtonState.Hot);
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            UpdateState(CustomButtonState.Pressed);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent); 
            UpdateState(CustomButtonState.HotFocused);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (this.Focused)
                UpdateState(CustomButtonState.Focused);
            else
                UpdateState(CustomButtonState.Normal);
        }
        
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            UpdateState(CustomButtonState.Normal);
        }
    }
}
