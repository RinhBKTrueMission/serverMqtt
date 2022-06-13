using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Device.Views.Home
{
    class Default : TableView<Models.Device>
    {
        class DigitalButton : MyButton
        {
        }
        class Indicator : MyTableLayout
        {
            List<DigitalButton> _buttons;
            string[] _digitalCss;

            public List<DigitalButton> Buttons => _buttons;
            public Indicator(int rows, int cols, string css, Action<DigitalButton, int> created) : base(rows, cols)
            {
                _digitalCss = css.Split(',');
                _buttons = new List<DigitalButton>();
                for (int i = 0, k = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++, k++)
                    {
                        var btn = new DigitalButton();
                        this.Add(i, j, btn);

                        _buttons.Add(btn);

                        created?.Invoke(btn, k);
                    }
                }
            }

            public void Render(int value, int index = 0)
            {
                foreach (var btn in _buttons)
                {
                    int on = value & (1 << index++);
                    btn.Css = _digitalCss[on == 0 ? 0: 1];
                }
            }
        }

        Indicator _inputs;
        Indicator _outputs;
        Indicator _leftLeds;
        Indicator _rightLeds;

        void RenderIO()
        {
            _inputs.Render(Model.Inputs);
            _outputs.Render(Model.Outputs);
            _leftLeds.Render(Model.Leds);
            _rightLeds.Render(Model.Leds, 4);
        }

        protected override void RenderCore()
        {
            var screen = new PanelElement<StackPanel> {
                Background = Brushes.Black,
            };

            string[] leds = new string[] { 
                "POWER",
                "ARMED",
                "NET",
                "ALARM",
                "OUT 0",
                "OUT 1",
                "OUT 2",
                "OUT 3",
            };

            var screenContainer = new MyTableLayout(1, 3);
            screenContainer.Add(0, 0, _leftLeds = new Indicator(4, 1, "io,led-on", (b, i) => {
                b.Text = leds[i];
            }));
            screenContainer.Add(0, 2, _rightLeds = new Indicator(4, 1, "io,led-on", (b, i) => {
                b.Text = leds[i + 4];
            }));
            screenContainer.Add(0, 1, screen);
            screenContainer.SetWidths(GridUnitType.Star, 1, 8, 1);

            var buttonsContainer = new MyTableLayout(4, 4) {
                Width = 320,
                Height = 200,
            };
            var ioContainer = new MyTableLayout(1, 2);

            _mainContent.AddRow(screenContainer);
            _mainContent.AddRow(buttonsContainer);
            _mainContent.AddRow(ioContainer);

            _mainContent.SetHeights(GridUnitType.Star, 4, 4, 2);

            string[] btnCaptions = new string[] { 
                "1",
                "2",
                "3",
                "UP",
                "4",
                "5",
                "6",
                "DOWN",
                "7",
                "8",
                "9",
                "DEL",
                "*",
                "0",
                "#",
                "ENTER",
            };

            int r = 0, c = 0;
            foreach (var s in btnCaptions)
            {
                var btn = new MyButton { 
                    Text = s,
                    FontSize = 20,
                    Url = "home/keyboard/" + s,
                };
                buttonsContainer.Add(r, c++, btn);
                if (c == 4)
                {
                    ++r;
                    c = 0;
                }
            }

            ioContainer.Add(0, 0, _inputs = new Indicator(1, 4, "io,alarm", (b, i) => {
                b.Text = "INPUT " + i;
                b.Url = "home/input/" + i;
            }));
            ioContainer.Add(0, 1, _outputs = new Indicator(1, 4, "io,relay-on", (b, i) => {
                b.Text = "RELAY " + i;
            }));
            RenderIO();

            Model.StatusChanged += (i, o) => {
                AsyncUpdate(() => RenderIO());
            };
        }
    }
}
