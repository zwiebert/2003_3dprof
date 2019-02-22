using System;
using System.IO;

namespace TDProf.App
{

	/// <summary>
	/// Summary description for AppEventLog.
	/// </summary>
	public class AppEventLog
	{
		public AppEventLog()
		{

		}

    static public void log_clock(string msg) {
      App.AppEventLog.LogEvent logEvent = new App.AppEventLog.LogEvent(AppEventLog.LogEvent.Type.Clocking, msg);
      write(logEvent);
    }

    
    static public void log_app(string msg) {
      App.AppEventLog.LogEvent logEvent = new App.AppEventLog.LogEvent(AppEventLog.LogEvent.Type.Application, msg);
      write(logEvent);
    }

    
    public void write_log(LogEvent e) {
      System.Windows.Forms.MessageBox.Show(e.ToString(), "LogEvent");
    }

    static private void write(LogEvent e) {
      try {
        StreamWriter stw = File.AppendText("app.log"); //new StreamWriter("app.log", true, System.Text.Encoding.ASCII);
        stw.WriteLine(e.ToString());
        stw.Close();
      } catch {}
    }


    public class LogEvent {

      public enum Type {
        Clocking,
        Registry,
        Application,
      };

      static private string[] TypeTags = {
                                           "C",
                                           "R",
                                           "A",
      };

      public LogEvent(Type type, string message) {
        m_type = type;
        m_message = message;
        m_time = System.DateTime.Now;
      }

      public override string ToString() {
        string result = string.Format("D={0}|T={1}|M={2}",
          m_time.ToString("s") + "," + m_time.Millisecond.ToString("d3"),
          TypeTags[(int)m_type],
          m_message);
        return result;
      }

      private Type            m_type;
      private string          m_message; 
      private System.DateTime m_time;
    }


  }
}
