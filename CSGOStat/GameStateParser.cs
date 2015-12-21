using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSGSI;
using CSGSI.Nodes;
using Caliburn.Micro;

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
        }
        

        void OnNewGameState(GameState gs)
        {
            Currentgs = gs;
            You = gs.Player;
            tms = GetTeams(gs);
            BombHandling(gs);
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
