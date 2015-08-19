using System;
using Ending.Audio;
using SFML.Graphics;
using SFML.System;

namespace Ending.UI
{
    public class Button : Widget
    {
        public Label Label { get; private set; }

        private Frame Frame { get; }

        public Action ClickAction = delegate { };

        public Color DefaultTextColor;

        public Color TextHighlightColor;

        public string HoverSoundFilename;

        private bool _shouldPlayHoverSound = true;

        public Button(Text text)
        {
            Label = new Label(text);
            Frame = new Frame(new RectangleShape(), Color.Transparent);
            DefaultTextColor = new Color(242, 242, 242);
            TextHighlightColor = new Color(73, 73, 73);
            HoverSoundFilename = "sounds/buttonhover.wav";

            UpdateFrame();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(Label);
            target.Draw(Frame);
        }

        public override void Update(int mouseX, int mouseY, bool isLeftMouseButtonPressed)
        {
            if (Frame.BoundingRect.Contains(mouseX, mouseY))
            {
                HighlightText(TextHighlightColor);
                if (_shouldPlayHoverSound)
                {
                    AudioHandler.PlaySound(HoverSoundFilename);
                    _shouldPlayHoverSound = false;
                }

                if (isLeftMouseButtonPressed)
                {
                    ClickAction();
                }
            }
            else
            {
                HighlightText(DefaultTextColor);
                _shouldPlayHoverSound = true;
            }

        }

        private void HighlightText(Color color)
        {
            Label.Text.Color = color;
        }

        private void UpdateFrame()
        {
            var buttonBorder = new RectangleShape();
            var textRect = Label.Text.GetGlobalBounds();

            buttonBorder.Position = new Vector2f(textRect.Left, textRect.Top);
            buttonBorder.Size = new Vector2f(textRect.Width + 10, textRect.Height + 10);
            buttonBorder.FillColor = Color.Transparent;

            Frame.SetBorderRect(buttonBorder);
        }

        public void SetPosition(float x, float y)
        {
            Label.Text.Position = new Vector2f(x, y);
            UpdateFrame();
        }

        public void SetLabel(Label label)
        {
            Label = label;
            UpdateFrame();
        }
    }
}
