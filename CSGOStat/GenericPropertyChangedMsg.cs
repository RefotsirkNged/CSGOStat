using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace CSGOStat
{
    class GenericPropertyChangedMsg : PropertyChangedBase
    {
        private Delegate _property;
        public GenericPropertyChangedMsg(Delegate property)
        {
            Property = property;
        }

        public Delegate Property
        {
            get
            {
                return _property;
            }

            set
            {
                _property = value;
                NotifyOfPropertyChange(() => Property);
            }
        }
    }
}
