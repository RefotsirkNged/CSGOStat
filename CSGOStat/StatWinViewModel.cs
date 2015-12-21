using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using CSGSI;
using System.IO;

namespace CSGOStat
{

    [Export(typeof(IShell))]
    class StatWinViewModel : PropertyChangedBase , IShell
    {
        string _backgroundIMG;
        private string _currentTeam;

        private GameStateParser _GSP;




        [ImportingConstructor]
        public StatWinViewModel()
        {
            GSP = new GameStateParser(3000);
        }









        public string BackgroundIMG
        {
            get
            {
                return _backgroundIMG;
            }
            set
            {
                _backgroundIMG = value;
                NotifyOfPropertyChange(() => BackgroundIMG);
            }
        }
        public string CurrentTeam
        {
            get
            {
                return _currentTeam;
            }
            set
            {
                _currentTeam = value;
                NotifyOfPropertyChange(() => CurrentTeam);
            }
        }
        public GameStateParser GSP
        {
            get
            {
                return _GSP;
            }

            set
            {
                _GSP = value;
                NotifyOfPropertyChange(() => GSP);
            }
        }
    }
}
