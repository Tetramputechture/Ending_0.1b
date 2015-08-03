using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;
using Ending.Audio;

namespace Ending.UI
{
    public class Button : Widget
    {
        public Label label { get; private set; }

        private Frame frame { get; }

        public Action clickAction;

        public Color defaultTextColor;

        public Color textHighlightColor;

        public string hoverSoundFilename;

        private bool shouldPlayHoverSound = true;

        public Button(Text text)
        {
            label = new Label(text);
            frame = new Frame(new RectangleShape(), Color.Transparent);
            defaultTextColor = new Color(242, 242, 242);
            textHighlightColor = new Color(73, 73, 73);
            hoverSoundFilename = "sounds/buttonhover.wav";

            UpdateFrame();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(label);
            target.Draw(frame);
        }

        public override void Update(int mouseX, int mouseY, bool isLeftMouseButtonPressed)
        {
            if (frame.boundingRect.Contains(mouseX, mouseY))
            {
                HighlightText(textHighlightColor);
                if (shouldPlayHoverSound)
                {
                    PlayHoverSound(hoverSoundFilename);
                    shouldPlayHoverSound = false;
                }
                else
                {
                    PlayHoverSound(string.Empty);
                }
                if (clickAction != null && isLeftMouseButtonPressed)
                {
                    clickAction();
                }
            }
            else
            {
                HighlightText(defaultTextColor);
                shouldPlayHoverSound = true;
            }

        }

        private void HighlightText(Color color)
        {
            label.text.Color = color;
        }

        private void PlayHoverSound(string filename)
        {
            if (filename != string.Empty)
            {
                AudioHandler.PlaySound(filename);
            }
        }

        private void UpdateFrame()
        {
            RectangleShape buttonBorder = new RectangleShape();
            FloatRect textRect = label.text.GetGlobalBounds();

            buttonBorder.Position = new Vector2f(textRect.Left, textRect.Top);
            buttonBorder.Size = new Vector2f(textRect.Width + 10, textRect.Height + 10);
            buttonBorder.FillColor = Color.Transparent;

            frame.SetBorderRect(buttonBorder);
        }

        public void SetPosition(float x, float y)
        {
            label.text.Position = new Vector2f(x, y);
            UpdateFrame();
        }

        public void SetLabel(Label label)
        {
            this.label = label;
            UpdateFrame();
        }
    }
}
