using System;
using System.Threading.Tasks;

namespace CAF.IM.Web
{
    public partial class Startup
    {
        private static void SetupErrorHandling()
        {
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                try
                {
                    // Write all unobserved exceptions
                    ReportError(e.Exception);
                }
                catch
                {
                    // Swallow!
                }
                finally
                {
                    e.SetObserved();
                }
            };
        }

        private static void ReportError(Exception e)
        {

        }
    }
}