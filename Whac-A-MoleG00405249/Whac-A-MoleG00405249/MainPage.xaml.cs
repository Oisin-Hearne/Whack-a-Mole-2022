using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace Whac_A_MoleG00405249
{
    /* G00405249 Oisin Hearne
     * On startup, presents the user with the option of 3x3 or 5x5 before starting
     * Game lasts 100 seconds, and moles are worth more points the further the game continues.
     * As the game continues, it gets harder. A rare version of a mole may appear that rewards extra points, too.
     * When the game concludes, it resets everything and asks the user if they'd like to continue.
     */

    public partial class MainPage : ContentPage
    {
        //Mole 2 and 3 are only used past a certain time.
        ImageButton _Mole1 = new ImageButton(), _Mole2 = new ImageButton(), _Mole3 = new ImageButton();
        System.Timers.Timer _time;
        Random _rng = new Random();
        int _Count = 1000; //1sec
        int _moleExists; //tracks no. of moles on screen
        int _gridSize;
        int _score;
        int _stage;
        int _stopClicked;


        public MainPage()
        {
            InitializeComponent();
            
            CreateTimer();

            //Mole attributes are set up in the constructor. 3 different mole objects are used for the stage and the bonus points.
            _Mole1.Source = "moleactive.png";
            _Mole1.IsVisible = false;
            _Mole1.Clicked += _Mole1_Clicked;
            _Mole2.Source = "moleactive.png";
            _Mole2.IsVisible = false;
            _Mole2.Clicked += _Mole2_Clicked;
            _Mole3.Source = "moleshadesactive.png";
            _Mole3.IsVisible = false;
            _Mole3.Clicked += _Mole3_Clicked;
        }

        #region Timer Content
        private void CreateTimer()
        {
            //using the way we learned it in the lecture on thursday, System.Timers.Timer as it seemed to lag less than the other method.
            _time = new System.Timers.Timer();
            _time.Interval = _Count;

            _time.Elapsed += _time_Elapsed;
        }

        private void _time_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvokeOnMainThread(
                () =>
                {
                    timeUpdate();
                });
        }

        private void timeUpdate()
        {
            //Runs every (hopefully) second

            
            LblTime.Text = Convert.ToString(Convert.ToInt32(LblTime.Text) - 1); //Decrements timer

            //Based on the current time remaining, sets the stage & text colour.
            if(Convert.ToInt32(LblTime.Text) <= 100 && Convert.ToInt32(LblTime.Text) > 50) //100 - 50
            {
                _stage = 1;

                LblTime.TextColor = Color.Lime;
            }
            else if(Convert.ToInt32(LblTime.Text) < 50 && Convert.ToInt32(LblTime.Text) > 25) //50 - 25
            {
                _stage = 2;
                LblTime.TextColor = Color.Yellow;
            }
            else if(Convert.ToInt32(LblTime.Text) < 25 && Convert.ToInt32(LblTime.Text) > 0) //25 - 0
            {
                _stage = 3;
                LblTime.TextColor = Color.Red;
            }
            else if(Convert.ToInt32(LblTime.Text) <= 0) //0
            {
                //Ends the game

                EndTheGame();

            }
            SpawnMole();
        }

        #endregion

        //Ends the game for use at the end of the timer and when the stop button is pushed. Resets everything to as it was when the app is started up.
        private void EndTheGame()
        {
            _time.Stop();
            Game3x3.IsVisible = false;
            Game5x5.IsVisible = false;

            //Resets variables used.
            _moleExists = 0;
            _stage = 1;
            _stopClicked = 0;
            LblTime.Text = "100";
            LblScore.Text = "000";
            _score = 0;

            //Displays score to user using the Instruction Box from when the app is started.
            LblInstruction.Text = "Your final score was " + _score + "! Well done!";
            InstructionBox.IsVisible = true;

            //Resets stop/start button.
            Btn_Start.Clicked -= Btn_Stop_Clicked;
            Btn_Start.Clicked += Btn_Start_Clicked;
            Btn_Start.BackgroundColor = Color.Green;
            Btn_Start.Text = "Start";
            Btn_Start.FontSize = 17;

            //removes the Moles from the grids so that they don't end up behind the grid when re-generated.
            Game3x3.Children.Remove(_Mole1);
            Game3x3.Children.Remove(_Mole2);
            Game3x3.Children.Remove(_Mole3);
            Game5x5.Children.Remove(_Mole1);
            Game5x5.Children.Remove(_Mole2);
            Game5x5.Children.Remove(_Mole3);
            _Mole1.IsVisible = false;
            _Mole2.IsVisible = false;
            _Mole3.IsVisible = false;
        }


        //Uses a very long nested if statement to decide if a mole should be spawned or despawned.
        //Based on chance and current stage.
        private void SpawnMole()
        {
            //Stage 1, first 50 seconds.
            if (_stage == 1)
            {
                //If there are zero moles on screen and it's in the first 50 seconds, a mole has a 75% chance of spawning a second.
                if (_moleExists <= 0 && _rng.Next(1, 100) > 25)
                {
                    _Mole1.IsVisible = true;
                    _Mole1.SetValue(Grid.RowProperty, _rng.Next(0, _gridSize));
                    _Mole1.SetValue(Grid.ColumnProperty, _rng.Next(0, _gridSize));
                    _moleExists++;
                }
                //If there is a mole on screen, it'll have a 40% chance of disappearing every second.
                else if (_moleExists == 1 && _rng.Next(1, 100) > 40)
                {
                    _Mole1.IsVisible = false;
                    _moleExists--;
                }
            }

            //Stage 2, from 50 to 25 seconds left.
            else if (_stage == 2)
            {
                //If there are zero moles on screen, a mole has an 90% chance of spawning.
                if (_moleExists <= 0 && _rng.Next(1, 100) > 10)
                {
                    _Mole1.IsVisible = true;
                    _Mole1.SetValue(Grid.RowProperty, _rng.Next(0, _gridSize));
                    _Mole1.SetValue(Grid.ColumnProperty, _rng.Next(0, _gridSize));
                    _moleExists++;
                }
                //If there is a mole on screen, there's a 70% chance of another spawning, and a further 10% chance of that spawned mole being a Shades mole.
                else if (_moleExists == 1 && _rng.Next(1, 100) > 30)
                {
                    if (_rng.Next(1, 100) > 10)
                    {
                        _Mole2.IsVisible = true;
                        _Mole2.SetValue(Grid.RowProperty, _rng.Next(0, _gridSize));
                        _Mole2.SetValue(Grid.ColumnProperty, _rng.Next(0, _gridSize));
                        _moleExists++;
                    }
                    else
                        _Mole3.IsVisible = true;
                    _Mole3.SetValue(Grid.RowProperty, _rng.Next(0, _gridSize));
                    _Mole3.SetValue(Grid.ColumnProperty, _rng.Next(0, _gridSize));
                    _moleExists++;

                }
                //If there is a mole on screen, it'll have a 70% chance of disappearing every second. Gets rid of oldest mole first.
                else if (_moleExists >= 1 && _rng.Next(1, 100) > 70)
                {
                    if (_Mole1.IsVisible)
                        _Mole1.IsVisible = false;
                    else if (_Mole2.IsVisible)
                        _Mole2.IsVisible = false;
                    else
                        _Mole3.IsVisible = false;
                    _moleExists--;
                }

            }

            //Stage 3, 25 seconds left.
            else if (_stage == 3)
            {
                //If there are zero moles on screen, two moles spawn.
                if (_moleExists == 0)
                {
                    _Mole1.IsVisible = true;
                    _Mole1.SetValue(Grid.RowProperty, _rng.Next(0, _gridSize));
                    _Mole1.SetValue(Grid.ColumnProperty, _rng.Next(0, _gridSize));
                    _moleExists++;

                    _Mole2.IsVisible = true;
                    _Mole2.SetValue(Grid.RowProperty, _rng.Next(0, _gridSize));
                    _Mole2.SetValue(Grid.ColumnProperty, _rng.Next(0, _gridSize));
                    _moleExists++;
                }
                //If there is a mole on screen, there's a 50% chance of another spawning, and a further 10% chance of that spawned mole being a Shades mole.
                else if (_moleExists == 1 && _rng.Next(1, 100) > 50)
                {
                    if (_rng.Next(1, 100) > 10)
                    {
                        _Mole2.IsVisible = true;
                        _Mole2.SetValue(Grid.RowProperty, _rng.Next(0, _gridSize));
                        _Mole2.SetValue(Grid.ColumnProperty, _rng.Next(0, _gridSize));
                        _moleExists++;
                    }
                    else
                        _Mole3.IsVisible = true;
                    _Mole3.SetValue(Grid.RowProperty, _rng.Next(0, _gridSize));
                    _Mole3.SetValue(Grid.ColumnProperty, _rng.Next(0, _gridSize));
                    _moleExists++;
                }
                //If there are two moles on scren, there's a 50% of a Shades mole spawning.
                else if (_moleExists == 2 && _rng.Next(1, 100) > 50)
                {
                    _Mole3.IsVisible = true;
                    _Mole3.SetValue(Grid.RowProperty, _rng.Next(0, _gridSize));
                    _Mole3.SetValue(Grid.ColumnProperty, _rng.Next(0, _gridSize));
                    _moleExists++;
                }
            //If there is a mole on screen, it'll have a 70% chance of disappearing every second. Gets rid of oldest mole first.
            else if (_moleExists >= 1 && _rng.Next(1, 100) > 70)
            {
                if (_Mole1.IsVisible)
                    _Mole1.IsVisible = false;
                else if (_Mole2.IsVisible)
                    _Mole2.IsVisible = false;
                else
                    _Mole3.IsVisible = false;
                _moleExists--;
            }
            }
        }

        //Starts the game, resetting the time and score and changing the start button to a stop button.
        //Uses the radio button selection to decide which grid it should generate using the GenerateGrid() method.
        private void Btn_Start_Clicked(object sender, EventArgs e)
        {
            InstructionBox.IsVisible = false;
            Btn_Start.Text = "Stop";
            Btn_Start.BackgroundColor = Color.Red;
            Btn_Start.Clicked -= Btn_Start_Clicked;
            Btn_Start.Clicked += Btn_Stop_Clicked;
            
            if(Rad3x3.IsChecked)
            {
                Game3x3.IsVisible = true;
                _gridSize = 3;
                

            }
            else if(Rad5x5.IsChecked)
            {
                Game5x5.IsVisible = true;
                _gridSize = 5;
            }

            GenerateGrid(_gridSize);

            _time.Start();


        }

        //Alternate clicked event for when the button changes to Stop.
        //Asks the user if they are sure by changing the text of the button and setting a variable that is checked if the button is clicked again.
        private void Btn_Stop_Clicked(object sender, EventArgs e)
        {
            if(_stopClicked == 0)
            {
                Btn_Start.Text = "Are you sure? \nClick again if so.";
                Btn_Start.FontSize = 14;
                _stopClicked = 1;
            }
            else if (_stopClicked == 1)
                EndTheGame();
        }


        //A nested for loop is used to generate gridbox images for every square of the chosen grid option.
        private void GenerateGrid(int size)
        {
            if(size==3) //Grid Option 3
            {
                for (int r = 0; r < size; r++)
                {
                    for (int c = 0; c < size; c++)
                    {
                        Game3x3.Children.Add(new Image
                        {
                            Source = "gridbox.png"
                        }, c, r);
                    }
                }
                Game3x3.Children.Add(_Mole1);
                Game3x3.Children.Add(_Mole2);
                Game3x3.Children.Add(_Mole3);
            }

            else //As there's no other option, this represents Grid Option 5.
            {
                for (int r = 0; r < size; r++)
                {
                    for (int c = 0; c < size; c++)
                    {
                        Game5x5.Children.Add(new Image
                        {
                            Source = "gridbox.png"
                        }, c, r);
                    }
                }
                Game5x5.Children.Add(_Mole1);
                Game5x5.Children.Add(_Mole2);
                Game5x5.Children.Add(_Mole3);
            }


        }

        //These three just hide the respective moles, increment the score and decrement the number of moles that currently exist.
        //I tried to make this all one method, but it didn't work :(
        //The score is multiplied by the current stage.
        private void _Mole3_Clicked(object sender, EventArgs e)
        {
            _Mole3.IsVisible = false;
            _moleExists--;
            _score += 100 * _stage;
            LblScore.Text = Convert.ToString(_score);
        }

        private void _Mole2_Clicked(object sender, EventArgs e)
        {
            _Mole2.IsVisible = false;
            _moleExists--;
            _score += 10*_stage;
            LblScore.Text = Convert.ToString(_score);
        }

        private void _Mole1_Clicked(object sender, EventArgs e)
        {
            _Mole1.IsVisible = false;
            _moleExists--;
            _score += 10 * _stage;
            LblScore.Text = Convert.ToString(_score);
        }

    }
}
