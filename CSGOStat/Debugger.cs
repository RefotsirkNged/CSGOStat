using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGOStat
{
  class Debugger : ILog
  {
    #region Fields
    private readonly Type _type;
    #endregion

    #region Constructors
    public Debugger(Type type)
    {
      _type = type;
    }
    #endregion

    #region Helper Methods
    private string CreateLogMessage(string format, params object[] args)
    {
      return string.Format("[{0}] {1}",
      DateTime.Now.ToString("o"),
      string.Format(format, args));
    }
    #endregion

    #region ILog Members
    public void Error(Exception exception)
    {
      Debug.WriteLine(CreateLogMessage(exception.ToString()), "ERROR");
    }

    public void Info(string format, params object[] args)
    {
      Debug.WriteLine(CreateLogMessage(format, args), "INFO");
    }

    public void Warn(string format, params object[] args)
    {
      Debug.WriteLine(CreateLogMessage(format, args), "WARN");
    }
    #endregion
    //Debugger for Caliburn: http://buksbaum.us/2010/08/08/how-to-do-logging-with-caliburn-micro%20(Naxos's%20conflicted%20copy%202015-05-19)/
  }
}
