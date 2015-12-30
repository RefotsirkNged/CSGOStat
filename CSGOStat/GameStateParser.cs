using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSGSI;
using CSGSI.Nodes;
using Caliburn.Micro;
using System.Timers;
using System.Globalization;

namespace CSGOStat
{
    public struct Teams
    {
        public List<PlayerNode> Teammates;
        public List<PlayerNode> Opponents;
    }
    public struct Score
    {
        public int ctScore;
        public int tScore;
    }
    class GameStateParser : PropertyChangedBase
    {
        public bool GSInit;

        private GameStateListener gsl;
        private GameState _currentgs;
        private bool _isBombPlanted;
        private PlayerNode _you;
        private Teams tms;
        private Timer _timer;
        private DateTime _roundTime;


        //This should be run in a try catch
        public GameStateParser(int Port)
        {
            Gsl = new GameStateListener(Port);
            Gsl.NewGameState += new NewGameStateHandler(OnNewGameState);
            if (!Gsl.Start())
            {
                GSInit = false;
                throw new GameStateInitFailedException("GameStateIntegration Initialization failed");
            }
            GSInit = true;
            InitTimer();
        }
        
        private void InitTimer()
        {
            _timer = new Timer(1000);
            //Autoreset kept off to not have unneeded time changed events.
            _timer.AutoReset = true;
            _timer.Elapsed += OnTimeElapsedOneSecond;
        }

        //1 sec elapsed.
        private void OnTimeElapsedOneSecond(Object source, ElapsedEventArgs e)
        {
            _roundTime.Add(new TimeSpan(0, 0, 0, 1));
        }

        void OnNewGameState(GameState gs)
        {
            Currentgs = gs;
            You = gs.Player;
            tms = GetTeams(gs);
            BombHandling(gs);
            TimerHandling(gs);
        }

        private void TimerHandling(GameState gs)
        {
            //Logic happening in different round phases happens here
            switch (gs.Round.Phase)
            {
                case RoundPhase.Live:
                    //If timer isnt enabled, and as the case states, the round is live, we start the timer, and initialize the time to 2 min.
                    //Timer.start just changes the Timer.Enabled property to true, nothing else afaik.
                    if (!_timer.Enabled)
                    {
                        _timer.Start();
                        _roundTime = Parse_time("02:00");
                    }
                    break;
                default:
                    //If it hits any unhandled case, default to stopping the clock, and setting time to zero.
                    _timer.Stop();
                    _roundTime = Parse_time("00:00");
                    break;
            }
        }

        //parses time input in the format "mm:ss", and returns a datetime. 
        private DateTime Parse_time(string time)
        {
            DateTime dTime;
            var format = "mm:ss";
            CultureInfo provider = CultureInfo.InvariantCulture;
            dTime = DateTime.ParseExact(time, format, provider);
            return dTime;
        }

        public DateTime RoundTime
        {
            get { return _roundTime; }
        }
        public Score GetScore()
        {
            Score _score;
            _score.ctScore = Currentgs.Map.TeamCT.Score;
            _score.tScore = Currentgs.Map.TeamT.Score;
            return _score;
        }
        public Teams GetTeams(GameState gs)
        {
            List<PlayerNode> teammates = new List<PlayerNode>();
            List<PlayerNode> opponents = new List<PlayerNode>();
            foreach (PlayerNode Player in gs.AllPlayers)
            {
                if (Player.Team == You.Team)
                    teammates.Add(Player);
                else
                    opponents.Add(Player);
            }

            Teams teams;
            teams.Teammates = teammates;
            teams.Opponents = opponents;

            return teams; 
        }

        private void BombHandling(GameState gs)
        {
            if(!IsBombPlanted &&
                gs.Round.Phase == RoundPhase.Live &&
                gs.Round.Bomb == BombState.Planted &&
                gs.Previously.Round.Bomb == BombState.Undefined)
            {
                IsBombPlanted = true;
            }
            else if (IsBombPlanted && gs.Round.Phase == RoundPhase.FreezeTime)
            {
                IsBombPlanted = false;
            }
        }

        public bool IsBombPlanted
        {
            get
            {
                return _isBombPlanted;
            }

            set
            {
                _isBombPlanted = value;
                NotifyOfPropertyChange(() => IsBombPlanted);
            }
        }
        public PlayerNode You
        {
            get
            {
                return _you;
            }

            set
            {
                _you = value;
                NotifyOfPropertyChange(() => You);
            }
        }
        public GameStateListener Gsl
        {
            get
            {
                return gsl;
            }

            set
            {
                gsl = value;
                NotifyOfPropertyChange(() => Gsl);
            }
        }
        public Teams Tms
        {
            get
            {
                return tms;
            }

            set
            {
                tms = value;
                NotifyOfPropertyChange(() => Tms);
            }
        }
        public GameState Currentgs
        {
            get
            {
                return _currentgs;
            }

            set
            {
                _currentgs = value;
                NotifyOfPropertyChange(() => Currentgs);
            }
        }
    }
}
